using CourseBank.dto.request;
using CourseBank.dto.response;

namespace CourseBank.interfaces;

public interface IDepositService
{
    IEnumerable<DepositProductResponse> GetAllProducts();
    DepositProductResponse? GetProductById(int productId);
    DepositProductResponse CreateProduct(CreateDepositProductRequest request);
    IEnumerable<UserDepositResponse> GetUserDeposits(int userId);
    UserDepositResponse? GetUserDepositById(int userDepositId);
    UserDepositResponse OpenDeposit(OpenUserDepositRequest request);
    DepositClosureResponse EarlyWithdraw(EarlyWithdrawRequest request);
}
