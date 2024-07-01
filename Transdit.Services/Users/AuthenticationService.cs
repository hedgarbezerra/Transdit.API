using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Constants;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Users;
using Transdit.Utilities.Extensions;

namespace Transdit.Services.Users
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUsersService _usersService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly SecurityKey _securityKey;

        public AuthenticationService(UserManager<ApplicationUser> userManager, JwtSettings jwtSettings, IUsersService usersService, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _usersService = usersService;
            _signInManager = signInManager;
            _tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            _securityKey = new SymmetricSecurityKey(key);
        }

        public async Task<AuthenticationResult> Authenticate(Login login)
        {
            var result = new AuthenticationResult();
            var user = await _usersService.Get(login.Email);
            if (user is not null)
            {
                var identityAuthentication = await _signInManager.PasswordSignInAsync(user, login.Password, false, true);
                if (identityAuthentication.Succeeded)
                {
                    var claims = await _usersService.GetClaims(user);
                    var tokenExpiracy = DateTime.Now + _jwtSettings.Validity;

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = tokenExpiracy,
                        NotBefore = DateTime.Now,
                        Issuer = _jwtSettings.Issuer,
                        IssuedAt = DateTime.Now,
                        SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = _tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = _tokenHandler.WriteToken(token);
                    await _userManager.ResetAccessFailedCountAsync(user);
                    result.Data = new JwtTokenResponse(jwtToken, tokenExpiracy);
                    result.Successful = true;
                }
                else if (identityAuthentication.IsLockedOut)
                {
                    var failedCount = await _userManager.GetAccessFailedCountAsync(user);
                    var waitTime = user.LockoutEnd.HasValue ? (user.LockoutEnd.Value - DateTime.UtcNow).Minutes : 0;
                    result.Messages.Add($"Conta bloqueada temporariamente. Foram feitas {failedCount} tentativas de acesso sem sucesso, aguarde até {waitTime} minutos.");
                }
                else
                {
                    var failedCount = await _userManager.GetAccessFailedCountAsync(user);
                    result.Messages.Add($"Usuários ou senha incorretos. Foram feitas {failedCount} tentativas inválidas.");
                }
            }
            else
                result.Messages.Add($"Não foram identificados usuários com o e-mail: {login.Email}");

            return result;
        }

        public bool ValidateToken(string token)
        {
            var isToken = _tokenHandler.CanReadToken(token);
            if (!isToken)
                return false;

            var jwtToken = _tokenHandler.ReadJwtToken(token);

            return jwtToken is not null &&
                jwtToken.Issuer == _jwtSettings.Issuer &&
                jwtToken.ValidTo > DateTime.UtcNow;
        }
    }
}
