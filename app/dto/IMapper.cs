using CourseBank.dto.response;
using CourseBank.model;

namespace CourseBank.dto;

public interface IMapper
{
    UserResponse Map(User user);
    AccountResponse Map(Account account);
    AccountBalanceResponse MapBalance(Account account);
    TransactionResponse Map(Transaction transaction);
    CreditApplicationResponse Map(CreditApplication application);
    PaymentRecipientResponse Map(PaymentRecipient recipient);
    PaymentTemplateResponse Map(PaymentTemplate template);
    DepositProductResponse Map(DepositProduct product);
    UserDepositResponse Map(UserDeposit deposit);
}
