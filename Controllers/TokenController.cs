using System.Numerics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TokenAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class TokenController : ControllerBase
    {
        private readonly TokenDbContext _context;
        private readonly BNBChainService _bnbChainService;
        private readonly JWTTokenService _jwtTokenService;

        public TokenController(TokenDbContext context, JWTTokenService jWTTokenService, BNBChainService bnbChainService)
        {
            _context = context;
            _jwtTokenService = jWTTokenService;
            _bnbChainService = bnbChainService;
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var token = _jwtTokenService.GenerateToken(model.Username);
            return Ok(new { token });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("token")]
        public IActionResult GetTokenData()
        {
            var tokenData = _context.TokenData.FirstOrDefault();
            return tokenData != null ? Ok(tokenData) : NotFound();
        }

        [HttpPost]
        [Authorize]
        [Route("token")]
        public async Task<IActionResult> CalculateTokenData()
        {
            try
            {
                var totalSupplyTask = _bnbChainService.GetTotalSupplyAsync();
                var nonCirculatingSupplyTask = _bnbChainService.GetNonCirculatingSupplyAsync();

                await Task.WhenAll(totalSupplyTask, nonCirculatingSupplyTask);

                BigInteger totalSupply = await totalSupplyTask;
                BigInteger nonCirculatingSupply = await nonCirculatingSupplyTask;
                BigInteger circulatingSupply = totalSupply - nonCirculatingSupply;

                var tokenData = new TokenData
                {
                    Id = 1,
                    Name = "BLP token",
                    TotalSupply = totalSupply.ToString(),
                    CirculatingSupply = circulatingSupply.ToString()
                };

                _context.TokenData.Update(tokenData);
                _context.SaveChanges();

                return Ok(tokenData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
