using DocAnalytics.Domain.Entities;   

namespace DocAnalytics.Service.Auth;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
