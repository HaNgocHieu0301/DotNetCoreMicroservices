using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Models;
using Mango.Services.EmailAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDTO cartDTO)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine("<br/>Cart Email Requested ");
            message.AppendLine("<br/>Cart Email Requested ");
            message.Append("<br/>");
            message.Append("<ul>");
            foreach (var item in cartDTO.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + " x " + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");

            await LogAndEmaill(message.ToString(), cartDTO.CartHeader.Email);
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            string message = "User Registeration Successful. <br/> Email : " + email;
            await LogAndEmaill(message, "hieuhn0301@gmail.com");
        }

        private async Task<bool> LogAndEmaill(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };
                await using var _db = new AppDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();
                return true;
            }catch (Exception ex)
            {
                return false;
            }
        }
    }
}
