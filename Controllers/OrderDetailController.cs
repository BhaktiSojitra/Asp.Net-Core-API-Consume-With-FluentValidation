using API_Consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace API_Consume.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OrderDetailController(IConfiguration configuration)
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
            List<OrderDetailModel> orderdetails = new List<OrderDetailModel>();
            HttpResponseMessage response = _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/OrderDetail/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                var dataOfObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject
                    (dataOfObject, Formatting.Indented);
                orderdetails = JsonConvert.DeserializeObject<List<OrderDetailModel>>(extractedDataJson);
            }
            return View("OrderDetailList", orderdetails);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int OrderDetailID)
        {
            HttpResponseMessage response = _httpClient.DeleteAsync
                ($"{_httpClient.BaseAddress}/OrderDetail/DeleteOrderDetail/{OrderDetailID}").Result;
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

        #region OrderDropDown
        public async Task OrderDropDown()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/OrderDetail/order");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                var data = jsonObject.data; // Access the "data" property
                var orders = JsonConvert.DeserializeObject<List<OrderDropDownModel>>(data.ToString());
                ViewBag.OrderList = orders;
            }
        }
        #endregion

        #region ProductDropDown
        public async Task ProductDropDown()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/OrderDetail/product");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                var data = jsonObject.data; // Access the "data" property
                var products = JsonConvert.DeserializeObject<List<ProductDropDownModel>>(data.ToString());
                ViewBag.ProductList = products;
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

        #region AddEditOrderDetail
        public async Task<IActionResult> AddEditOrderDetail(int? OrderDetailID)
        {
            await OrderDropDown();
            await ProductDropDown();
            await UserDropDown();
            OrderDetailModel orderDetail = new OrderDetailModel();
            if (OrderDetailID != null)
            {
                TempData["IsEditMode"] = true;
                HttpResponseMessage response = await _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/OrderDetail/GetOrderDetailByID/{OrderDetailID}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                    var dataOfObject = jsonObject.data;
                    orderDetail = JsonConvert.DeserializeObject<OrderDetailModel>(dataOfObject.ToString());
                }
            }
            else
            {
                TempData["IsEditMode"] = false;
            }
            TempData.Keep("IsEditMode");
            return View("AddEditOrderDetail", orderDetail);
        }
        #endregion

        #region OrderDetailSave
        public async Task<IActionResult> OrderDetailSave(OrderDetailModel orderDetailModel)
        {
            await OrderDropDown();
            await UserDropDown();
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(orderDetailModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (orderDetailModel.OrderDetailID == null)
                {
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/OrderDetail/InsertOrderDetail", content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/OrderDetail/UpdateOrderDetail/{orderDetailModel.OrderDetailID}", content);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = orderDetailModel.OrderDetailID == null
                        ? "OrderDetail added successfully!"
                        : "OrderDetail updated successfully!";
                    return RedirectToAction("AddEditOrderDetail", new { OrderDetailID = orderDetailModel.OrderDetailID });
                }
            }
            return RedirectToAction("AddEditOrderDetail", orderDetailModel);
        }
        #endregion
    }
}
