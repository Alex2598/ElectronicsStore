using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store
{
    public interface IComponentRepository
    {
        
        Task<Component[]> GetAllByUidAsync(string uid);

        Task<Component[]> GetAllByPackageOrNameOfComponentAsync(string PackageOrNameOfComponent);

        Task<Component> GetByIdAsync(int id);

        Task<Component[]> GetAllByIdsAsync(IEnumerable<int> componentIds);
    }
}
