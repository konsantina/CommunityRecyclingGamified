using CommunityRecyclingGamified.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommunityRecyclingGamified.Repositories.Interfaces
{
    public interface IMaterialRepository
    {
       Task<IEnumerable<Material>> GetAllAsync();
       Task<Material> GetByIdAsync(int id);
       public bool AddMaterial(Material material);
       public bool UpdateAsync(Material material, int id);
       Task<bool> Delete(int id);

    }
}
