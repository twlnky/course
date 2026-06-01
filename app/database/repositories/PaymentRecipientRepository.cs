using CourseBank.interfaces;
using CourseBank.model;
using Microsoft.EntityFrameworkCore;

namespace CourseBank.database.repositories;

public class PaymentRecipientRepository(CourseDbContext db) : IPaymentRecipientRepository
{
    public IEnumerable<PaymentRecipient> GetAll(string? nameFilter)
    {
        var query = db.PaymentRecipients.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(nameFilter))
            query = query.Where(r => r.Name.Contains(nameFilter));
        return query.OrderBy(r => r.Name).ToList();
    }

    public PaymentRecipient? GetById(int id) =>
        db.PaymentRecipients.AsNoTracking().FirstOrDefault(r => r.Id == id);

    public PaymentRecipient? GetByIdForUpdate(int id) =>
        db.PaymentRecipients.FirstOrDefault(r => r.Id == id);

    public void Add(PaymentRecipient recipient) => db.PaymentRecipients.Add(recipient);

    public void Update(PaymentRecipient recipient) => db.PaymentRecipients.Update(recipient);

    public void Delete(PaymentRecipient recipient) => db.PaymentRecipients.Remove(recipient);
}
