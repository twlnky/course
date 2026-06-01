using CourseBank.interfaces;
using CourseBank.model;
using Microsoft.EntityFrameworkCore;

namespace CourseBank.database.repositories;

public class PaymentTemplateRepository(CourseDbContext db) : IPaymentTemplateRepository
{
    public IEnumerable<PaymentTemplate> GetByUserId(int userId) =>
        db.PaymentTemplates.AsNoTracking()
            .Include(t => t.Recipient)
            .Where(t => t.UserId == userId)
            .OrderBy(t => t.Id)
            .ToList();

    public PaymentTemplate? GetById(int id) =>
        db.PaymentTemplates.AsNoTracking()
            .Include(t => t.Recipient)
            .FirstOrDefault(t => t.Id == id);

    public PaymentTemplate? GetByIdForUpdate(int id) =>
        db.PaymentTemplates.Include(t => t.Recipient).FirstOrDefault(t => t.Id == id);

    public void Add(PaymentTemplate template) => db.PaymentTemplates.Add(template);

    public void Update(PaymentTemplate template) => db.PaymentTemplates.Update(template);

    public void Delete(PaymentTemplate template) => db.PaymentTemplates.Remove(template);

    public void DeactivateByUserId(int userId)
    {
        var templates = db.PaymentTemplates.Where(t => t.UserId == userId && t.IsActive).ToList();
        foreach (var template in templates)
            template.IsActive = false;
    }
}
