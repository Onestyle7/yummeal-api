namespace yummealAPI.Dtos
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserInfo User { get; set; } = new UserInfo();
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(1);
    }

}