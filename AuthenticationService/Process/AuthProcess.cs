using AuthenticationService.EntityModel;
using AuthenticationService.Process;
using AuthenticationService.Repository;

using Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AuthenticationService.Process
{
    public class AuthProcess
    {
        private readonly IUserRepository _repository;
        private readonly TokenManager _tokenManager;
        private readonly IHttpContextAccessor _context;

        public AuthProcess(IUserRepository repository, TokenManager mgr, IHttpContextAccessor ctx)
        {
            _repository = repository;
            _tokenManager = mgr;
            _context = ctx;
        }
        public async Task<AuthResponse> ValidateUserAndGenerateResponse(AuthRequest request)
        {
            var user = await _repository.Authenticate(request);
            Role role = null!;
            string token = string.Empty;
            if (user != null)
            {
                role = await _repository.GetRoleForUser(user.UserId);
                token =  _tokenManager.GetJwtToken(user, role);
                return new AuthResponse(user, role, token);
            }
            return null!;
        }
        public async Task<AuthResponse> ValidateTokenAndReturnUser()
        {
           
            var userEmail = _context.HttpContext?.Items["Email"]?.ToString(); 
            if(string.IsNullOrEmpty(userEmail))
            {
                throw new ArgumentException("Invalid token");
            }
            //Based on the email, call the repository to get the user
            var user = await _repository.GetUserByEmail(userEmail);
            if (user != null)
            {
                var role = await _repository.GetRoleForUser(user.UserId);
                
                return new AuthResponse(user, role, "");
            }
            return null!;
        }
    }
}
