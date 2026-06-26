using CorporateWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CorporateWeb.Business {
    public interface IServiceManager {
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task AddServiceAsync(Service service);
    }
}