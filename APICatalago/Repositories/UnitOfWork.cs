using APICatalago.Data;

namespace APICatalago.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private ProductRepository? _ProductRepository;
        private CategoryRepository? _CategoryRepository;
        public AppDbContext _context;


        public UnitOfWork(AppDbContext context)
        {

            _context = context;
        }

        public IProductRepository ProductRepository
        {
            get
            {
                //return _ProductRepository = _ProductRepository ?? new ProductRepository(_context);
                if (_ProductRepository is null)
                {
                    _ProductRepository = new ProductRepository(_context);
                }
                return _ProductRepository;
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                // return _CategoryRepository = _CategoryRepository ?? new CategoryRepository(_context);
                if (_CategoryRepository is null)
                {
                    _CategoryRepository = new CategoryRepository(_context);
                }
                return _CategoryRepository;
            }
        }

        public async Task commitAsync()
        {
           await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
