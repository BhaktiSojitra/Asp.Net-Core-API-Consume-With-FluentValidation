using API_Consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using API_Consume.Validators;
using System.Text;
using System.Text.RegularExpressions;

namespace API_Consume.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public UserController(IConfiguration configuration)
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
            List<UserModel> users = new List<UserModel>();
            HttpResponseMessage response = _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/User/GetAll").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                var dataOfObject = jsonObject.data;
                var extractedDataJson = JsonConvert.SerializeObject
                    (dataOfObject, Formatting.Indented);
                users = JsonConvert.DeserializeObject<List<UserModel>>(extractedDataJson);
            }
            return View("UserList", users);
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int UserID)
        {
            HttpResponseMessage response = _httpClient.DeleteAsync
                ($"{_httpClient.BaseAddress}/User/DeleteUser/{UserID}").Result;
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

        #region AddEditUser
        public async Task<IActionResult> AddEditUser(int? UserID)
        {
            UserModel user = new UserModel();
            if (UserID != null)
            {
                HttpResponseMessage response = await _httpClient.GetAsync
                ($"{_httpClient.BaseAddress}/User/GetUserByID/{UserID}");
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JsonConvert.DeserializeObject<dynamic>(data);
                    var dataOfObject = jsonObject.data;
                    user = JsonConvert.DeserializeObject<UserModel>(dataOfObject.ToString());
                }
            }
            TempData.Keep("IsEditMode");
            return View("AddEditUser", user);
        }
        #endregion

        #region UserSave
        public async Task<IActionResult> UserSave(UserModel userModel)
        {
            //UserValidator validator = new UserValidator();
            //var validationResult = validator.Validate(userModel);

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(userModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (userModel.UserID == null)
                {
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/User/InsertUser", content);
                }
                else
                {
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/User/UpdateUser/{userModel.UserID}", content);
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = userModel.UserID == null
                        ? "User added successfully!"
                        : "User updated successfully!";
                    return RedirectToAction("AddEditUser", new { UserID = userModel.UserID });
                }
            }
            return RedirectToAction("AddEditUser", userModel);
        }
        #endregion
    }
}
