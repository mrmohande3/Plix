using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlixP.Models;
using PlixP.Repositories.Contracts;

namespace PlixP.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly PlixContext _context;
        public RepositoryWrapper(PlixContext context)
        {
            _context = context;
        }
        public IBaseRepository<T> SetRepository<T>() where T : Entity
        {
            IBaseRepository<T> repository = new BaseRepository<T>(_context);
            return repository;
        }
    }
}
