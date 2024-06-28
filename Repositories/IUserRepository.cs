namespace ExpensesCalculator.Repositories
{
    public interface IUserRepository
    {
        bool UserExists(string userName);
    }
}
