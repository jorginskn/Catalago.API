using APICatalago.Data;
using APICatalago.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalago.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
         

        public CategoryRepository(AppDbContext context): base(context) 
        {
            
        }

      
    }
}
