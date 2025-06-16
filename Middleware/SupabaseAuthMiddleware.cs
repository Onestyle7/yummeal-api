using System.Security.Claims;
using System.Text.Json;

namespace yummealAPI.Middleware
{
    public class SupabaseAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Supabase.Client _supabase;
        private readonly ILogger<SupabaseAuthMiddleware> _logger;

        public SupabaseAuthMiddleware(
            RequestDelegate next,
            Supabase.Client supabase,
            ILogger<SupabaseAuthMiddleware> logger
        )
        {
            _next = next;
            _supabase = supabase;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    _logger.LogInformation(
                        "Attempting to validate token: {TokenStart}...",
                        token.Substring(0, Math.Min(20, token.Length))
                    );

                    // Sprawdź token przez Supabase
                    var user = await _supabase.Auth.GetUser(token);

                    if (user != null)
                    {
                        _logger.LogInformation(
                            "Token validated successfully for user: {UserId}",
                            user.Id
                        );

                        // Stwórz claims dla użytkownika
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Email, user.Email ?? ""),
                            new Claim("sub", user.Id),
                            new Claim("email", user.Email ?? ""),
                            new Claim("email_verified", user.EmailConfirmedAt.HasValue.ToString()),
                            new Claim("role", "authenticated"),
                        };

                        var identity = new ClaimsIdentity(claims, "supabase");
                        context.User = new ClaimsPrincipal(identity);

                        _logger.LogInformation(
                            "Successfully set user claims for: {UserId}",
                            user.Id
                        );
                    }
                    else
                    {
                        _logger.LogWarning("Token validation returned null user");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating token: {Error}", ex.Message);
                    // Token nieprawidłowy - nie robimy nic, użytkownik pozostaje nieautoryzowany
                }
            }
            else
            {
                _logger.LogInformation("No Authorization header found");
            }

            await _next(context);
        }
    }
}
