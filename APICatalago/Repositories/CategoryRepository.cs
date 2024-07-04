using APICatalago.Controllers;
using APICatalago.Data;
using APICatalago.Models;
using APICatalago.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalago.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {


        public CategoryRepository(AppDbContext context) : base(context)
        {

        }

        public PagedList<Category> GetCategories(CategoriesParameters categoriesParams)
        {
            var categories = GetAll().OrderBy(c => c.CategoryId).AsQueryable();
            var orderedCategories = PagedList<Category>.ToPagedList(categories, categoriesParams.PageNumber, categoriesParams.PageSize);
            return orderedCategories;
        }

        public PagedList<Category> GetCategoryByName(CategoryFilterName categoryParams)
        {
            var category = GetAll().AsQueryable();
            if (!string.IsNullOrEmpty(categoryParams.Name))
            {
                category = category.Where(c => c.Name.Contains(categoryParams.Name));
            }

            var filteredCategory = PagedList<Category>.ToPagedList(category, categoryParams.PageNumber, categoryParams.PageSize);
            return filteredCategory;
        }
    }
}
