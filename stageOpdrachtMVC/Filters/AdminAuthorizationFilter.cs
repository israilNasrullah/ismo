using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace stageOpdrachtMVC.Filters
{
    public class AuthorizationFilter : Attribute, IAuthorizationFilter
    {
        private string _allowed;
        
        public AuthorizationFilter(string allowed)
        {
            _allowed = allowed;
            
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var returnUrl = context.HttpContext.Request.Path;
            string tokenString = context.HttpContext.Request.Cookies["token"];

            if (string.IsNullOrEmpty(tokenString))
            {
                context.Result = new RedirectToActionResult("Index", "Home", new { returnUrl = returnUrl});  // Redirect to login page
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = "https://localhost:7139/",
                ValidAudience = "https://localhost:7139/",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mijnkey123456789012345678901234567890"))
            };

            try
            {
                var principal = tokenHandler.ValidateToken(tokenString, tokenValidationParameters, out var securityToken);
               
                if (principal is ClaimsPrincipal claimsPrincipal)
                {
                    context.HttpContext.User = claimsPrincipal;
                    var roleClaim = claimsPrincipal.FindFirst("Role"); 

                    if (roleClaim != null)
                    {
                        var userRole = roleClaim.Value;

                        // Sla de rol van de gebruiker op in de sessie
                       context.HttpContext.Session.SetString("role", userRole);
                    }

                    //claimsPrincipal.FindFirst(ClaimTypes.Name);


                    //context.HttpContext.Items["user"] = principal;
                    ///HttpContext.Current.User
                    var allowed = _allowed?.Split(';');
                    var claims = allowed == null ? null : claimsPrincipal.FindAll((claim) => _allowed.Contains(claim.Value, StringComparison.OrdinalIgnoreCase));

                    if (claims == null || claims.Count() == 0)
                    {
                        context.Result = new RedirectToActionResult("Index", "Access", null);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log exception if needed
                context.Result = new RedirectToActionResult("Index", "Home", null); // Redirect to home page
                return;
            }
        }
    }
}
