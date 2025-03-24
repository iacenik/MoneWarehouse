using BusinessLayer.Services;
using DataAccessLayer;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class FormulaService : IFormulaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FormulaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Formula>> GetAllFormulasAsync()
        {
            return await _unitOfWork.Formulas.GetAllAsync();
        }

        public async Task<Formula> GetFormulaByIdAsync(int id)
        {
            var formula = await _unitOfWork.Formulas.GetByIdAsync(id);
            if (formula == null)
                throw new Exception("Formül bulunamadı.");

            return formula;
        }

        public async Task<Formula> GetFormulaByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Formül adı boş olamaz.");

            var formula = await _unitOfWork.Formulas.GetFormulaByNameAsync(name);
            if (formula == null)
                throw new Exception("Formül bulunamadı.");

            return formula;
        }

        public async Task CreateFormulaAsync(Formula formula)
        {
            if (string.IsNullOrEmpty(formula.FormulaName))
                throw new ArgumentException("Formül adı boş olamaz.");

            if (string.IsNullOrEmpty(formula.FormulaContent))
                throw new ArgumentException("Formül içeriği boş olamaz.");

            var existingFormula = await _unitOfWork.Formulas.GetFormulaByNameAsync(formula.FormulaName);
            if (existingFormula != null)
                throw new InvalidOperationException("Bu isimde bir formül zaten mevcut.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Formulas.AddAsync(formula);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Formül oluşturulurken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task UpdateFormulaAsync(Formula formula)
        {
            var existingFormula = await _unitOfWork.Formulas.GetByIdAsync(formula.FormulaId);
            if (existingFormula == null)
                throw new Exception("Formül bulunamadı.");

            if (string.IsNullOrEmpty(formula.FormulaName))
                throw new ArgumentException("Formül adı boş olamaz.");

            if (string.IsNullOrEmpty(formula.FormulaContent))
                throw new ArgumentException("Formül içeriği boş olamaz.");

            if (existingFormula.FormulaName != formula.FormulaName)
            {
                var nameExists = await _unitOfWork.Formulas.GetFormulaByNameAsync(formula.FormulaName);
                if (nameExists != null)
                    throw new InvalidOperationException("Bu isimde bir formül zaten mevcut.");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                existingFormula.FormulaName = formula.FormulaName;
                existingFormula.FormulaContent = formula.FormulaContent;
                await _unitOfWork.Formulas.UpdateAsync(existingFormula);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Formül güncellenirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task DeleteFormulaAsync(int id)
        {
            var formula = await _unitOfWork.Formulas.GetByIdAsync(id);
            if (formula == null)
                throw new Exception("Formül bulunamadı.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await _unitOfWork.Formulas.RemoveAsync(formula);
                await _unitOfWork.CompleteAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Formül silinirken bir hata oluştu: " + ex.Message);
            }
        }
    }
}
