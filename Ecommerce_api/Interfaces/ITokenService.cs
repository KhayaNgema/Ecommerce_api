using Ecommerce_api.Models;

public interface ITokenService
{
    Task<string> GenerateToken(UserBaseModel user);
}
