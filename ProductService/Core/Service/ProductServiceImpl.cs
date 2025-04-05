using Entities.Product;
using Repositories.Product;
using Serilog;

namespace Service
{
    public class ProductManager
    {
        public async Task<IEnumerable<product>> GetProducts(int offset, int limit = 20)
        {
            try
            {
                Log.Information("Попытка получить список продуктов с offset {Offset} и limit {Limit}", offset, limit);

                var products = await Productdb.getOffsset(offset, limit);

                if (products == null)
                {
                    Log.Error($"Ошибка при получении списка продуктов с offset {offset}");
                    return null;
                }

                Log.Information($"Успешно получено {products.Count()} продуктов");
                return products;
            }

            catch (Exception ex)
            {
                Log.Fatal("Ошибка при получении списка продуктов\n{ex}");
                return null;
            }
        }

        public async Task<product> GetProductById(int id)
        {
            try
            {
                Log.Information($"Попытка получить продукт по id {id}");

                var product = await Productdb.getId(id);

                if (product == null)
                {
                    Log.Error($"Продукт с id {id} не найден или ошибка при запросе", id);
                    return null;
                }

                Log.Information($"Продукт с id {id} успешно получен");
                return product;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при получении продукта по id {id}\n{ex}");
                return null;
            }
        }

        public async Task<IEnumerable<product>> SearchProducts(int offset, int limit = 20, string seller = null, string name = null, double? price = null, int? quantity = null)
        {
            try
            {
                Log.Information($"Попытка поиска продуктов с параметрами: offset={offset}, limit={limit}, seller={seller}, name={name}, price={price}, quantity={quantity}");

                var products = await Productdb.search(offset, limit, seller, name, price, quantity);

                if (products == null)
                {
                    Log.Error("Ошибка при поиске продуктов");
                    return null;
                }

                Log.Information($"Успешно найдено {products.Count()} продуктов");
                return products;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при поиске продуктов\n{ex}");
                return null;
            }
        }

        public async Task<int> CreateProduct(product newProduct)
        {
            try
            {
                Log.Information($"Попытка создать продукт: {newProduct.Name}");

                int productId = await Productdb.create(newProduct.Seller, newProduct.Name, newProduct.Description, newProduct.Price, newProduct.Quantity);

                if (productId <= 0)
                {
                    Log.Error($"Ошибка при создании продукта: {newProduct.Name}");
                    return -1;
                }

                Log.Information($"Продукт {newProduct.Name} успешно создан с id {productId}");
                return productId;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при создании продукта\n{ex}");
                return -1;
            }
        }

        public async Task<bool> UpdateProduct(updateProduct updatedProduct)
        {
            try
            {
                Log.Information("Попытка обновить продукт с id {Id}", updatedProduct.id);

                bool isUpdated = await Productdb.Update(updatedProduct.id, updatedProduct.NewName, updatedProduct.NewDescription, updatedProduct.NewPrice, updatedProduct.NewQuantity);

                if (!isUpdated)
                {
                    Log.Error($"Ошибка при обновлении продукта с id {updatedProduct.id}");
                    return false;
                }

                Log.Information($"Продукт с id {updatedProduct.id} успешно обновлен");
                return true;
            }
            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при обновлении продукта с id {updatedProduct.id}\n{ex}");
                return false;
            }
        }

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                Log.Information($"Попытка удалить продукт с id {id}");

                bool isDeleted = await Productdb.Delete(id);

                if (!isDeleted)
                {
                    Log.Error($"Ошибка при удалении продукта с id {id}");
                    return false;
                }

                Log.Information($"Продукт с id {id} успешно удален");
                return true;
            }

            catch (Exception ex)
            {
                Log.Fatal($"Ошибка при удалении продукта с id {id}\n{ex}");
                return false;
            }
        }
    }
}