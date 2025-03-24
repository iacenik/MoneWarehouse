using DataAccessLayer;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Implementations
{
    public class SalesDetailService : ISalesDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInjectionStockService _injectionStockService;
        private readonly IPlıntusStockService _plintusStockService;

        public SalesDetailService(
            IUnitOfWork unitOfWork,
            IInjectionStockService injectionStockService,
            IPlıntusStockService plintusStockService)
        {
            _unitOfWork = unitOfWork;
            _injectionStockService = injectionStockService;
            _plintusStockService = plintusStockService;
        }

        public async Task<IEnumerable<SalesDetail>> GetAllDetailsAsync()
        {
            return await _unitOfWork.SalesDetails.GetAllAsync();
        }

        public async Task<SalesDetail> GetDetailByIdAsync(int id)
        {
            var detail = await _unitOfWork.SalesDetails.GetByIdAsync(id);
            if (detail == null)
                throw new Exception("Satış detayı bulunamadı.");

            return detail;
        }

        public async Task<IEnumerable<SalesDetail>> GetDetailsBySalesIdAsync(int salesId)
        {
            // Satış varlığını kontrol et
            var sale = await _unitOfWork.Sales.GetByIdAsync(salesId);
            if (sale == null)
                throw new Exception("Belirtilen satış bulunamadı.");

            return await _unitOfWork.SalesDetails.GetDetailsBySalesIdAsync(salesId);
        }

        public async Task<IEnumerable<SalesDetail>> GetDetailsByProductTypeAsync(string productType)
        {
            if (string.IsNullOrEmpty(productType))
                throw new ArgumentException("Ürün tipi boş olamaz.");

            return await _unitOfWork.SalesDetails.GetDetailsByProductTypeAsync(productType);
        }

        public async Task<IEnumerable<SalesDetail>> GetDetailsByStockIdAsync(int stockId, string productType)
        {
            if (string.IsNullOrEmpty(productType))
                throw new ArgumentException("Ürün tipi boş olamaz.");

            // Stok varlığını kontrol et
            if (productType == "Plıntus")
            {
                var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(stockId);
                if (stock == null)
                    throw new Exception("Belirtilen plıntus stoğu bulunamadı.");
            }
            else if (productType == "Injection")
            {
                var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(stockId);
                if (stock == null)
                    throw new Exception("Belirtilen enjeksiyon stoğu bulunamadı.");
            }
            else
            {
                throw new ArgumentException("Geçersiz ürün tipi. 'Plıntus' veya 'Injection' olmalıdır.");
            }

            return await _unitOfWork.SalesDetails.GetDetailsByStockIdAsync(stockId, productType);
        }

        public async Task CreateDetailAsync(SalesDetail detail)
        {
            // Satış varlığını kontrol et
            var sale = await _unitOfWork.Sales.GetByIdAsync(detail.SalesId);
            if (sale == null)
                throw new Exception("Belirtilen satış bulunamadı.");

            if (detail.Quantity <= 0)
                throw new ArgumentException("Satış miktarı 0'dan büyük olmalıdır.");

            if (detail.UnitPrice <= 0)
                throw new ArgumentException("Birim fiyat 0'dan büyük olmalıdır.");

            // Toplam fiyatı hesapla
            detail.TotalPrice = detail.Quantity * detail.UnitPrice;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ürün tipine göre stok kontrolü ve stok düşümü
                if (detail.ProductType == "Plıntus" && detail.PlıntusStockId.HasValue)
                {
                    // Stok varlığını ve yeterliliğini kontrol et
                    var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(detail.PlıntusStockId.Value);
                    if (stock == null)
                        throw new Exception($"Belirtilen plıntus stoğu bulunamadı: {detail.PlıntusStockId}");

                    if (stock.Quantity < detail.Quantity)
                        throw new InvalidOperationException($"Plıntus stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen: {detail.Quantity}");

                    // Satış detayını ekle
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

                    // Satış detayını ekle
                    await _unitOfWork.SalesDetails.AddAsync(detail);
                    await _unitOfWork.CompleteAsync();

                    // Stok miktarını azalt
                    await _injectionStockService.DecreaseStockQuantityAsync(detail.InjectionStockId.Value, detail.Quantity);
                }
                else
                {
                    throw new InvalidOperationException($"Geçersiz ürün tipi veya ürün ID'si. Ürün tipi: {detail.ProductType}");
                }

                // Satışın toplam tutarını güncelle
                sale.TotalAmount += detail.TotalPrice;
                await _unitOfWork.Sales.UpdateAsync(sale);
                await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Satış detayı eklenirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task UpdateDetailAsync(SalesDetail detail, int oldQuantity, int? oldProductId, string oldProductType)
        {
            var existingDetail = await _unitOfWork.SalesDetails.GetByIdAsync(detail.Id);
            if (existingDetail == null)
                throw new Exception("Satış detayı bulunamadı.");

            if (detail.Quantity <= 0)
                throw new ArgumentException("Satış miktarı 0'dan büyük olmalıdır.");

            if (detail.UnitPrice <= 0)
                throw new ArgumentException("Birim fiyat 0'dan büyük olmalıdır.");

            // Toplam fiyatı hesapla
            detail.TotalPrice = detail.Quantity * detail.UnitPrice;

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Ürün tipi veya ID değişirse stokları uygun şekilde güncelle
                if (oldProductType != detail.ProductType ||
                    (oldProductType == "Plıntus" && oldProductId != detail.PlıntusStockId) ||
                    (oldProductType == "Injection" && oldProductId != detail.InjectionStockId))
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
                    if (detail.ProductType == "Plıntus" && detail.PlıntusStockId.HasValue)
                    {
                        // Stok varlığını ve yeterliliğini kontrol et
                        var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(detail.PlıntusStockId.Value);
                        if (stock == null)
                            throw new Exception($"Belirtilen plıntus stoğu bulunamadı: {detail.PlıntusStockId}");

                        if (stock.Quantity < detail.Quantity)
                            throw new InvalidOperationException($"Plıntus stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen: {detail.Quantity}");

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

                        await _injectionStockService.DecreaseStockQuantityAsync(detail.InjectionStockId.Value, detail.Quantity);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Geçersiz ürün tipi veya ürün ID'si. Ürün tipi: {detail.ProductType}");
                    }
                }
                // Sadece miktar değişmişse, farkı hesapla ve stoku güncelle
                else if (oldQuantity != detail.Quantity)
                {
                    int quantityDifference = detail.Quantity - oldQuantity;

                    // Eğer fark pozitifse, stoktan ilave miktarı düş
                    // Eğer fark negatifse, stoka iade edilen miktarı ekle
                    if (detail.ProductType == "Plıntus" && detail.PlıntusStockId.HasValue)
                    {
                        if (quantityDifference > 0)
                        {
                            // Stok yeterliliğini kontrol et
                            var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(detail.PlıntusStockId.Value);
                            if (stock.Quantity < quantityDifference)
                                throw new InvalidOperationException($"Plıntus stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen artış: {quantityDifference}");

                            await _plintusStockService.DecreaseStockQuantityAsync(detail.PlıntusStockId.Value, quantityDifference);
                        }
                        else if (quantityDifference < 0)
                        {
                            await _plintusStockService.IncreaseStockQuantityAsync(detail.PlıntusStockId.Value, Math.Abs(quantityDifference));
                        }
                    }
                    else if (detail.ProductType == "Injection" && detail.InjectionStockId.HasValue)
                    {
                        if (quantityDifference > 0)
                        {
                            // Stok yeterliliğini kontrol et
                            var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(detail.InjectionStockId.Value);
                            if (stock.Quantity < quantityDifference)
                                throw new InvalidOperationException($"Enjeksiyon stok miktarı yetersiz. Mevcut stok: {stock.Quantity}, Talep edilen artış: {quantityDifference}");

                            await _injectionStockService.DecreaseStockQuantityAsync(detail.InjectionStockId.Value, quantityDifference);
                        }
                        else if (quantityDifference < 0)
                        {
                            await _injectionStockService.IncreaseStockQuantityAsync(detail.InjectionStockId.Value, Math.Abs(quantityDifference));
                        }
                    }
                }

                // Satışın toplam tutarını güncelle
                var sale = await _unitOfWork.Sales.GetByIdAsync(existingDetail.SalesId);
                sale.TotalAmount = sale.TotalAmount - existingDetail.TotalPrice + detail.TotalPrice;

                // Detay bilgilerini güncelle
                existingDetail.ProductType = detail.ProductType;
                existingDetail.PlıntusStockId = detail.PlıntusStockId;
                existingDetail.InjectionStockId = detail.InjectionStockId;
                existingDetail.Quantity = detail.Quantity;
                existingDetail.UnitPrice = detail.UnitPrice;
                existingDetail.TotalPrice = detail.TotalPrice;

                await _unitOfWork.SalesDetails.UpdateAsync(existingDetail);
                await _unitOfWork.Sales.UpdateAsync(sale);
                await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Satış detayı güncellenirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task DeleteDetailAsync(int id)
        {
            var detail = await _unitOfWork.SalesDetails.GetByIdAsync(id);
            if (detail == null)
                throw new Exception("Satış detayı bulunamadı.");

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Satış detayını sil
                await _unitOfWork.SalesDetails.RemoveAsync(detail);
                await _unitOfWork.CompleteAsync();

                // Stok miktarını artır (satış iptali nedeniyle)
                if (detail.ProductType == "Plıntus" && detail.PlıntusStockId.HasValue)
                {
                    await _plintusStockService.IncreaseStockQuantityAsync(detail.PlıntusStockId.Value, detail.Quantity);
                }
                else if (detail.ProductType == "Injection" && detail.InjectionStockId.HasValue)
                {
                    await _injectionStockService.IncreaseStockQuantityAsync(detail.InjectionStockId.Value, detail.Quantity);
                }

                // Satışın toplam tutarını güncelle
                var sale = await _unitOfWork.Sales.GetByIdAsync(detail.SalesId);
                sale.TotalAmount -= detail.TotalPrice;
                await _unitOfWork.Sales.UpdateAsync(sale);
                await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Satış detayı silinirken bir hata oluştu: " + ex.Message);
            }
        }

        public async Task<Dictionary<string, int>> GetProductSalesCountAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            // Belirtilen tarih aralığındaki satışları getir
            var sales = await _unitOfWork.Sales.GetSalesByDateRangeAsync(startDate, endDate);

            // Satışların ID'lerini al
            var salesIds = sales.Select(s => s.Id).ToList();

            // Ürün bazlı satış miktarlarını hesapla
            var result = new Dictionary<string, int>();

            foreach (var salesId in salesIds)
            {
                var details = await _unitOfWork.SalesDetails.GetDetailsBySalesIdAsync(salesId);

                foreach (var detail in details)
                {
                    string productName = string.Empty;

                    if (detail.ProductType == "Plıntus" && detail.PlıntusStockId.HasValue)
                    {
                        var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(detail.PlıntusStockId.Value);
                        productName = $"Plıntus - {stock.Name}";
                    }
                    else if (detail.ProductType == "Injection" && detail.InjectionStockId.HasValue)
                    {
                        var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(detail.InjectionStockId.Value);
                        productName = $"Injection - {stock.Name}";
                    }

                    if (!string.IsNullOrEmpty(productName))
                    {
                        if (result.ContainsKey(productName))
                            result[productName] += detail.Quantity;
                        else
                            result[productName] = detail.Quantity;
                    }
                }
            }

            return result;
        }

        public async Task<Dictionary<string, decimal>> GetProductSalesAmountAsync(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Başlangıç tarihi, bitiş tarihinden sonra olamaz.");

            // Belirtilen tarih aralığındaki satışları getir
            var sales = await _unitOfWork.Sales.GetSalesByDateRangeAsync(startDate, endDate);

            // Satışların ID'lerini al
            var salesIds = sales.Select(s => s.Id).ToList();

            // Ürün bazlı satış tutarlarını hesapla
            var result = new Dictionary<string, decimal>();

            foreach (var salesId in salesIds)
            {
                var details = await _unitOfWork.SalesDetails.GetDetailsBySalesIdAsync(salesId);

                foreach (var detail in details)
                {
                    string productName = string.Empty;

                    if (detail.ProductType == "Plıntus" && detail.PlıntusStockId.HasValue)
                    {
                        var stock = await _unitOfWork.PlintusStocks.GetByIdAsync(detail.PlıntusStockId.Value);
                        productName = $"Plıntus - {stock.Name}";
                    }
                    else if (detail.ProductType == "Injection" && detail.InjectionStockId.HasValue)
                    {
                        var stock = await _unitOfWork.InjectionStocks.GetByIdAsync(detail.InjectionStockId.Value);
                        productName = $"Injection - {stock.Name}";
                    }

                    if (!string.IsNullOrEmpty(productName))
                    {
                        if (result.ContainsKey(productName))
                            result[productName] += detail.TotalPrice;
                        else
                            result[productName] = detail.TotalPrice;
                    }
                }
            }

            return result;
        }
    }
}
