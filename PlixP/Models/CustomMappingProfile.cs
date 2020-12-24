using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace PlixP.Models
{
    public class CustomMappingProfile : Profile
    {
        public CustomMappingProfile(IEnumerable<IMapperConf> mapperConfs)
        {
            foreach (var mapperConf in mapperConfs)
                mapperConf.MapperConfig(this);
        }
    }
}
