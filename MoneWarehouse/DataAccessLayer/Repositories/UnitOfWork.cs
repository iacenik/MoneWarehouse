using DataAccessLayer.Data;
using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool disposed = false;
        private IDbContextTransaction _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Clients = new ClientRepository(_context);
            Codes = new CodesRepository(_context);
            Employees = new EmployeeRepository(_context);
            Formulas = new FormulaRepository(_context);
            Injections = new InjectionRepository(_context);
            InjectionDailies = new InjectionDailyRepository(_context);
            InjectionStocks = new InjectionStockRepository(_context);
            Materials = new MaterialRepository(_context);
            Plintuses = new PlıntusRepository(_context);
            PlintusDailies = new PlıntusDailyRepository(_context);
            PlintusStocks = new PlıntusStockRepository(_context);
            Requests = new RequestRepository(_context);
            Sales = new SalesRepository(_context);
            SalesDetails = new SalesDetailRepository(_context);
            Sizes = new SizeRepository(_context);
        }

        public IClientRepository Clients { get; private set; }
        public ICodesRepository Codes { get; private set; }
        public IEmployeeRepository Employees { get; private set; }
        public IFormulaRepository Formulas { get; private set; }
        public IInjectionRepository Injections { get; private set; }
        public IInjectionDailyRepository InjectionDailies { get; private set; }
        public IInjectionStockRepository InjectionStocks { get; private set; }
        public IMaterialRepository Materials { get; private set; }
        public IPlıntusRepository Plintuses { get; private set; }
        public IPlıntusDailyRepository PlintusDailies { get; private set; }
        public IPlıntusStockRepository PlintusStocks { get; private set; }
        public IRequestRepository Requests { get; private set; }
        public ISalesRepository Sales { get; private set; }
        public ISalesDetailRepository SalesDetails { get; private set; }
        public ISizeRepository Sizes { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction.CommitAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (_transaction != null)
                    {
                        _transaction.Dispose();
                        _transaction = null;
                    }
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}