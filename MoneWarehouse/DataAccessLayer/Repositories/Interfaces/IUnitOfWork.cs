using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public interface IUnitOfWork : IDisposable
    {
        IClientRepository Clients { get; }
        ICodesRepository Codes { get; }
        IEmployeeRepository Employees { get; }
        IFormulaRepository Formulas { get; }
        IInjectionRepository Injections { get; }
        IInjectionDailyRepository InjectionDailies { get; }
        IInjectionStockRepository InjectionStocks { get; }
        IMaterialRepository Materials { get; }
        IPlıntusRepository Plintuses { get; }
        IPlıntusDailyRepository PlintusDailies { get; }
        IPlıntusStockRepository PlintusStocks { get; }
        IRequestRepository Requests { get; }
        ISalesRepository Sales { get; }
        ISalesDetailRepository SalesDetails { get; }
        ISizeRepository Sizes { get; }

        Task<int> CompleteAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}