using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Reso.Sdk.Core.Extension
{
    public static class AuthorExtension
    {
        public static void ConfigAuthor<TIdentityUser, TIdentityRole, TDbContext>(this IServiceCollection services) where TIdentityUser : class where TIdentityRole : class where TDbContext : DbContext
        {
            services.AddIdentityCore<TIdentityUser>(delegate (IdentityOptions options)
            {
                options.SignIn.RequireConfirmedAccount = false;
            }).AddRoles<TIdentityRole>().AddEntityFrameworkStores<TDbContext>()
                .AddDefaultTokenProviders()
                .AddSignInManager();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(delegate (AuthenticationOptions x)
            {
                x.DefaultAuthenticateScheme = "Bearer";
                x.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(delegate (JwtBearerOptions x)
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("this is secret key ad5")),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "this is issuer fht",
                    ValidAudience = "this is issuer fht"
                };
            });
        }

        public static void ConfigureAuthor(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}
