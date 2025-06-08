namespace Cinema_Management_System.Services.Auth
{
    public class CookieService : ICookieService
    {
        public void SetTokenCookie(HttpResponse response, string token)
        {
            response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(10)
            });
        }

        public void DeleteTokenCookie(HttpResponse response)
        {
            response.Cookies.Delete("jwt");
        }
    }
}
