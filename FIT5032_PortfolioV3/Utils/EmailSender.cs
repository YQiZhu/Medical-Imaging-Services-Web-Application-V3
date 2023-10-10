using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FIT5032_PortfolioV3.Utils
{
    public class EmailSender
    {
        // Please use your API KEY here.
        private const String API_KEY = "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";

        public void Send(String toEmail, String subject, String contents)
        {
            var client = new SendGridClient(API_KEY);
            var from = new EmailAddress("zhuyanqi001215@gmail.com", "X-rayConnext Email Comfrim");
            var to = new EmailAddress(toEmail, "");
            var plainTextContent = contents;
            var htmlContent = "<p>" + contents + "</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
        }

    }
}