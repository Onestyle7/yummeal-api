using System.Security.Claims;

namespace yummealAPI.Middleware
{
    public class SupabaseAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Supabase.Client _supabase;

        public SupabaseAuthMiddleware(RequestDelegate next, Supabase.Client supabase)
        {
            _next = next;
            _supabase = supabase;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                
                try
                {
                    // Sprawdź token przez Supabase
                    var user = await _supabase.Auth.GetUser(token);
                    
                    if (user != null)
                    {
                        // Stwórz claims dla użytkownika
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Email, user.Email ?? ""),
                            new Claim("sub", user.Id),
                            new Claim("email", user.Email ?? "")
                        };

                        var identity = new ClaimsIdentity(claims, "supabase");
                        context.User = new ClaimsPrincipal(identity);
                    }
                }
                catch (Exception)
                {
                    // Token nieprawidłowy - nie robimy nic, użytkownik pozostaje nieautoryzowany
                }
            }

            await _next(context);
        }
    }
}