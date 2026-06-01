using CourseBank.interfaces;
using CourseBank.model;
using Microsoft.EntityFrameworkCore;

namespace CourseBank.database.repositories;

public class CreditApplicationRepository(CourseDbContext db) : ICreditApplicationRepository
{
    private static readonly CreditApplicationStatus[] ActiveStatuses =
    [
        CreditApplicationStatus.Pending,
        CreditApplicationStatus.Approved,
        CreditApplicationStatus.Issued
    ];

    public IEnumerable<CreditApplication> GetByUserId(int userId) =>
        db.CreditApplications.AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.AppliedAt)
            .ToList();

    public CreditApplication? GetById(int id) =>
        db.CreditApplications.AsNoTracking().FirstOrDefault(c => c.Id == id);

    public CreditApplication? GetByIdForUpdate(int id) =>
        db.CreditApplications.FirstOrDefault(c => c.Id == id);

    public bool HasActiveApplication(int userId) =>
        db.CreditApplications.Any(c => c.UserId == userId && ActiveStatuses.Contains(c.Status));

    public void Add(CreditApplication application) => db.CreditApplications.Add(application);

    public void Update(CreditApplication application) => db.CreditApplications.Update(application);
}
