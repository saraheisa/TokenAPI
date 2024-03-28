using System.Numerics;
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
        private readonly BNBChainService _bnbChainService;


        public TokenController(TokenDbContext context, IConfiguration configuration
        , BNBChainService bnbChainService
        )
        {
            _context = context;
            _configuration = configuration;
            _bnbChainService = bnbChainService;
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
        public async Task<IActionResult> CalculateTokenData()
        {
            // Calculate total and circulating supply
            BigInteger totalSupply = await _bnbChainService.GetTotalSupplyAsync();
            BigInteger nonCirculatingSupply = await _bnbChainService.GetNonCirculatingSupplyAsync();
            BigInteger circulatingSupply = totalSupply - nonCirculatingSupply;

            var token = new TokenData
            {
                Id = 1,
                Name = "BLP token",
                TotalSupply = totalSupply.ToString(),
                CirculatingSupply = circulatingSupply.ToString()
            };

            _context.TokenData.Update(token);
            _context.SaveChanges();

            return Ok(token);
        }
    }

}
