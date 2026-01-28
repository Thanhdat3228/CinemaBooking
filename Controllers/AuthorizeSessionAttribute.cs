using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CinemaBooking.Controllers
{
    /// <summary>
    /// Attribute để kiểm tra xác thực (authentication) dựa vào Session
    /// Sử dụng: [AuthorizeSession]
    /// </summary>
    public class AuthorizeSessionAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            
            if (userId == null)
            {
                // Lưu URL hiện tại để redirect lại sau khi login
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                
                // Redirect tới Login
                context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl });
            }
        }
    }
}
