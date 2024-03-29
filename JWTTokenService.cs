using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JWTTokenService
{
    private readonly string _jwtKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JWTTokenService(IConfiguration configuration)
    {
        _jwtKey = configuration["Jwt"] ?? throw new InvalidOperationException("JWT secret key is missing or empty. Please provide a valid JWT secret key.");
        _issuer = configuration["JwtIssuer"] ?? "";
        _audience = configuration["JwtAudience"] ?? "";
    }

    public string GenerateToken(string username)
    {
        var securityKey = GetSecurityKey();
        var credentials = GetSigningCredentials(securityKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Role, "user")
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddDays(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private SecurityKey GetSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
    }

    private SigningCredentials GetSigningCredentials(SecurityKey securityKey)
    {
        return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    }
}
