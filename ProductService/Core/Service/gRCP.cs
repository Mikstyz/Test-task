using Serilog;
using Repositories.Product;
using Entities.Product;

namespace gRCP
{
    public class grspManager
    {
        public static async Task<bool> CheckProductCount(int ProductId, int Count)
        {
            try
            {
                Log.Information("Получение информации о налии товара");

                bool Quantitustatus = await Productdb.Availability(ProductId, Count);

                if (!Quantitustatus)
                {
                    Log.Information("Товара нет в наличии");
                    return false;
                }

                return true;
            }

            catch (Exception ex) 
            {
                Log.Fatal("Ошибка при получении наличия товара");
                return false;
            }
        }
        
        public static async Task<bool> CreateOrderInProduct(List<productsIds> products)
        {
            try
            {
                bool status = await Productdb.UpdateProductQuantities(products);

                if (!status)
                {
                    Log.Information("Кол-во товара не достаточно для оформления заказ");
                    return false;
                }

                Log.Information($"Успешное уменьшение кол-во товара");
                return true;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при изменении кол-во товара\n{ex}");
                return false;
            }
        }
    }
}