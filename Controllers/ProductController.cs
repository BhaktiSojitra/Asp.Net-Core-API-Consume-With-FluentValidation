using API_Consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace API_Consume.Controllers
{
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new System.Uri(_configuration["WebApiBaseUrl"])
            };
        }

        #region GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<ProductModel> products = new List<ProductModel>();
            HttpResponseMessage response = _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/Product/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                var dataOfObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject
                    (dataOfObject, Formatting.Indented);
                products = JsonConvert.DeserializeObject<List<ProductModel>>(extractedDataJson);
            }
            return View("ProductList", products);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int ProductID)
        {
            HttpResponseMessage response = _httpClient.DeleteAsync
                ($"{_httpClient.BaseAddress}/Product/DeleteProduct/{ProductID}").Result;
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "A foreign key constraint error occurred. Please try again.";
            }
            return RedirectToAction("GetAll");
        }
        #endregion

        #region UserDropDown
        public async Task UserDropDown()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Product/user");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                var userData = jsonObject.data; // Access the "data" property
                var users = JsonConvert.DeserializeObject<List<UserDropDownModel>>(userData.ToString());
                ViewBag.UserList = users;
            }
        }
        #endregion

        #region AddEditProduct
        public async Task<IActionResult> AddEditProduct(int? ProductID)
        {
            await UserDropDown();
            ProductModel product = new ProductModel();
            if (ProductID != null)
            {
                HttpResponseMessage response = await _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/Product/GetProductByID/{ProductID}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                    var dataOfObject = jsonObject.data;
                    product = JsonConvert.DeserializeObject<ProductModel>(dataOfObject.ToString());
                }
            }
            TempData.Keep("IsEditMode");
            return View("AddEditProduct", product);
        }
        #endregion

        #region ProductSave
        public async Task<IActionResult> ProductSave(ProductModel productModel)
        {
            await UserDropDown();
            ModelState.Remove("ProductPrice");
            if (productModel.ProductPrice <= 0)
            {
                ModelState.AddModelError("ProductPrice", "Please Enter Product Price greater than 0");
            }
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(productModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (productModel.ProductID == null)
                {
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Product/InsertProduct", content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Product/UpdateProduct/{productModel.ProductID}", content);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = productModel.ProductID == null
                        ? "Product added successfully!"
                        : "Product updated successfully!";
                    return RedirectToAction("AddEditProduct", new { ProductID = productModel.ProductID });
                }
            }
            return RedirectToAction("AddEditProduct", productModel);
        }
        #endregion
    }
}
