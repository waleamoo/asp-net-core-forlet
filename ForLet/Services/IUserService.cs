namespace ForLet.Services
{
    public interface IUserService
    {
        string GetUserId();
        bool IsAuthenticated();
    }
}