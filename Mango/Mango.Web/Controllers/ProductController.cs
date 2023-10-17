using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService ProductService)
        {
            _productService = ProductService;
        }

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDTO> list = new();

            ResponseDTO? response = await _productService.GetAllProductsAsync();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }

            return View(list);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDTO model)
        {
            //check valid in server-side >< check valid in client-side
            if (ModelState.IsValid)
            {
                ResponseDTO? response = await _productService.CreateProductsAsync(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["Success"] = "Product create successfully!";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["Error"] = response?.Message;
                }

            }
            return View(model);
        }

        public async Task<IActionResult> ProductDelete(int ProductId)
        {
            ResponseDTO? response = await _productService.GetProductByIdAsync(ProductId);

            if (response != null && response.IsSuccess)
            {
                ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["Error"] = response?.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDTO ProductDto)
        {
            ResponseDTO? response = await _productService.DeleteProductsAsync(ProductDto.ProductId);

            if (response != null && response.IsSuccess)
            {
                TempData["Success"] = "Product delete successfully!";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["Error"] = response?.Message;
            }
            return View(ProductDto);
        }

		public async Task<IActionResult> ProductEdit(int ProductId)
		{
			ResponseDTO? response = await _productService.GetProductByIdAsync(ProductId);

			if (response != null && response.IsSuccess)
			{
				ProductDTO? model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
				return View(model);
			}
			else
			{
				TempData["Error"] = response?.Message;
			}

			return NotFound();
		}

		[HttpPost]
		public async Task<IActionResult> ProductEdit(ProductDTO ProductDto)
		{
			ResponseDTO? response = await _productService.UpdateProductsAsync(ProductDto);

			if (response != null && response.IsSuccess)
			{
				TempData["Success"] = "Product updated successfully!";
				return RedirectToAction(nameof(ProductIndex));
			}
			else
			{
				TempData["Error"] = response?.Message;
			}
			return View(ProductDto);
		}
	}
}
