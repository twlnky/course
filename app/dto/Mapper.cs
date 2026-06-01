using CourseBank.dto.response;
using CourseBank.model;
using CourseBank.services;

namespace CourseBank.dto;

public class Mapper : IMapper
{
    public UserResponse Map(User user) => EntityMapper.ToUserResponse(user);
    public AccountResponse Map(Account account) => EntityMapper.ToAccountResponse(account);
    public AccountBalanceResponse MapBalance(Account account) => EntityMapper.ToAccountBalanceResponse(account);
    public TransactionResponse Map(Transaction transaction) => EntityMapper.ToTransactionResponse(transaction);
    public CreditApplicationResponse Map(CreditApplication application) => EntityMapper.ToCreditApplicationResponse(application);
    public PaymentRecipientResponse Map(PaymentRecipient recipient) => EntityMapper.ToPaymentRecipientResponse(recipient);
    public PaymentTemplateResponse Map(PaymentTemplate template) => EntityMapper.ToPaymentTemplateResponse(template);
    public DepositProductResponse Map(DepositProduct product) => EntityMapper.ToDepositProductResponse(product);
    public UserDepositResponse Map(UserDeposit deposit) => EntityMapper.ToUserDepositResponse(deposit);
}
