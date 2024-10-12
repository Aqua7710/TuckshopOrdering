using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using TuckshopOrdering.Models;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace TuckshopOrdering.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult FAQ()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Contact(string name, string email, string phone, string message)
        {
            string mailBody = "<h1 style='text-align: center;'>New Message!</h1>";
            mailBody += $"<p><strong>Recipient:</strong> {name}</p>";
            mailBody += $"<p><strong>Recipients Email:</strong> {email}</p>";
            mailBody += $"<p><strong>Recipients Phone:</strong> {phone}</p>";
            mailBody += $"<p><strong>Recipients Message:</strong> {message}</p>";

            // Email configuration
            string mainTitle = "Tuckshop Message";
            string mailSubject = "Contact form";
            string fromMail = "example@outlook.com"; // senders email address
            string mailPassword = "Password@123"; // senders email password

            // if email is provided, configure and send the email
            if (!email.IsNullOrEmpty())
            {
                MailMessage contactMessage = new MailMessage(new MailAddress(fromMail, mainTitle), new MailAddress(fromMail))
                {
                    Subject = mailSubject, // set email subject
                    Body = mailBody, // set email body
                    IsBodyHtml = true // specifiy that the email body is in an hmtl format
                };

                // configures the SMTP client for sending email 
                SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com", 587)
                {
                    EnableSsl = true, // enables ssl encryption
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromMail, mailPassword) // sets senders email credentials
                };

                // sends the email
                smtp.Send(contactMessage);
                return View("ContactReceived");
            }

            return View();

        }
    }
}