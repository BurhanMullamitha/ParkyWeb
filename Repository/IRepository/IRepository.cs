using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParkyWeb.Repository.IRepository
{
    interface IRepository<T> where T : class
    {
        Task<T> GetAsync(string url, int Id);
        Task<IEnumerable<T>> GetAllAsync(string url);
        Task<bool> CreateAsync(string url, T objToCreate);
        Task<bool> UpdateAsync(String url, T objToUpdate);
        Task<bool> DeleteAsync(String url, int Id);
    }
}
