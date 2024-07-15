namespace ExpensesCalculator.Repositories.Interfaces
{
    public interface IUserRepository
    {
        bool UserExists(string userName);
    }
}
