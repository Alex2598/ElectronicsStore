using Store.Web.App;
using System.Collections.Generic;
using System.Linq;
using System;

using System.Threading.Tasks;
namespace Store.Web.App
{
    public class ComponentService
    {
        private readonly IComponentRepository componentRepository;

        public ComponentService(IComponentRepository componentRepository)
        {
            this.componentRepository = componentRepository;
        }

        public async Task<ComponentModel> GetByIdAsync(int id)
        {
            var component = await componentRepository.GetByIdAsync(id);

            return Map(component);
        }

        public async Task<IReadOnlyCollection<ComponentModel>> GetAllByQueryAsync(string query)
        {
            var component = Component.IsUId(query)
                      ? await componentRepository.GetAllByUidAsync(query)
                      : await componentRepository.GetAllByPackageOrNameOfComponentAsync(query);

            return component.Select(Map)
                        .ToArray();
        }

        private ComponentModel Map(Component component)
        {
            return new ComponentModel
            {
                Id = component.Id,
                UId = component.UId,
                NameOfComponent = component.NameOfComponent,
                Package = component.Package,
                Description = component.Description,
                Price = component.Price,
            };
        }
    }
}
