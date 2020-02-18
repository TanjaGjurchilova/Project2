using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Project2.Repositories;
using Project2.Repositories.Abstract;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Project2.Helpers
{
   
    public static class SendMailHelper
    {       
        public static bool Send(string to, string body)
        {
            string from = "ins.tanja.test@gmail.com";
            string mailPassword = "novpass123";
            try
            {
                using (MailMessage mm = new MailMessage(from, to))
                {
                    mm.Subject = "password reset";
                    mm.Body = body;
                    mm.IsBodyHtml = false;
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        EnableSsl = true
                    };
                    NetworkCredential NetworkCred = new NetworkCredential(from, mailPassword);
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
      


    }
}
