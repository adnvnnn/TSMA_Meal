namespace TSMA_Meal.Shared
{
    public interface IOtpRepository
    {
        Task AddAsync(string phone, string code);

        Task<string?> GetAsync(string phone);

        Task DeleteAsync(string phone);

    }

    public class OtpRepository : IOtpRepository
    {
        public Task AddAsync(string phone, string code)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetAsync(string phone)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string phone)
        {
            throw new NotImplementedException();
        }
    }

}
