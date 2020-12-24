using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlixP.Services.Contracts;

namespace PlixP.Services
{
    public class ServiceWrapper : IServiceWrapper
    {
        private LocalServices _localServices;

        public LocalServices LocalServices
        {
            get
            {
                if (_localServices == null)
                    _localServices = new LocalServices();
                return _localServices;
            }
        }

        private MovieServices _movieServices;

        public MovieServices MovieServices
        {
            get
            {
                if (_movieServices == null)
                    _movieServices = new MovieServices();
                return _movieServices;
            }
        }

        private CatcherService _catcherService;

        public CatcherService CatcherService
        {
            get
            {
                if (_catcherService == null)
                    _catcherService = new CatcherService();
                return _catcherService;
            }
        }
    }
}
