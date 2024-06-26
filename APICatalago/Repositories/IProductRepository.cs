﻿using APICatalago.Models;
using APICatalago.Pagination;

namespace APICatalago.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        //IEnumerable<Product> GetProducts(ProductParameters productParams);
        PagedList<Product> GetProducts(ProductParameters productParams);
        IEnumerable<Product> GetProductsByCategory(int id);
        public PagedList<Product> GetProductByPrice(ProductFilterPrice productFilterParams);

    }
}
