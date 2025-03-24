using DataAccessLayer;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class SalesService : ISalesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInjectionStockService _injectionStockService;
        private readonly IPlıntusStockService _plintusStockService;

        public SalesService(
            IUnitOfWork unitOfWork,
            IInjectionStockService injectionStockService,
            IPlıntusStockService plintusStockService)
        {
            _unitOfWork = unitOfWork;
            _injectionStockService = injectionStockService;
            _plintusStockService = plintusStockService;
        }

        public async Task<IEnumerable<Sales>> GetAllSalesAsync()
        {
            return await _unitOfWork.Sales.GetAllAsync();
        }

        public async Task<Sales> GetSaleByIdAsync(int id)
        {
            var sale = await _unitOfWork.Sales.GetByIdAsync(id);
            if (sale == null)
                throw new Exception("Satış kaydı bulunamadı.");

            return sale;
        }

        public async Task<IEnumerable<Sales>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            return await _unitOfWork.Sales.GetSalesByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<Sales>> GetSalesByClientAsync(int clientId)
        {
            // Müşteri varlığını kontrol et
            var client = await _unitOfWork.Clients.GetByIdAsync(clientId);
            if (client == null)
                throw new Exception("Belirtilen müşteri bulunamadı.");

            return await _unitOfWork.Sales.GetSalesByClientAsync(clientId);
        }

        public async Task<IEnumerable<Sales>> GetSalesByEmployeeAsync(int employeeId)
        {
            // Çalışan varlığını kontrol et
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
            if (employee == null)
                throw new Exception("Belirtilen çalışan bulunamadı.");

            return await _unitOfWork.Sales.GetSalesByEmployeeAsync(employeeId);
        }

        public async Task<IEnumerable<Sales>> GetSalesByStatusAsync(string status)
        {
            if (string.IsNullOrEmpty(status))
                throw new ArgumentException("Durum bilgisi boş olamaz.");

            return await _unitOfWork.Sales.GetSalesByStatusAsync(status);
        }

        public async Task<Sales> GetSaleWithDetailsAsync(int id)
        {
            var sale = await _unitOfWork.Sales.GetSaleWithDetailsAsync(id);
            if (sale == null)
                throw new Exception("Satış kaydı bulunamadı.");

            return sale;
        }

        public async Task CreateSaleAsync(Sales sale)
        {
            // Müşteri ve çalışan varlığını kontrol et
            var client = await _unitOfWork.Clients.GetByIdAsync(sale.ClientId);
            if (client == null)
                throw new Exception("Belirtilen müşteri bulunamadı.");

            var employee = await _unitOfWork.Employees.GetByIdAsync(sale.EmployeeId);
            if (employee == null)
                throw new Exception("Belirtilen çalışan bulunamadı.");

            if (!employee.IsActive)
                throw new InvalidOperationException("Bu çalışan aktif değil. Satış kaydı eklenemez.");

            // Satış detaylarının varlığını kontrol et
            if (sale.SalesDetails == null || !sale.SalesDetails.Any())
                throw new ArgumentException("Satış detayları boş olamaz.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Satış ana kaydını ekle
                sale.SalesDate = DateTime.Now;
                sale.Status = "Completed"; // Default olarak tamamlanmış durumda
                await _unitOfWork.Sales.AddAsync(sale);
                await _unitOfWork.CompleteAsync();

                // Her bir satış detayı için stok miktarını kontrol et ve düş
                foreach (var detail in sale.SalesDetails)
                {
                    detail.SalesId = sale.Id; // Ana satış ID'sini atama
                    detail.TotalPrice = detail.Quantity * detail.UnitPrice; // Toplam fiyatı hesapla

                    // Ürün tipine göre stok kontrolü ve stok düşümü
                    if (detail.ProductType == "Plıntus" && detail.PlıntusStockId.HasValue)
                    {
                        // Stok varlığını ve yeterliliğini kontrol et
                        var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(detail.PlıntusStockId.Value);
                        if (stock == null)
                            throw new Exception($"Belirtilen plıntus stoğu bulunamadı: {detail.PlıntusStockId}");

                        if (stock.Quantity < detail.Quantity)
                            throw new InvalidOperationException($"Plıntus stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen: {detail.Quantity}");

                        // Stok düşümü Sales Detail Service tarafından yapılacak
                        await _unitOfWork.SalesDetails.AddAsync(detail);
                        await _unitOfWork.CompleteAsync();

                        // Stok miktarını azalt
                        await _plintusStockService.DecreaseStockQuantityAsync(detail.PlıntusStockId.Value, detail.Quantity);
                    }
                    else if (detail.ProductType == "Injection" && detail.InjectionStockId.HasValue)
                    {
                        // Stok varlığını ve yeterliliğini kontrol et
                        var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(detail.InjectionStockId.Value);
                        if (stock == null)
                            throw new Exception($"Belirtilen enjeksiyon stoğu bulunamadı: {detail.InjectionStockId}");

                        if (stock.Quantity < detail.Quantity)
                            throw new InvalidOperationException($"Enjeksiyon stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen: {detail.Quantity}");

                        // Stok düşümü Sales Detail Service tarafından yapılacak
                        await _unitOfWork.SalesDetails.AddAsync(detail);
                        await _unitOfWork.CompleteAsync();

                        // Stok miktarını azalt
                        await _injectionStockService.DecreaseStockQuantityAsync(detail.InjectionStockId.Value, detail.Quantity);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Geçersiz ürün tipi veya ürün ID'si. Ürün tipi: {detail.ProductType}");
                    }
                }

                // Toplam tutarı güncelle
                sale.TotalAmount = sale.SalesDetails.Sum(d => d.TotalPrice);
                await _unitOfWork.Sales.UpdateAsync(sale);

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Satış kaydı oluşturulurken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task UpdateSaleAsync(Sales sale)
        {
            var existingSale = await _unitOfWork.Sales.GetSaleWithDetailsAsync(sale.Id);
            if (existingSale == null)
                throw new Exception("Satış kaydı bulunamadı.");

            // Müşteri ve çalışan varlığını kontrol et
            var client = await _unitOfWork.Clients.GetByIdAsync(sale.ClientId);
            if (client == null)
                throw new Exception("Belirtilen müşteri bulunamadı.");

            var employee = await _unitOfWork.Employees.GetByIdAsync(sale.EmployeeId);
            if (employee == null)
                throw new Exception("Belirtilen çalışan bulunamadı.");

            // Mevcut detayları geçici olarak sakla
            var existingDetails = existingSale.SalesDetails.ToList();

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ana satış bilgilerini güncelle
                existingSale.ClientId = sale.ClientId;
                existingSale.EmployeeId = sale.EmployeeId;
                existingSale.SalesDate = sale.SalesDate;
                existingSale.Status = sale.Status;
                existingSale.Notes = sale.Notes;

                await _unitOfWork.Sales.UpdateAsync(existingSale);
                await _unitOfWork.CompleteAsync();

                // Detayları güncelle: Silinen, güncellenen ve yeni eklenen detayları işle

                // 1. Silinen detayları bul ve stokları geri ekle
                foreach (var existingDetail in existingDetails)
                {
                    if (!sale.SalesDetails.Any(d => d.Id == existingDetail.Id && d.Id != 0))
                    {
                        // Silinen detay için stok miktarını geri ekle
                        if (existingDetail.ProductType == "Plıntus" && existingDetail.PlıntusStockId.HasValue)
                        {
                            await _plintusStockService.IncreaseStockQuantityAsync(existingDetail.PlıntusStockId.Value, existingDetail.Quantity);
                        }
                        else if (existingDetail.ProductType == "Injection" && existingDetail.InjectionStockId.HasValue)
                        {
                            await _injectionStockService.IncreaseStockQuantityAsync(existingDetail.InjectionStockId.Value, existingDetail.Quantity);
                        }

                        // Detayı veritabanından sil
                        await _unitOfWork.SalesDetails.RemoveAsync(existingDetail);
                        await _unitOfWork.CompleteAsync();
                    }
                }

                // 2. Güncellenen ve yeni eklenen detayları işle
                foreach (var newDetail in sale.SalesDetails)
                {
                    newDetail.TotalPrice = newDetail.Quantity * newDetail.UnitPrice; // Toplam fiyatı hesapla

                    if (newDetail.Id != 0) // Mevcut detay (güncelleniyor)
                    {
                        var existingDetail = existingDetails.FirstOrDefault(d => d.Id == newDetail.Id);
                        if (existingDetail != null)
                        {
                            // Eski ve yeni detay bilgilerini karşılaştır
                            int oldQuantity = existingDetail.Quantity;
                            int? oldProductId = null;
                            string oldProductType = existingDetail.ProductType;

                            if (existingDetail.ProductType == "Plıntus")
                                oldProductId = existingDetail.PlıntusStockId;
                            else if (existingDetail.ProductType == "Injection")
                                oldProductId = existingDetail.InjectionStockId;

                            // Ürün tipi veya ID değişirse stokları uygun şekilde güncelle
                            if (oldProductType != newDetail.ProductType ||
                                (oldProductType == "Plıntus" && oldProductId != newDetail.PlıntusStockId) ||
                                (oldProductType == "Injection" && oldProductId != newDetail.InjectionStockId))
                            {
                                // Eski ürün stoğunu geri ekle
                                if (oldProductType == "Plıntus" && oldProductId.HasValue)
                                {
                                    await _plintusStockService.IncreaseStockQuantityAsync(oldProductId.Value, oldQuantity);
                                }
                                else if (oldProductType == "Injection" && oldProductId.HasValue)
                                {
                                    await _injectionStockService.IncreaseStockQuantityAsync(oldProductId.Value, oldQuantity);
                                }

                                // Yeni ürün stoğundan düş
                                if (newDetail.ProductType == "Plıntus" && newDetail.PlıntusStockId.HasValue)
                                {
                                    // Stok varlığını ve yeterliliğini kontrol et
                                    var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(newDetail.PlıntusStockId.Value);
                                    if (stock == null)
                                        throw new Exception($"Belirtilen plıntus stoğu bulunamadı: {newDetail.PlıntusStockId}");

                                    if (stock.Quantity < newDetail.Quantity)
                                        throw new InvalidOperationException($"Plıntus stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen: {newDetail.Quantity}");

                                    await _plintusStockService.DecreaseStockQuantityAsync(newDetail.PlıntusStockId.Value, newDetail.Quantity);
                                }
                                else if (newDetail.ProductType == "Injection" && newDetail.InjectionStockId.HasValue)
                                {
                                    // Stok varlığını ve yeterliliğini kontrol et
                                    var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(newDetail.InjectionStockId.Value);
                                    if (stock == null)
                                        throw new Exception($"Belirtilen enjeksiyon stoğu bulunamadı: {newDetail.InjectionStockId}");

                                    if (stock.Quantity < newDetail.Quantity)
                                        throw new InvalidOperationException($"Enjeksiyon stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen: {newDetail.Quantity}");

                                    await _injectionStockService.DecreaseStockQuantityAsync(newDetail.InjectionStockId.Value, newDetail.Quantity);
                                }
                                else
                                {
                                    throw new InvalidOperationException($"Geçersiz ürün tipi veya ürün ID'si. Ürün tipi: {newDetail.ProductType}");
                                }
                            }
                            // Sadece miktar değişmişse, farkı hesapla ve stoku güncelle
                            else if (oldQuantity != newDetail.Quantity)
                            {
                                int quantityDifference = newDetail.Quantity - oldQuantity;

                                // Eğer fark pozitifse, stoktan ilave miktarı düş
                                // Eğer fark negatifse, stoka iade edilen miktarı ekle
                                if (newDetail.ProductType == "Plıntus" && newDetail.PlıntusStockId.HasValue)
                                {
                                    if (quantityDifference > 0)
                                    {
                                        // Stok yeterliliğini kontrol et
                                        var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(newDetail.PlıntusStockId.Value);
                                        if (stock.Quantity < quantityDifference)
                                            throw new InvalidOperationException($"Plıntus stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen artış: {quantityDifference}");

                                        await _plintusStockService.DecreaseStockQuantityAsync(newDetail.PlıntusStockId.Value, quantityDifference);
                                    }
                                    else if (quantityDifference < 0)
                                    {
                                        await _plintusStockService.IncreaseStockQuantityAsync(newDetail.PlıntusStockId.Value, Math.Abs(quantityDifference));
                                    }
                                }
                                else if (newDetail.ProductType == "Injection" && newDetail.InjectionStockId.HasValue)
                                {
                                    if (quantityDifference > 0)
                                    {
                                        // Stok yeterliliğini kontrol et
                                        var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(newDetail.InjectionStockId.Value);
                                        if (stock.Quantity < quantityDifference)
                                            throw new InvalidOperationException($"Enjeksiyon stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen artış: {quantityDifference}");

                                        await _injectionStockService.DecreaseStockQuantityAsync(newDetail.InjectionStockId.Value, quantityDifference);
                                    }
                                    else if (quantityDifference < 0)
                                    {
                                        await _injectionStockService.IncreaseStockQuantityAsync(newDetail.InjectionStockId.Value, Math.Abs(quantityDifference));
                                    }
                                }
                            }

                            // Detay bilgilerini güncelle
                            existingDetail.ProductType = newDetail.ProductType;
                            existingDetail.PlıntusStockId = newDetail.PlıntusStockId;
                            existingDetail.InjectionStockId = newDetail.InjectionStockId;
                            existingDetail.Quantity = newDetail.Quantity;
                            existingDetail.UnitPrice = newDetail.UnitPrice;
                            existingDetail.TotalPrice = newDetail.TotalPrice;

                            await _unitOfWork.SalesDetails.UpdateAsync(existingDetail);
                            await _unitOfWork.CompleteAsync();
                        }
                    }
                    else // Yeni detay (ekleniyor)
                    {
                        newDetail.SalesId = sale.Id;

                        // Yeni ürün stoğundan düş
                        if (newDetail.ProductType == "Plıntus" && newDetail.PlıntusStockId.HasValue)
                        {
                            // Stok varlığını ve yeterliliğini kontrol et
                            var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(newDetail.PlıntusStockId.Value);
                            if (stock == null)
                                throw new Exception($"Belirtilen plıntus stoğu bulunamadı: {newDetail.PlıntusStockId}");

                            if (stock.Quantity < newDetail.Quantity)
                                throw new InvalidOperationException($"Plıntus stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen: {newDetail.Quantity}");

                            // Stok düşümü Sales Detail Service tarafından yapılacak
                            await _unitOfWork.SalesDetails.AddAsync(newDetail);
                            await _unitOfWork.CompleteAsync();

                            await _plintusStockService.DecreaseStockQuantityAsync(newDetail.PlıntusStockId.Value, newDetail.Quantity);
                        }
                        else if (newDetail.ProductType == "Injection" && newDetail.InjectionStockId.HasValue)
                        {
                            // Stok varlığını ve yeterliliğini kontrol et
                            var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(newDetail.InjectionStockId.Value);
                            if (stock == null)
                                throw new Exception($"Belirtilen enjeksiyon stoğu bulunamadı: {newDetail.InjectionStockId}");

                            if (stock.Quantity < newDetail.Quantity)
                                throw new InvalidOperationException($"Enjeksiyon stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen: {newDetail.Quantity}");

                            // Stok düşümü Sales Detail Service tarafından yapılacak
                            await _unitOfWork.SalesDetails.AddAsync(newDetail);
                            await _unitOfWork.CompleteAsync();

                            await _injectionStockService.DecreaseStockQuantityAsync(newDetail.InjectionStockId.Value, newDetail.Quantity);
                        }
                        else
                        {
                            throw new InvalidOperationException($"Geçersiz ürün tipi veya ürün ID'si. Ürün tipi: {newDetail.ProductType}");
                        }
                    }
                }

                // Toplam tutarı yeniden hesapla ve güncelle
                var updatedSale = await _unitOfWork.Sales.GetSaleWithDetailsAsync(sale.Id);
                updatedSale.TotalAmount = updatedSale.SalesDetails.Sum(d => d.TotalPrice);
                await _unitOfWork.Sales.UpdateAsync(updatedSale);
                await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Satış kaydı güncellenirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task DeleteSaleAsync(int id)
        {
            var sale = await _unitOfWork.Sales.GetSaleWithDetailsAsync(id);
            if (sale == null)
                throw new Exception("Satış kaydı bulunamadı.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Satış detaylarını sil ve stokları geri ekle
                foreach (var detail in sale.SalesDetails.ToList())
                {
                    // Stok miktarını geri ekle
                    if (detail.ProductType == "Plıntus" && detail.PlıntusStockId.HasValue)
                    {
                        await _plintusStockService.IncreaseStockQuantityAsync(detail.PlıntusStockId.Value, detail.Quantity);
                    }
                    else if (detail.ProductType == "Injection" && detail.InjectionStockId.HasValue)
                    {
                        await _injectionStockService.IncreaseStockQuantityAsync(detail.InjectionStockId.Value, detail.Quantity);
                    }

                    // Detayı veritabanından sil
                    await _unitOfWork.SalesDetails.RemoveAsync(detail);
                }
                await _unitOfWork.CompleteAsync();

                // Ana satış kaydını sil
                await _unitOfWork.Sales.RemoveAsync(sale);
                await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Satış kaydı silinirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task<decimal> CalculateTotalSalesAmountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            var sales = await _unitOfWork.Sales.GetSalesByDateRangeAsync(startDate, endDate);
            return sales.Sum(s => s.TotalAmount);
        }

        public async Task<Dictionary<string, decimal>> GetMonthlySalesReportAsync(int year)
        {
            if (year < 2000 || year > DateTime.Now.Year)
                throw new ArgumentException("Geçersiz yıl değeri.");

            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);
            var sales = await _unitOfWork.Sales.GetSalesByDateRangeAsync(startDate, endDate);

            return sales
                .GroupBy(s => s.SalesDate.ToString("MMMM"))
                .ToDictionary(g => g.Key, g => g.Sum(s => s.TotalAmount));
        }

        public async Task<Dictionary<int, decimal>> GetSalesByClientReportAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            var sales = await _unitOfWork.Sales.GetSalesByDateRangeAsync(startDate, endDate);

            return sales
                .GroupBy(s => s.ClientId)
                .ToDictionary(g => g.Key, g => g.Sum(s => s.TotalAmount));
        }

        public async Task<Dictionary<int, decimal>> GetSalesByEmployeeReportAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            var sales = await _unitOfWork.Sales.GetSalesByDateRangeAsync(startDate, endDate);

            return sales
                .GroupBy(s => s.EmployeeId)
                .ToDictionary(g => g.Key, g => g.Sum(s => s.TotalAmount));
        }
    }
}