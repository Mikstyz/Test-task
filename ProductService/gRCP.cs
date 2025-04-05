using Grpc.Core;
using ProductService;

public class ProductServiceImpl : ProductService.ProductService.ProductServiceBase
{
    // Реализация метода для проверки наличия товара
    public override Task<CheckAvailabilityResponse> CheckAvailability(CheckAvailabilityRequest request, ServerCallContext context)
    {
        // Пример логики проверки наличия товара
        bool isAvailable = false;
        int quantity = 0;

        // Пример: если товар с ID "123" в наличии
        if (request.ProductId == "123")
        {
            isAvailable = true;
            quantity = 50;  // Количество товара в наличии
        }

        var response = new CheckAvailabilityResponse
        {
            Available = isAvailable,
            Quantity = quantity
        };

        return Task.FromResult(response);
    }
}
