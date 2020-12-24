using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace PlixP.Models
{
    public interface IMapperConf
    {
        void MapperConfig(Profile profile);
    }
}
