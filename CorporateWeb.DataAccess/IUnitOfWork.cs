using System;
using System.Threading.Tasks;
using CorporateWeb.Entities; // Bu satır eksikti

namespace CorporateWeb.DataAccess {
    public interface IUnitOfWork : IDisposable {
        IRepository<Service> Services { get; }
        Task<int> SaveAsync();
    }
}