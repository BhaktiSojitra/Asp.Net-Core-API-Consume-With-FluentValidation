using API_Consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace API_Consume.Controllers
{
    public class OrderController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OrderController(IConfiguration configuration)
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
            List<OrderModel> orders = new List<OrderModel>();
            HttpResponseMessage response = _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/Order/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                var dataOfObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject
                    (dataOfObject, Formatting.Indented);
                orders = JsonConvert.DeserializeObject<List<OrderModel>>(extractedDataJson);
            }
            return View("OrderList", orders);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int OrderID)
        {
            HttpResponseMessage response = _httpClient.DeleteAsync
                ($"{_httpClient.BaseAddress}/Order/DeleteOrder/{OrderID}").Result;
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

        #region CustomerDropDown
        public async Task CustomerDropDown()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Order/customer");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                var data = jsonObject.data; // Access the "data" property
                var customers = JsonConvert.DeserializeObject<List<CustomerDropDownModel>>(data.ToString());
                ViewBag.CustomerList = customers;
            }
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

        #region AddEditOrder
        public async Task<IActionResult> AddEditOrder(int? OrderID)
        {
            await CustomerDropDown();
            await UserDropDown();
            OrderModel order = new OrderModel();
            if (OrderID != null)
            {
                TempData["IsEditMode"] = true;
                HttpResponseMessage response = await _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/Order/GetOrderByID/{OrderID}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                    var dataOfObject = jsonObject.data;
                    order = JsonConvert.DeserializeObject<OrderModel>(dataOfObject.ToString());
                }
            }
            else
            {
                TempData["IsEditMode"] = false;
            }
            TempData.Keep("IsEditMode");
            return View("AddEditOrder", order);
        }
        #endregion

        #region OrderSave
        public async Task<IActionResult> OrderSave(OrderModel orderModel)
        {
            await CustomerDropDown();
            await UserDropDown();
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(orderModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (orderModel.OrderID == null)
                {
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Order/InsertOrder", content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Order/UpdateOrder/{orderModel.OrderID}", content);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = orderModel.OrderID == null
                        ? "Order added successfully!"
                        : "Order updated successfully!";
                    return RedirectToAction("AddEditOrder", new { OrderID = orderModel.OrderID });
                }
            }
            return RedirectToAction("AddEditOrder", orderModel);
        }
        #endregion
    }
}
