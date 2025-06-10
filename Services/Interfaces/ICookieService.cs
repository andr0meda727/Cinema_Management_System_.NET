namespace Cinema_Management_System.Services.Interfaces
{
    public interface ICookieService
    {
        void SetTokenCookie(HttpResponse response, string token);
        void DeleteTokenCookie(HttpResponse response);
    }
}
