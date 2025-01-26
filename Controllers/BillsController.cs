using API_Consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace API_Consume.Controllers
{
    public class BillsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public BillsController(IConfiguration configuration)
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
            List<BillsModel> bills = new List<BillsModel>();
            HttpResponseMessage response = _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/Bills/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                var dataOfObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject
                    (dataOfObject, Formatting.Indented);
                bills = JsonConvert.DeserializeObject<List<BillsModel>>(extractedDataJson);
            }
            return View("BillsList", bills);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int BillID)
        {
            HttpResponseMessage response = _httpClient.DeleteAsync
                ($"{_httpClient.BaseAddress}/Bills/DeleteBill/{BillID}").Result;
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

        #region UserDropDown
        public async Task UserDropDown()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Product/user");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                var data = jsonObject.data; // Access the "data" property
                var users = JsonConvert.DeserializeObject<List<UserDropDownModel>>(data.ToString());
                ViewBag.UserList = users;
            }
        }
        #endregion

        #region AddEditBills
        public async Task<IActionResult> AddEditBills(int? BillID)
        {
            await OrderDropDown();
            await UserDropDown();
            BillsModel Bills = new BillsModel();
            if (BillID != null)
            {
                HttpResponseMessage response = await _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/Bills/GetBillByID/{BillID}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                    var dataOfObject = jsonObject.data;
                    Bills = JsonConvert.DeserializeObject<BillsModel>(dataOfObject.ToString());
                }
            }
            return View("AddEditBills", Bills);
        }
        #endregion

        #region BillsSave
        public async Task<IActionResult> BillsSave(BillsModel billsModel)
        {
            await OrderDropDown();
            await UserDropDown();
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(billsModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (billsModel.BillID == null)
                {
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Bills/InsertBill", content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Bills/UpdateBill/{billsModel.BillID}", content);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = billsModel.BillID == null
                        ? "Bills added successfully!"
                        : "Bills updated successfully!";
                    return RedirectToAction("AddEditBills", new { BillID = billsModel.BillID });
                }
            }
            return RedirectToAction("AddEditBills", billsModel);
        }
        #endregion
    }
}
