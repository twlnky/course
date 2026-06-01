using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using CourseBank.model;
using CourseBank.services;

namespace CourseBank.services;

public class PaymentTemplateService(
    IUserRepository userRepository,
    IPaymentRecipientRepository recipientRepository,
    IPaymentTemplateRepository templateRepository,
    IUnitOfWork unitOfWork) : IPaymentTemplateService
{
    public IEnumerable<PaymentTemplateResponse> GetByUserId(int userId) =>
        templateRepository.GetByUserId(userId).Select(EntityMapper.ToPaymentTemplateResponse);

    public PaymentTemplateResponse? GetById(int templateId)
    {
        var template = templateRepository.GetById(templateId);
        return template == null ? null : EntityMapper.ToPaymentTemplateResponse(template);
    }

    public PaymentTemplateResponse Create(CreatePaymentTemplateRequest request)
    {
        if (userRepository.GetById(request.UserId) == null)
            throw new AppException("Пользователь не найден.", StatusCodes.Status404NotFound);

        if (recipientRepository.GetById(request.RecipientId) == null)
            throw new AppException("Получатель платежа не найден.", StatusCodes.Status404NotFound);

        var template = new PaymentTemplate
        {
            UserId = request.UserId,
            RecipientId = request.RecipientId,
            Amount = request.Amount,
            Nickname = request.Nickname,
            IsActive = true,
            IsScheduled = request.IsScheduled,
            NextRunDate = request.NextRunDate,
            PeriodDays = request.PeriodDays
        };

        templateRepository.Add(template);
        unitOfWork.SaveChanges();

        var created = templateRepository.GetById(template.Id)!;
        return EntityMapper.ToPaymentTemplateResponse(created);
    }

    public PaymentTemplateResponse? Update(int templateId, UpdatePaymentTemplateRequest request)
    {
        var template = templateRepository.GetByIdForUpdate(templateId);
        if (template == null) return null;

        if (request.Amount.HasValue)
            template.Amount = request.Amount;
        if (request.Nickname != null)
            template.Nickname = request.Nickname;
        if (request.IsActive.HasValue)
            template.IsActive = request.IsActive.Value;
        if (request.IsScheduled.HasValue)
            template.IsScheduled = request.IsScheduled.Value;
        if (request.NextRunDate.HasValue)
            template.NextRunDate = request.NextRunDate;
        if (request.PeriodDays.HasValue)
            template.PeriodDays = request.PeriodDays;

        templateRepository.Update(template);
        unitOfWork.SaveChanges();
        return EntityMapper.ToPaymentTemplateResponse(template);
    }

    public bool Delete(int templateId)
    {
        var template = templateRepository.GetByIdForUpdate(templateId);
        if (template == null) return false;

        templateRepository.Delete(template);
        unitOfWork.SaveChanges();
        return true;
    }
}
