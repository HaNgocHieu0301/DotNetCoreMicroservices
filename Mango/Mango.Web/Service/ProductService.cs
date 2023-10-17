using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService) { 
            _baseService = baseService;
        }
        public async Task<ResponseDTO> CreateProductsAsync(ProductDTO ProductDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = ProductDTO,
                Url = SD.ProductAPIBase + "/api/Product"
            });
        }

        public async Task<ResponseDTO> DeleteProductsAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProductAPIBase + "/api/Product/" + id
            });
        }

        public async Task<ResponseDTO> GetAllProductsAsync()
        {
             return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/Product"
            });
        }

        public async Task<ResponseDTO> GetProductAsync(string ProductCode)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/Product/GetByCode/" + ProductCode
            });
        }

        public async Task<ResponseDTO> GetProductByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ProductAPIBase + "/api/Product/"+id
            });
        }

        public async Task<ResponseDTO> UpdateProductsAsync(ProductDTO ProductDTO)
        {
            return await _baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.PUT,
                Data = ProductDTO,
                Url = SD.ProductAPIBase + "/api/product"
			});
        }
    }
}
