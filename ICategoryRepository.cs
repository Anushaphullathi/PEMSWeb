
using System.Collections.Generic;
using PEMS.Web.Models;

namespace PEMS.Web.DAL.Interfaces
{
    public interface ICategoryRepository
    {
        // Read: show user-owned + global categories
        IEnumerable<Category> GetAll(string userId);

        // Read one: user-owned or global (read-only)
        Category GetById(string userId, int id);

        // Create: user-owned only (UserId must be set)
        int Create(Category category);

        // Update: only if owned by user (global is read-only)
        bool Update(Category category);

        // Delete: only if owned by user (global cannot be deleted)
        bool Delete(string userId, int id);

        // Duplicate check within user's categories
        bool ExistsByName(string userId, string name, int? excludeId = null);
    }
}
