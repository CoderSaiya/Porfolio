namespace BE_Portfolio.Data;

public interface IUnitOfWork
{
    Task CommitAsync();
}