using API_Consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace API_Consume.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public CustomerController(IConfiguration configuration)
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
            List<CustomerModel> customers = new List<CustomerModel>();
            HttpResponseMessage response = _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/Customer/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                var dataOfObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject
                    (dataOfObject, Formatting.Indented);
                customers = JsonConvert.DeserializeObject<List<CustomerModel>>(extractedDataJson);
            }
            return View("CustomerList", customers);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int CustomerID)
        {
            HttpResponseMessage response = _httpClient.DeleteAsync
                ($"{_httpClient.BaseAddress}/Customer/DeleteCustomer/{CustomerID}").Result;
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

        #region AddEditCustomer
        public async Task<IActionResult> AddEditCustomer(int? CustomerID)
        {
            await UserDropDown();
            CustomerModel customer = new CustomerModel();
            if (CustomerID != null)
            {
                HttpResponseMessage response = await _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/Customer/GetCustomerByID/{CustomerID}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                    var dataOfObject = jsonObject.data;
                    customer = JsonConvert.DeserializeObject<CustomerModel>(dataOfObject.ToString());
                }
            }
            TempData.Keep("IsEditMode");
            return View("AddEditCustomer", customer);
        }
        #endregion

        #region CustomerSave
        public async Task<IActionResult> CustomerSave(CustomerModel customerModel)
        {
            await UserDropDown();
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(customerModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (customerModel.CustomerID == null)
                {
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Customer/InsertCustomer", content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Customer/UpdateCustomer/{customerModel.CustomerID}", content);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = customerModel.CustomerID == null
                        ? "Customer added successfully!"
                        : "Customer updated successfully!";
                    return RedirectToAction("AddEditCustomer", new { CustomerID = customerModel.CustomerID });
                }
            }
            return RedirectToAction("AddEditCustomer", customerModel);
        }
        #endregion
    }
}
