using Mango.Services.AuthAPI.Models;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GeneratorToken(ApplicationUser user, IEnumerable<string> roles);
    }
}
