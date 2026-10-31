using CinemaDashboard.Data;
using CinemaDashboard.Models;
using CinemaDashboard.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaDashboard.Services;

public class CategoryService : ICategoryService, ICategoryManagementService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context) => _context = context;

    public async Task<List<Category>> GetAllAsync() =>
        await _context.Categories.AsNoTracking().ToListAsync();

    public Task<Category?> GetByIdAsync(int id) =>
        _context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Category> CreateAsync(CategoryFormViewModel vm)
    {
        var category = new Category { Name = vm.Name.Trim() };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> UpdateAsync(CategoryFormViewModel vm)
    {
        var category = await _context.Categories.FindAsync(vm.Id);
        if (category == null) return false;

        category.Name = vm.Name.Trim();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DeleteResult> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return DeleteResult.NotFound;

        if (await _context.Movies.AnyAsync(m => m.CategoryId == id))
            return DeleteResult.InUse;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return DeleteResult.Deleted;
    }
}