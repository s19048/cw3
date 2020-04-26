using cw3.DTOs.Requests;
using cw3.DTOs.Responses;

namespace cw3.Services
{
    interface ILoginService
    {
        TokenResponse Login(LoginRequestDto login);
        TokenResponse RefreshToken(string rToken);
    }
}
