using CorporateWeb.DataAccess;
using CorporateWeb.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace CorporateWeb.Business {
    public class ServiceManager : IServiceManager {
        private readonly IUnitOfWork _unitOfWork;
        public ServiceManager(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Service>> GetAllServicesAsync() {
            return await _unitOfWork.Services.GetAllAsync(s => s.IsActive);
        }
        public async Task AddServiceAsync(Service service) {
            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveAsync();
        }
    }
}