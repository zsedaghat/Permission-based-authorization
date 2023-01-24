namespace SpaceManagment.Common
{
    public class RefreshToken
    {
        public string Jwt { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; }
    }
}
