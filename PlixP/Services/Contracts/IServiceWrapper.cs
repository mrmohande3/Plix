using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlixP.Services.Contracts
{
    public interface IServiceWrapper
    {
        LocalServices LocalServices { get; }
        MovieServices MovieServices { get; }
        CatcherService CatcherService { get; }
    }
}
