using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MyBudgetTracker.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Mail;

namespace MyBudgetTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HomeController> _logger;

        private readonly string userFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UserRegistrationDetails", "users.json");

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        public IActionResult RegisterUser(UserRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var users = GetUserList();
                if (!users.Any(u => u.Email == model.Email || u.Username == model.Username))
                {
                    users.Add(model);
                    System.IO.File.WriteAllText(userFilePath, JsonConvert.SerializeObject(users));
                    return Json(new { success = true, message = "User registered successfully" });
                }
                return Json(new { success = false, message = "User already exists" });
            }

            // If the model is invalid, extract and return validation errors
            var errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, message = "Validation failed", errors = errorMessages });
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var users = GetUserList();

            // Find the user based on username/email and password
            var user = users.FirstOrDefault(u =>
                (u.Username == model.UsernameOrEmail || u.Email == model.UsernameOrEmail) &&
                u.Password == model.Password);

            // Check if the user exists and the credentials are correct
            if (user != null)
            {
                // Return user details upon successful login
                return Json(new
                {
                    success = true,
                    username = user.Username,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName
                });
            }

            // Return error message for invalid login
            return Json(new { success = false, message = "Invalid login" });
        }


        [HttpPost]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var users = GetUserList();
            var user = users.FirstOrDefault(u => u.Email == model.Email || u.Username == model.Email); // Check both Email and Username
            if (user != null)
            {
                string resetCode = new Random().Next(1000, 9999).ToString();
                user.ResetCode = resetCode;

                System.IO.File.WriteAllText(userFilePath, JsonConvert.SerializeObject(users));

                // Send reset code to email
                SendResetEmail(user.Email, resetCode);
                return Json(new { success = true, message = $"Reset code sent to your email {user.Email}" });
            }
            return Json(new { success = false, message = "Email/Username not found" });
        }


        [HttpPost]
        public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
        {
            var users = GetUserList();
            var user = users.FirstOrDefault(u => u.Email == model.Email && u.ResetCode == model.ResetCode);
            if (user != null)
            {
                user.Password = model.NewPassword;
                user.ResetCode = null; // Clear reset code after successful use
                System.IO.File.WriteAllText(userFilePath, JsonConvert.SerializeObject(users));
                return Json(new { success = true, message = "Password reset successful" });
            }
            return Json(new { success = false, message = "Invalid reset code" });
        }



        private List<UserRegistrationModel> GetUserList()
        {
            if (System.IO.File.Exists(userFilePath))
            {
                var json = System.IO.File.ReadAllText(userFilePath);
                return JsonConvert.DeserializeObject<List<UserRegistrationModel>>(json) ?? new List<UserRegistrationModel>();
            }
            return new List<UserRegistrationModel>();
        }

        private void SendResetEmail(string toEmail, string resetCode)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Personal Finance App", "velrajmohan1009@gmail.com"));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Password Reset Code";

            var builder = new BodyBuilder { HtmlBody = $"Your password reset code is <strong>{resetCode}</strong>" };
            message.Body = builder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                client.Authenticate("velrajmohan1009@gmail.com", "whbmuylwchtrpnxt");
                client.Send(message);
                client.Disconnect(true);
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetBudgetData(string name)
        {
            string jsonFilePath = Path.Combine(_env.WebRootPath, "UsersBudgetData", $"{name}.json");

            if (!System.IO.File.Exists(jsonFilePath))
            {
                 jsonFilePath = Path.Combine(_env.WebRootPath, "UsersBudgetData", "defaultMasterBudgetData.json");
            }

            string jsonData = await System.IO.File.ReadAllTextAsync(jsonFilePath);

            // Assuming your JSON file contains valid JSON, deserialize it if needed
            return Content(jsonData, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> SaveBudgetData([FromBody] BudgetDataRequest request)
        {
            string? userData = request?.UserData; // Get the user data
            string folderPath = Path.Combine(_env.WebRootPath, "UsersBudgetData");
            string jsonFilePath = Path.Combine(folderPath, $"{userData}.json");

            try
            {
                // Ensure the directory exists
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Serialize the incoming userMasterData to JSON format
                string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(request?.UserMasterData, Newtonsoft.Json.Formatting.Indented);

                // Check if the file already exists
                if (System.IO.File.Exists(jsonFilePath))
                {
                    // File exists, update it with the new data
                    await System.IO.File.WriteAllTextAsync(jsonFilePath, jsonData);
                    return Ok("Data updated successfully.");
                }
                else
                {
                    // File doesn't exist, create a new file and save the data
                    await System.IO.File.WriteAllTextAsync(jsonFilePath, jsonData);
                    return Ok("Data saved successfully.");
                }
            }
            catch (Exception ex)
            {
                // Log error, return error response
                return StatusCode(500, $"Error saving data: {ex.Message}");
            }
        }

    }
}
