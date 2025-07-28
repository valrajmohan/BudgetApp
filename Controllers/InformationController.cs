using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using MyBudgetTracker.Models;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MyBudgetTracker.Controllers
{  
    public class InformationController : Controller
    {
        private readonly HttpClient client = new HttpClient();
        private readonly IOptions<MailSettings> _mailSettings;

        public InformationController(ILogger<HomeController> logger, IWebHostEnvironment env, IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Disclaimer()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ContactInfo(ContactFormModel model)
        {
            if (ModelState.IsValid)
            {
                sendMailforServayAttempt(model.Name, model.Email, model.Message);
                // Process the form data here
                // For example, save the data to the database or send an email

                // Redirect to a confirmation page or show a success message
                return RedirectToAction("ContactConfirmation");
            }

            // If the form data is not valid, return to the form view with validation messages
            return View(model);
        }

        public void sendMailforServayAttempt(string? name, string? mailid, string? message)
        {
            var mailSettings = _mailSettings.Value;
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(mailSettings.FromName, mailSettings.FromAddress));
            email.To.Add(new MailboxAddress(mailSettings.ToName, mailSettings.ToAddress));

            email.Subject = $"BudgetTracker";

            // Construct the email body with user information
            StringBuilder emailBodyBuilder = new StringBuilder();
            emailBodyBuilder.AppendLine($"User: {name}");
            emailBodyBuilder.AppendLine($"User Email: {mailid}");
            emailBodyBuilder.AppendLine($"User Message: {message}");

            string emailBody = emailBodyBuilder.ToString();


            email.Body = new TextPart(MimeKit.Text.TextFormat.Plain)
            {
                Text = emailBody
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    smtp.Connect(mailSettings.SmtpServer, mailSettings.SmtpPort, SecureSocketOptions.StartTls);
                    smtp.Authenticate(mailSettings.SmtpUsername, mailSettings.SmtpPassword);
                    smtp.Send(email);
                    Console.WriteLine("Email sent successfully!");
                }
                catch (SmtpCommandException smtpEx)
                {
                    Console.WriteLine("SMTP Command Error: " + smtpEx.Message);
                }
                catch (SmtpProtocolException smtpProtocolEx)
                {
                    Console.WriteLine("SMTP Protocol Error: " + smtpProtocolEx.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error sending email: " + ex.Message);
                }
                finally
                {
                    smtp.Disconnect(true);
                }
            }
        }

        public IActionResult ContactConfirmation()
        {
            return View();
        }

        public async Task<string> ProcessTextLogic(string input)
        {
            try
            {
                string processedContent = input;
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(input, Encoding.UTF8), "data");
                formData.Add(new StringContent("en", Encoding.UTF8), "lang");

                HttpResponseMessage response = await client.PostAsync("https://aitohumantextconverter.com/php/process.php", formData);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();

                processedContent = responseContent
                    .Replace("&lt;b&gt;", "<b>")
                    .Replace("&lt;/b&gt;", "</b>")
                    .Replace("&lt;a", "<h")
                    .Replace("&lt;/a&gt;", "</a>")
                    .Replace("&gt;", ">")
                    .Replace(@"\r\n", "")
                    .Replace("goal='", "target='")
                    .Trim();

                return processedContent;
            }
            catch (Exception ex)
            {
                return input;
            }

        }
        public async Task<string> ProcessTextLogic2(string input)
        {
            try
            {
                string processedContent = input;
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(input, Encoding.UTF8), "data");
                formData.Add(new StringContent("en", Encoding.UTF8), "lang");

                HttpResponseMessage response = await client.PostAsync("https://texttohandwriting.com/ai-text-converter/php/process.php", formData);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();

                processedContent = responseContent
                    .Replace("&lt;b&gt;", "<b>")
                    .Replace("&lt;/b&gt;", "</b>")
                    .Replace("&lt;a", "<h")
                    .Replace("&lt;/a&gt;", "</a>")
                    .Replace("&gt;", ">")
                    .Replace(@"\r\n", "")
                    .Replace("goal='", "target='")
                    .Trim();

                Console.WriteLine(processedContent);
                return processedContent;
            }
            catch (Exception ex)
            {
                return input;
            }

        }
        public async Task<string> ProcessTextLogic3(string input)
        {
            try
            {
                string processedContent = input; // Get the first "word" from the array
                                                 // Construct the API endpoint URL with query parameters
                string apiUrl = $"https://api.datamuse.com/words?rel_syn={input}&max=1";

                // Send GET request to Datamuse API
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                // Read response content
                string responseContent = await response.Content.ReadAsStringAsync();

                // Process the JSON response from Datamuse API
                // Assuming the response is a JSON array with objects containing "word" field
                JArray jsonArray = JArray.Parse(responseContent);

                if (jsonArray.Count > 0)
                {
                    processedContent = jsonArray[0]["word"].ToString();
                }
                // Example processing for formatting
                processedContent = processedContent.Replace("&lt;b&gt;", "<b>")
                                               .Replace("&lt;/b&gt;", "</b>")
                                               .Replace("&lt;a", "<h")
                                               .Replace("&lt;/a&gt;", "</a>")
                                               .Replace("&gt;", ">")
                                               .Replace(@"\r\n", "")
                                               .Replace("goal='", "target='")
                                               .Trim();
                return processedContent;
            }
            catch (Exception ex)
            {
                return input;
            }

        }
    }
}
