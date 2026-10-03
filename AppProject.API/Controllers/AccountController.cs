using System.Security.Claims;
using AppProject.API.Helper;
using AutoMapper;
using Core.DTO;
using Core.Entities;
using Core.interfaces;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.API.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [Authorize]
        [HttpGet("get-address-for-user")]
        public async Task<IActionResult> getAddress()
        {
            var address = await work.Auth.getUserAddress(User.FindFirst(ClaimTypes.Sid).Value);
            var result = mapper.Map<ShipAddressDTO>(address);
            return Ok(result);
        }

        [HttpGet("Logout")]
        public IActionResult logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                IsEssential = true,
                //Domain = "localhost",
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            };
            Response.Cookies.Append("token", "", cookieOptions);
            Response.Cookies.Append("refreshToken", "", cookieOptions);

            return Ok(new ResponseAPI(200, "You have successfully logged out."));
        }


        [Authorize]
        [HttpGet("get-user-name")]
        public IActionResult GetUserName()
        {
            return Ok(new ResponseAPI(200, User.Identity.Name));
        }

        [Authorize]
        [HttpGet("IsUserAuth")]
        public async Task<IActionResult> IsUserAuth(
            [FromServices] UserManager<AppUser> userManager,
            [FromServices] IGenerateToken generateToken) 
        {
            if (!User.Identity!.IsAuthenticated)
            {
                return Ok(new UserSessionDto { IsAuthenticated = false });
            }

            var userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var tokenSecurityStamp = User.FindFirst("AspNet.Identity.SecurityStamp")?.Value
                                     ?? User.FindFirst(ClaimTypes.SerialNumber)?.Value;

            var roles = await userManager.GetRolesAsync(user);
            var dbClaims = await userManager.GetClaimsAsync(user);

            if (!string.IsNullOrEmpty(tokenSecurityStamp) && tokenSecurityStamp != user.SecurityStamp)
            {
                var newToken = generateToken.GetAndCreateToken(user, roles, dbClaims, isRefreshToken: false);
                var newRefreshToken = generateToken.GetAndCreateToken(user, roles, dbClaims, isRefreshToken: true);
                var isHttps = Request.IsHttps;
                Response.Cookies.Append("token", newToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = isHttps,
                    SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                    IsEssential = true,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });

                Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    //Secure = true,
                    Secure = isHttps,
                    SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                    IsEssential = true,
                    //Domain = "localhost",
                    Expires = DateTime.Now.AddMinutes(30)
                });
                
            }

            var standardClaimTypes = new[]
            {
                ClaimTypes.Sid, ClaimTypes.Name, ClaimTypes.Role, "exp", "iss", "aud", "AspNet.Identity.SecurityStamp"
            };
            var customClaims = dbClaims
                .Where(c => !standardClaimTypes.Contains(c.Type))
                .ToDictionary(c => c.Type, c => c.Value);

            return Ok(new UserSessionDto
            {
                IsAuthenticated = true,
                UserId = user.Id,
                Email = user.UserName,
                Roles = roles.ToList(),
                Claims = customClaims,
                TokenExpired = User.FindFirst("exp")?.Value != null ? long.Parse(User.FindFirst("exp")!.Value) : 0
            });
        }


        [Authorize]
        [HttpPut("update-address")]
        public async Task<IActionResult> updateAddress(ShipAddressDTO addressDTO)
        {
            if (addressDTO == null) throw new ArgumentNullException(nameof(addressDTO));
            if (addressDTO == null) throw new ArgumentNullException(nameof(addressDTO));
            if (addressDTO == null) throw new ArgumentNullException(nameof(addressDTO));
            if (addressDTO == null) throw new ArgumentNullException(nameof(addressDTO));
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var address = mapper.Map<Address>(addressDTO);
            var result = await work.Auth.UpdateAddress(email, address);
            return result ? Ok() : BadRequest();
        }

        [HttpPost("Register")]
        public async Task<ActionResult<RegisterDTO>> register(RegisterDTO registerDTO)
        {
            string result = await work.Auth.RegisterAsync(registerDTO);
            if (result != "done")
            {
                return BadRequest(new ResponseAPI(400, result));
            }

            return Ok(new ResponseAPI(200, result));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> login(LoginDTO loginDTO)
        {
            AuthResponse result = await work.Auth.LoginAsync(loginDTO);
            if (string.IsNullOrEmpty(result.Token))
            {
                return BadRequest(new ResponseAPI(400, result?.Message ?? "Invalid login attempt"));
            }

            var isHttps = Request.IsHttps;
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = isHttps,
                //Secure = true,
                SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                IsEssential = true,
                //Domain = "localhost",
                Expires = DateTime.Now.AddMinutes(30),
            };
            Response.Cookies.Append("token", result.Token, cookieOptions);

            var refreshTokenOptions = new CookieOptions
            {
                HttpOnly = true,
                //Secure = true,
                Secure = isHttps,
                SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                IsEssential = true,
                //Domain = "localhost",
                Expires = DateTime.Now.AddMinutes(30)
            };
            Response.Cookies.Append("refreshToken", result.RefreshToken, refreshTokenOptions);

            return Ok(new ResponseAPI(200, "Login successful"));
        }


        [Authorize]
        [HttpGet("loginByRefreshToken")]
        public async Task<ActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new ResponseAPI(401, "The session has ended; please log in again."));
            }

            var result = await work.Auth.LoginByRefeshToken(refreshToken);

            if (string.IsNullOrEmpty(result.Token))
            {
                return BadRequest(new ResponseAPI(400, result?.Message ?? "The token renewal process failed."));
            }

            var isHttps = Request.IsHttps;
            Response.Cookies.Append("token", result.Token, new CookieOptions
            {
                HttpOnly = true,
                //Secure = true,
                Secure = isHttps,
                SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                IsEssential = true,
                // Domain = "localhost",
                Expires = DateTime.Now.AddMinutes(30)
            });

            Response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                //Secure = true,
                Secure = isHttps,
                SameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax,
                IsEssential = true,
                //Domain = "localhost",
                Expires = DateTime.Now.AddMinutes(30)
            });

            return Ok(new ResponseAPI(200, "The token has been successfully renewed."));
        }


        // ====================For localStorage===============================================================

        // [HttpPost("Login")]
        // public async Task<IActionResult> login(LoginDTO loginDTO)
        // {
        //     AuthResponse result = await work.Auth.LoginAsync(loginDTO);
        //     if (result.Message.StartsWith("please"))
        //     {
        //         return Ok(new {success = false, message = result.Message});
        //     }
        //
        //     return Ok(new {success = true, message = result.Message, result.Token, result.TokenExpired, result.RefreshToken });
        // }


        // [HttpGet("loginByRefeshToken")]
        // public async Task<ActionResult<AuthResponse>> LoginByRefeshToken(string refreshToken)
        // {
        //     AuthResponse result = await work.Auth.LoginByRefeshToken(refreshToken);
        //
        //     if (!string.IsNullOrEmpty(result.Message) && result.Message.StartsWith("please"))
        //     {
        //         return Ok(new {success = false, message = result.Message});
        //     }
        //     return Ok(new {result.Token, result.TokenExpired, result.RefreshToken });
        // }


        [HttpPost("active-account")]
        public async Task<ActionResult<ActiveAccountDTO>> active(ActiveAccountDTO accountDTO)
        {
            var result = await work.Auth.ActiveAccount(accountDTO);
            return result ? Ok(new ResponseAPI(200)) : BadRequest(new ResponseAPI(200));
        }


        [HttpGet("send-email-forget-password")]
        public async Task<IActionResult> forget(string email)
        {
            var result = await work.Auth.SendEmailForForgetPassword(email);
            return result ? Ok(new ResponseAPI(200)) : BadRequest(new ResponseAPI(200));
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> reset(RestPasswordDTO restPasswordDTO)
        {
            var result = await work.Auth.ResetPassword(restPasswordDTO);
            if (result == "done")
            {
                return Ok(new ResponseAPI(200));
            }

            return BadRequest(new ResponseAPI(400));
        }
    }
}