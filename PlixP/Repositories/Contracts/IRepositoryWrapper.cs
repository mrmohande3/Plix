using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlixP.Models;

namespace PlixP.Repositories.Contracts
{
    public interface IRepositoryWrapper
    {
        IBaseRepository<T> SetRepository<T>() where T : Entity;
    }
}
