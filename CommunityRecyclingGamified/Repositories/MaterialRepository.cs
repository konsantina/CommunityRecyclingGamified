using CommunityRecyclingGamified.Data;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunityRecyclingGamified.Repositories
{
    public class MaterialRepository: IMaterialRepository
    {
        private readonly AppDbContext _context;

        public MaterialRepository(AppDbContext context) {
            _context = context;
        }

        public bool AddMaterial(Material material)
        {
            _context.Materials.Add(material);

            return _context.SaveChanges() > 0;
        }

        public async Task<bool> Delete(int id)
        {
          var material = await _context.Materials.FirstOrDefaultAsync(u => u.Id == id);
            if (material == null)
                return false;
            _context.Materials.Remove(material);
            _context.SaveChanges();
            return true;
        }

        public async Task<IEnumerable<Material>> GetAllAsync()
        {
            return await _context.Materials.ToListAsync();
        }

        public async Task<Material> GetByIdAsync(int id)
        {
            return await _context.Materials.FirstOrDefaultAsync(m => m.Id == id);
        }

        public bool UpdateAsync(Material material, int id)
        {
            var result = _context.Materials.FirstOrDefault(x => x.Id == id);

            return true;

        }
    }
}
