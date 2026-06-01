using CourseBank.interfaces;
using CourseBank.model;
using Microsoft.EntityFrameworkCore;

namespace CourseBank.database.repositories;

public class DepositProductRepository(CourseDbContext db) : IDepositProductRepository
{
    public IEnumerable<DepositProduct> GetAll() =>
        db.DepositProducts.AsNoTracking().OrderBy(p => p.Id).ToList();

    public DepositProduct? GetById(int id) =>
        db.DepositProducts.AsNoTracking().FirstOrDefault(p => p.Id == id);

    public void Add(DepositProduct product) => db.DepositProducts.Add(product);
}
