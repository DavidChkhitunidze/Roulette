using Microsoft.AspNetCore.Mvc;
using Roulette.Services.Responses;
using Roulette.Services.Users.Abstracts;
using Roulette.Services.Users.Models.Read;
using Roulette.Services.Users.Models.Write;
using System.Threading.Tasks;

namespace Roulette.Api.Controllers
{
    [ProducesResponseType(500, Type = typeof(Response))]
    public class AccountController : BaseController
    {
        #region PrivateFields
        
        private readonly IUserService _user;

        #endregion

        #region Constructor

        public AccountController(IUserService user)
        {
            _user = user;
        }

        #endregion

        #region HttpPostMethods

        [ProducesResponseType(404, Type = typeof(Response))]
        [ProducesResponseType(401, Type = typeof(Response))]
        [ProducesResponseType(400, Type = typeof(Response))]
        [ProducesResponseType(200, Type = typeof(Response<UserPreviewDto>))]
        [HttpPost("login")]
        public async Task<ActionResult<Response<UserPreviewDto>>> Login([FromForm] LoginUserDto model)
        {
            var loginResponse = await _user.LoginAsync(model);
            if (!loginResponse.Success)
                return StatusCode(loginResponse.StatusCode, loginResponse);

            return StatusCode(loginResponse.StatusCode, loginResponse);
        }

        [ProducesResponseType(404, Type = typeof(Response))]
        [ProducesResponseType(401, Type = typeof(Response))]
        [ProducesResponseType(400, Type = typeof(Response))]
        [ProducesResponseType(202, Type = typeof(Response))]
        [ProducesResponseType(200, Type = typeof(Response<UserPreviewDto>))]
        [HttpPost("register")]
        public async Task<ActionResult<Response<UserPreviewDto>>> Register([FromForm] RegisterUserDto model)
        {
            var registerResponse = await _user.RegisterAsync(model);
            if (!registerResponse.Success)
                return StatusCode(registerResponse.StatusCode, registerResponse);

            return StatusCode(registerResponse.StatusCode, registerResponse);
        }

        #endregion

        #region HttpPutMethods
        
        [ProducesResponseType(401, Type = typeof(Response))]
        [ProducesResponseType(400, Type = typeof(Response))]
        [ProducesResponseType(200, Type = typeof(Response<UserPreviewDto>))]
        [HttpPut("token")]
        public async Task<ActionResult<Response<UserPreviewDto>>> Refresh(string token, string refreshToken)
        {
            var refreshTokenResponse = await _user.RefreshTokenAsync(token, refreshToken);
            if (!refreshTokenResponse.Success)
                return StatusCode(refreshTokenResponse.StatusCode, refreshTokenResponse);

            return StatusCode(refreshTokenResponse.StatusCode, refreshTokenResponse);
        }

        #endregion
    }
}
