using CourseBank.model;

namespace CourseBank.dto.request;

public class UpdateTransactionStatusRequest
{
    public TransactionStatus Status { get; set; }
}
