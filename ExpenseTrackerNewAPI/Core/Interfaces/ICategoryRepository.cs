using System.Collections.Generic;
using System.Threading.Tasks;
using ExpenseTrackerNewAPI.Core.Entities;

namespace ExpenseTrackerNewAPI.Core.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetByUserIdAsync(int userId);
        Task<bool> IsCategoryNameExistsAsync(int userId, string categoryName);
    }
} 