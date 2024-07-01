using APICatalago.Controllers;
using APICatalago.Models;
using APICatalago.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace APICatalago.Repositories
{
    public interface ICategoryRepository :IRepository<Category>
    {
        public PagedList<Category> GetCategories(CategoriesParameters categoriesParams);
    }
}
