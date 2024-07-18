using APICatalago.Controllers;
using APICatalago.Models;
using APICatalago.Pagination;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace APICatalago.Repositories
{
    public interface ICategoryRepository :IRepository<Category>
    {
        public Task<IPagedList<Category>> GetCategoriesAsync(CategoriesParameters categoriesParams);
        public Task<IPagedList<Category>> GetCategoryByNameAsync(CategoryFilterName categoryParams);
    }
}
