using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using PlixP.Models;

namespace PlixP.Extentions
{
    public static class AutoMapperConfig
    {
        public static void ConfigurationMapper()
        {
            Mapper.Initialize(config => { config.AddCustomMapping(Assembly.GetEntryAssembly()); });
            Mapper.Configuration.CompileMappings();
        }

        public static void AddCustomMapping(this IMapperConfigurationExpression expression, Assembly assemblies)
        {
            var allTypes = assemblies.ExportedTypes;
            var list = allTypes
                .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(IMapperConf)))
                .Select(type => (IMapperConf)Activator.CreateInstance(type)).ToList();
            expression.AddProfile(new CustomMappingProfile(list));
        }
    }
}
