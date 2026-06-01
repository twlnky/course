using CourseBank.dto.request;
using CourseBank.dto.response;
using CourseBank.interfaces;
using CourseBank.model;
using CourseBank.services;

namespace CourseBank.services;

public class PaymentRecipientService(
    IPaymentRecipientRepository recipientRepository,
    IUnitOfWork unitOfWork) : IPaymentRecipientService
{
    public IEnumerable<PaymentRecipientResponse> GetAll(string? name) =>
        recipientRepository.GetAll(name).Select(EntityMapper.ToPaymentRecipientResponse);

    public PaymentRecipientResponse? GetById(int recipientId)
    {
        var recipient = recipientRepository.GetById(recipientId);
        return recipient == null ? null : EntityMapper.ToPaymentRecipientResponse(recipient);
    }

    public PaymentRecipientResponse Create(CreatePaymentRecipientRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new AppException("Имя получателя обязательно.");
        if (string.IsNullOrWhiteSpace(request.BankAccount))
            throw new AppException("Банковский счёт получателя обязателен.");

        var recipient = new PaymentRecipient
        {
            Name = request.Name,
            TaxId = request.TaxId,
            BankAccount = request.BankAccount,
            Category = request.Category
        };

        recipientRepository.Add(recipient);
        unitOfWork.SaveChanges();
        return EntityMapper.ToPaymentRecipientResponse(recipient);
    }

    public PaymentRecipientResponse? Update(int recipientId, UpdatePaymentRecipientRequest request)
    {
        var recipient = recipientRepository.GetByIdForUpdate(recipientId);
        if (recipient == null) return null;

        if (!string.IsNullOrWhiteSpace(request.Name))
            recipient.Name = request.Name;
        if (request.TaxId != null)
            recipient.TaxId = request.TaxId;
        if (!string.IsNullOrWhiteSpace(request.BankAccount))
            recipient.BankAccount = request.BankAccount;
        if (request.Category.HasValue)
            recipient.Category = request.Category.Value;

        recipientRepository.Update(recipient);
        unitOfWork.SaveChanges();
        return EntityMapper.ToPaymentRecipientResponse(recipient);
    }

    public bool Delete(int recipientId)
    {
        var recipient = recipientRepository.GetByIdForUpdate(recipientId);
        if (recipient == null) return false;

        recipientRepository.Delete(recipient);
        unitOfWork.SaveChanges();
        return true;
    }
}
