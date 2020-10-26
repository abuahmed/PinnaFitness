using AutoMapper;
using PinnaFit.Core.Models;

namespace PinnaFit.WPF
{
    public static class MapperUtility<TEntity> where TEntity : EntityBase
    {
        public static EntityBase GetMap(TEntity source, TEntity destination)
        {
            Mapper.Reset();
            Mapper.CreateMap<TEntity, TEntity>();
            return Mapper.Map(source, destination);
        }
    }
}