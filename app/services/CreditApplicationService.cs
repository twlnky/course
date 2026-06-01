using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using CourseBank.model;
using CourseBank.services;

namespace CourseBank.services;

public class CreditApplicationService(
    IUserRepository userRepository,
    IAccountRepository accountRepository,
    ICreditApplicationRepository creditApplicationRepository,
    IUnitOfWork unitOfWork) : ICreditApplicationService
{
    public IEnumerable<CreditApplicationResponse> GetByUserId(int userId) =>
        creditApplicationRepository.GetByUserId(userId).Select(EntityMapper.ToCreditApplicationResponse);

    public CreditApplicationResponse? GetById(int applicationId)
    {
        var application = creditApplicationRepository.GetById(applicationId);
        return application == null ? null : EntityMapper.ToCreditApplicationResponse(application);
    }

    public CreditApplicationResponse Submit(SubmitCreditApplicationRequest request)
    {
        if (request.AmountRequested <= 0 || request.TermMonths <= 0)
            throw new AppException("Сумма и срок должны быть больше нуля.");

        if (userRepository.GetById(request.UserId) == null)
            throw new AppException("Пользователь не найден.", StatusCodes.Status404NotFound);

        if (creditApplicationRepository.HasActiveApplication(request.UserId))
            throw new AppException("У пользователя уже есть активная кредитная заявка.", StatusCodes.Status409Conflict);

        var monthlyPayment = CreditCalculator.CalculateMonthlyPayment(
            request.AmountRequested, request.TermMonths, request.InterestRate);

        var application = new CreditApplication
        {
            UserId = request.UserId,
            AmountRequested = request.AmountRequested,
            TermMonths = request.TermMonths,
            InterestRate = request.InterestRate,
            MonthlyPayment = monthlyPayment,
            Status = CreditApplicationStatus.Pending,
            AppliedAt = DateTime.UtcNow
        };

        creditApplicationRepository.Add(application);
        unitOfWork.SaveChanges();
        return EntityMapper.ToCreditApplicationResponse(application);
    }

    public CreditApplicationResponse? Review(int applicationId, ReviewCreditApplicationRequest request)
    {
        var application = creditApplicationRepository.GetByIdForUpdate(applicationId);
        if (application == null) return null;

        if (application.Status != CreditApplicationStatus.Pending)
            throw new AppException("Рассматривать можно только заявки в статусе Pending.");

        if (request.Status is not (CreditApplicationStatus.Approved or CreditApplicationStatus.Rejected))
            throw new AppException("Статус рассмотрения должен быть Approved или Rejected.");

        application.Status = request.Status;
        application.DecisionAt = DateTime.UtcNow;
        application.OperatorComment = request.OperatorComment;

        creditApplicationRepository.Update(application);
        unitOfWork.SaveChanges();
        return EntityMapper.ToCreditApplicationResponse(application);
    }

    public AccountResponse IssueCredit(IssueCreditRequest request)
    {
        var application = creditApplicationRepository.GetByIdForUpdate(request.ApplicationId);
        if (application == null)
            throw new AppException("Кредитная заявка не найдена.", StatusCodes.Status404NotFound);

        if (application.Status != CreditApplicationStatus.Approved)
            throw new AppException("Выдать кредит можно только по одобренной заявке.");

        if (string.IsNullOrWhiteSpace(request.AccountNumber))
            throw new AppException("Номер счёта обязателен.");

        if (accountRepository.AccountNumberExists(request.AccountNumber))
            throw new AppException("Счёт с таким номером уже существует.", StatusCodes.Status409Conflict);

        var account = new Account
        {
            UserId = application.UserId,
            AccountNumber = request.AccountNumber,
            AccountType = AccountType.Credit,
            Currency = request.Currency,
            Balance = -application.AmountRequested,
            DebtBalance = application.AmountRequested,
            CreditLimit = application.AmountRequested,
            InterestRate = application.InterestRate,
            IsBlocked = false,
            CreatedAt = DateTime.UtcNow,
            CreditApplicationId = application.Id,
            NextPaymentDate = DateTime.UtcNow.AddMonths(1)
        };

        accountRepository.Add(account);
        application.Status = CreditApplicationStatus.Issued;
        creditApplicationRepository.Update(application);
        unitOfWork.SaveChanges();

        return EntityMapper.ToAccountResponse(account);
    }
}
