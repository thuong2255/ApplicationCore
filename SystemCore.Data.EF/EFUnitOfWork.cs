using SystemCore.Infrastructure.Interfaces;

namespace SystemCore.Data.EF
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContex _context;

        public EFUnitOfWork(AppDbContex context)
        {
            _context = context;
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}