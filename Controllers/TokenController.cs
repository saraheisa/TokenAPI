using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TokenAPI.Controllers
{
    [ApiController]
    [Route("api/token")]
    public class TokenController : ControllerBase
    {
        private readonly TokenDbContext _context;
        private readonly IConfiguration _configuration;

        public TokenController(TokenDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var tokenService = new TokenService(_configuration);

            var token = tokenService.GenerateToken(model.Username);

            return Ok(new { token });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public IActionResult GetTokenData()
        {
            var tokenData = _context.TokenData.FirstOrDefault();

            if (tokenData == null)
            {
                return NotFound();
            }
            return Ok(tokenData);
        }

        [HttpPost]
        [Authorize]
        [Route("")]
        public IActionResult CalculateTokenData()
        {
            // Calculate total and circulating supply
            decimal totalSupply = 1000;
            decimal nonCirculatingSupply = 20;
            decimal circulatingSupply = totalSupply - nonCirculatingSupply;

            var token = new TokenData
            {
                Id = 1,
                Name = "BLP token",
                TotalSupply = totalSupply,
                CirculatingSupply = circulatingSupply
            };

            _context.TokenData.Update(token);
            _context.SaveChanges();

            return Ok();
        }
    }

}
