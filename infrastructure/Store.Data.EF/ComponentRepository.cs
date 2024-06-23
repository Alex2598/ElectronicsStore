using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Data.EF
{
    class ComponentRepository : IComponentRepository
    {
        private readonly DbContextFactory dbContextFactory;
        public ComponentRepository(DbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        public async Task<Component[]> GetAllByIdsAsync(IEnumerable<int> componentIds)
        {
            var dbContext = dbContextFactory.Create(typeof(ComponentRepository));

            var dtos = await dbContext.Components
                                       .Where(component => componentIds.Contains(component.Id))
                                       .ToArrayAsync();

            return dtos.Select(Component.Mapper.Map)
                       .ToArray();
        }

     
        public async Task<Component[]> GetAllByPackageOrNameOfComponentAsync(string PackageOrNameOfComponent)
        {
            var dbContext = dbContextFactory.Create(typeof(ComponentRepository));

            var parameter = new SqlParameter("@PackageOrNameOfComponent", PackageOrNameOfComponent);
            var dtos = await dbContext.Components
                                                  .FromSqlRaw("SELECT * FROM Components WHERE CONTAINS((Package, NameOfComponent), @PackageOrNameOfComponent)",
                                        parameter)
                                                  .ToArrayAsync();

            return dtos.Select(Component.Mapper.Map)
                       .ToArray();


        }


        public async Task<Component[]> GetAllByUidAsync(string uid)
        {
            var dbContext = dbContextFactory.Create(typeof(ComponentRepository));

            if (Component.TryFormatUId(uid, out string formattedUid))
            {
                var dtos = await dbContext.Components
                                          .Where(component => component.UId == formattedUid)
                                          .ToArrayAsync();

                return dtos.Select(Component.Mapper.Map)
                           .ToArray();
            }

            return new Component[0];
        }

        public async Task<Component> GetByIdAsync(int id)
        {
            var dbContext = dbContextFactory.Create(typeof(ComponentRepository));

            var dto = await dbContext.Components
                                    .SingleAsync(Component => Component.Id == id);

            return Component.Mapper.Map(dto);
        }
    }
}
