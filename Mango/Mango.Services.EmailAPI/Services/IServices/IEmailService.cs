using Mango.Services.EmailAPI.Models;

namespace Mango.Services.EmailAPI.Services.IServices
{
    public interface IEmailService
    {
        public Task EmailCartAndLog(CartDTO cartDTO);
        public Task RegisterUserEmailAndLog(string email);
    }
}
