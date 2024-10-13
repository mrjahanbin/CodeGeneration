using CodeGeneration.Repositories;

namespace CodeGeneration.UnitOfWork
{
    public interface IUnitOfWork
    {
        IProductRepository Products { get; }
        void Complete();
    }

    public class UnitOfWork : IUnitOfWork
    {
        public IProductRepository Products { get; } = new ProductRepository();

        public void Complete()
        {
            // Save changes
        }
    }
}
