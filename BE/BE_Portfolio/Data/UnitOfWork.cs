namespace BE_Portfolio.Data;

public class UnitOfWork(PortfolioDbContext context) : IUnitOfWork
{
    public Task CommitAsync() => context.SaveChangesAsync();
}