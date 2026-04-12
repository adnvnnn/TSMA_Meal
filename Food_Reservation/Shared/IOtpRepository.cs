using Azure.Core;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace TSMA_Meal.Shared
{
    public interface IOtpRepository
    {
        void Add(string phone, string code, TimeSpan window);

        string? Get(string phone);

        void Delete(string phone);

        void AddAttempt(string phone);

        int GetAttemptCount(string phone);

    }

    public class OtpRepository(IMemoryCache cache) : IOtpRepository
    {
        public void Add(string phone, string code, TimeSpan window)
        {
            cache.Set(phone, code, window);
        }

        public string? Get(string phone)
        {
            cache.TryGetValue(phone, out string? hashedCode);
            return hashedCode;
        }

        public void Delete(string phone)
        {
            cache.Remove(phone);
        }

        public void AddAttempt(string phone)
        {
            var current = GetAttemptCount(phone);

            cache.Set($"rate:{phone}", current + 1, TimeSpan.FromMinutes(15));
        }

        public int GetAttemptCount(string phone)
        {
            return cache.Get<int>($"rate:{phone}");
        }
    }

}
