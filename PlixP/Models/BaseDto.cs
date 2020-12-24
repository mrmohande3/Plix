using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace PlixP.Models
{
    /// <summary>
    /// Base Dto Class initial map config between entity and dto
    /// </summary>
    /// <typeparam name="TDto">Type of Dto Class</typeparam>
    /// <typeparam name="TEntity">Type of Entity Class</typeparam>
    public abstract class BaseDto<TDto, TEntity> : IMapperConf
        where TDto : class, new()
        where TEntity : new()
    {

        public TEntity ToEntity()
        {
            return Mapper.Map<TEntity>(CastToDerivedClass(this));
        }

        public TEntity ToEntity(TEntity entity)
        {
            return Mapper.Map(CastToDerivedClass(this), entity);
        }

        public static TDto FromEntity(TEntity model)
        {
            return Mapper.Map<TDto>(model);
        }

        protected TDto CastToDerivedClass(BaseDto<TDto, TEntity> baseInstance)
        {
            return Mapper.Map<TDto>(baseInstance);
        }

        public virtual void MapperConfig(Profile profile)
        {
            var mappingExpression = profile.CreateMap<TDto, TEntity>();

            var dtoType = typeof(TDto);
            var entityType = typeof(TEntity);
            //Ignore any property of source (like Post.Author) that dose not contains in destination 
            foreach (var property in entityType.GetProperties())
            {
                if (dtoType.GetProperty(property.Name) == null)
                    mappingExpression.ForMember(property.Name, opt => opt.Ignore());
            }

            CustomMappings(mappingExpression.ReverseMap());
        }

        public virtual void CustomMappings(IMappingExpression<TEntity, TDto> mapping)
        {
        }
    }
}
