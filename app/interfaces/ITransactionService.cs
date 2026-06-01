using CourseBank.dto.request;
using CourseBank.dto.response;

namespace CourseBank.interfaces;

public interface ITransactionService
{
    IEnumerable<TransactionResponse> GetByAccountId(int accountId);
    TransactionResponse? GetById(int transactionId);
    TransactionResponse Create(CreateTransactionRequest request);
    TransactionResponse? UpdateStatus(int transactionId, UpdateTransactionStatusRequest request);
    OperationResultResponse Transfer(TransferRequest request);
    OperationResultResponse RepayCredit(CreditRepaymentRequest request);
}
