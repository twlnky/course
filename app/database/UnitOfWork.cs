using CourseBank.interfaces;

namespace CourseBank.database;

public class UnitOfWork(CourseDbContext db) : IUnitOfWork
{
    public int SaveChanges() => db.SaveChanges();
}
