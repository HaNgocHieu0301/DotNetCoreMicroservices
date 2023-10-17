using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
        Task<ResponseDTO> GetProductAsync(string ProductCode);
        Task<ResponseDTO> GetAllProductsAsync();
        Task<ResponseDTO> GetProductByIdAsync(int id);
        Task<ResponseDTO> CreateProductsAsync(ProductDTO ProductDTO);
        Task<ResponseDTO> UpdateProductsAsync(ProductDTO ProductDTO);
        Task<ResponseDTO> DeleteProductsAsync(int id);
    }
}
