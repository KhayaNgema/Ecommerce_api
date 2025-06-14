namespace Ecommerce_api.Interfaces
{
    public interface IActivityLogger
    {
        Task Log(string activity, string userId);
    }
}
