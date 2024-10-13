using System.Collections.Generic;
using CodeGeneration.Domains;

namespace CodeGeneration.Repositories
{
    public interface IProductRepository
    {
        Product GetById(int id);
        IEnumerable<Product> GetAll();
        void Add(Product entity);
        void Update(Product entity);
        void Delete(int id);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new List<Product>();

        public Product GetById(int id) => _products.Find(p => p.Id == id);
        public IEnumerable<Product> GetAll() => _products;
        public void Add(Product entity) => _products.Add(entity);
        public void Update(Product entity)
        {
            var item = GetById(entity.Id);
            if (item != null)
            {
                // Update properties here
            }
        }
        public void Delete(int id) => _products.RemoveAll(p => p.Id == id);
    }
}
