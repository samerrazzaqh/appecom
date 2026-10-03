using System.Security.Claims;
using Core.DTO;
using Core.Entities;
using Core.interfaces;
using Core.Services;
using Core.Sharing;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositries
{
    public class AuthRepositry : IAuth
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IEmailService emailService;
        private readonly SignInManager<AppUser> signInManager;
        private readonly IGenerateToken generateToken;
        private readonly AppDbContext context;
        private readonly IConfiguration _configuration;
        public AuthRepositry(UserManager<AppUser> userManager, IEmailService emailService, SignInManager<AppUser> signInManager, 
            IGenerateToken generateToken, AppDbContext context,IConfiguration configuration)
        {
            this.userManager = userManager;
            this.emailService = emailService;
            this.signInManager = signInManager;
            this.generateToken = generateToken;
            this.context = context;
            _configuration = configuration;
        }
        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
            {
                return null;
            }
            if (await userManager.FindByNameAsync(registerDTO.UserName) is not null)
            {
                return "this UserName is already registerd";
            }
            if (await userManager.FindByEmailAsync(registerDTO.Email) is not null)
            {
                return "this email is already registerd";
            }

            AppUser user = new()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.UserName,
                DispalyName = registerDTO.DisplayName,
            };

            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (result.Succeeded is not true)
            {
                return result.Errors.ToList()[0].Description;
            }
            
            //Create new User Role For new User
            await userManager.AddToRoleAsync(user, "User");
            var defaultClaims = new List<Claim>
            {
                new Claim("Edit Role", "false"),
                new Claim("Delete Role", "false"),
                new Claim("Create Role", "false")
            };
            await userManager.AddClaimsAsync(user, defaultClaims);
            
            // Send Active Email
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            await SendEmail(user.Email, token, "active", "ActiveEmail", "Please active your email, click on button to active");
            return "done";
        }


        public async Task SendEmail(string email, string code, string component, string subject, string message)
        {
            // قراءة رابط الكلاينت من appsettings.json الخاص بالـ API
            var clientBaseUrl = _configuration["ClientSettings:BaseUrl"] ?? "http://localhost:6001/";

            var result = new EmailDTO(
                email,
                "a.j47sharp@gmail.com",
                subject,
                EmailStringBody.send(email, code, component, message, clientBaseUrl)
            );

            await emailService.SendEmail(result);
        }




        public async Task<AuthResponse> LoginAsync(LoginDTO login)
        {
            var authResponse = new AuthResponse();
            if (login == null)
            {
                return null;
            }
            var finduser = await userManager.FindByEmailAsync(login.Email);

            if (finduser == null)
            {
                authResponse.Message = "please check your email, something went wrong";
                return authResponse;
            }

            if (!finduser.EmailConfirmed)
            {
                string token = await userManager.GenerateEmailConfirmationTokenAsync(finduser);
                
                await SendEmail(finduser.Email, token, "active", "ActiveEmail", "Please active your email, click on button to active");
                
                authResponse.Message = "please confirem your email first, we have send activat to your E-mail";
                return authResponse;
            }
            
            bool isLockedOut = finduser.LockoutEnd.HasValue && finduser.LockoutEnd.Value > DateTimeOffset.UtcNow;
            if (isLockedOut)
            {
                if (finduser.LockoutEnd.Value.Year >= 9999)
                {
                    authResponse.Message = "Your account has been permanently banned. Please contact technical support.";
                }
                else
                {
                    var localLockoutEnd = finduser.LockoutEnd.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
                    authResponse.Message = $"Your account is temporarily locked until: {localLockoutEnd}";
                }
                return authResponse;
            }

            var result = await signInManager.CheckPasswordSignInAsync(finduser, login.Password, true);

            if (result.Succeeded)
            {
                var userRoles = await userManager.GetRolesAsync(finduser!);
                IList<Claim> userClaims = await userManager.GetClaimsAsync(finduser!);
                var token = generateToken.GetAndCreateToken(finduser,userRoles,userClaims, isRefreshToken: false);
                var refreshToken = generateToken.GetAndCreateToken(finduser, userRoles,userClaims,isRefreshToken: true);

                await generateToken.AddRefreshTokenModel(new CreateRefreshToken
                {
                    RefreshToken = refreshToken,
                    UserId = finduser.Id,
                });
                return new AuthResponse
                {
                    Message = "Success",
                    Token = token,
                    RefreshToken = refreshToken,
                };
            }
            authResponse.Message = "please check your password, something went wrong";
            return authResponse;
        }
        
        
        public async Task<AuthResponse> LoginByRefeshToken(string refreshToken)
        {
            var refreshTokenModel = await generateToken.GetRefreshTokenModel(refreshToken);
            if (refreshTokenModel == null)
            {
                return new AuthResponse
                {
                    Message = "please check your refresh token, it is invalid or expired"
                };
            }
            var finduser = await userManager.FindByIdAsync(refreshTokenModel.UserId);
            if (finduser == null)
            {
                return new AuthResponse
                {
                    Message = "please check your email and password, something went wrong",
                };
            }
            
            var userRoles = await userManager.GetRolesAsync(finduser!);
            IList<Claim> userClaims = await userManager.GetClaimsAsync(finduser!);
            var newToken =  generateToken.GetAndCreateToken(finduser,userRoles,userClaims, isRefreshToken: false);
            var newRefreshToken =  generateToken.GetAndCreateToken(finduser, userRoles,userClaims,isRefreshToken: true);

            await generateToken.AddRefreshTokenModel(new CreateRefreshToken
            {
                RefreshToken = newRefreshToken,
                UserId = refreshTokenModel.UserId
            });
            
            return new AuthResponse
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                Message = "Success"
            };
        }

        public async Task<bool> SendEmailForForgetPassword(string email)
        {
            var findUser = await userManager.FindByEmailAsync(email);
            if (findUser is null)
            {
                return false;
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(findUser);
            await SendEmail(findUser.Email, token, "Reset-Password", "Rest pssword", "click on button to Reset your password");

            return true;

        }

        public async Task<string> ResetPassword(RestPasswordDTO restPassword)
        {
            var findUser = await userManager.FindByEmailAsync(restPassword.Email);
            if (findUser is null)
            {
                return null;
            }

            var result = await userManager.ResetPasswordAsync(findUser, restPassword.Token, restPassword.Password);

            if (result.Succeeded)
            {
                return "done";
            }
            return result.Errors.ToList()[0].Description;
        }
        public async Task<bool> ActiveAccount(ActiveAccountDTO accountDTO)
        {
            var findUser = await userManager.FindByEmailAsync(accountDTO.Email);
            if (findUser is null)
            {
                return false;
            }

            var reslt = await userManager.ConfirmEmailAsync(findUser, accountDTO.Token);
            if (reslt.Succeeded)
                return true;

            var token = await userManager.GenerateEmailConfirmationTokenAsync(findUser);
            await SendEmail(findUser.Email, token, "active", "ActiveEmail", "Please active your email, click on button to active");

            return false;
        }

        public async Task<bool> UpdateAddress(string email, Address address)
        {
            var findUser = await userManager.FindByEmailAsync(email);
            if (findUser is null)
            {
                return false;
            }
            var Myaddress = await context.Addresses.AsNoTracking()
                .FirstOrDefaultAsync(m => m.AppUserId == findUser.Id);

            if (Myaddress is null)
            {
                address.AppUserId = findUser.Id;
                await context.Addresses.AddAsync(address);
            }
            else
            {
                context.Entry(Myaddress).State = EntityState.Detached;
                address.Id = Myaddress.Id;
                address.AppUserId = Myaddress.AppUserId;
                context.Addresses.Update(address);

            }
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Address> getUserAddress(string id)
        {
            //var User = await userManager.FindByEmailAsync(email);
            var address = await context.Addresses.FirstOrDefaultAsync(m => m.AppUserId == id);

            return address;
        }
    }
}
