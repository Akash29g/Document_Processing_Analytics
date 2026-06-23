using DocAnalytics.Domain.Entities;   // 👈 adjust to wherever your User entity lives

namespace DocAnalytics.Service.Auth;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
