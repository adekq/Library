namespace JwtSecurity
{
    public class JwtTokenConfiguration
    {
        public string Secret { get; set; }

        public JwtClaims Claims { get; set; }
    }

    public class JwtClaims
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int ExpirationTime { get; set; }
    }
}