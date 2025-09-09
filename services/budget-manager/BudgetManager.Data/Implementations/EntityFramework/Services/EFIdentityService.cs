using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using BudgetManager.Data.Abstraction.Services;
using BudgetManager.Data.Implementations.EntityFramework.Entities;
using BudgetManager.Data.Models.DomainModels;
using IdentityResult = BudgetManager.Data.Abstraction.Services.IdentityResult;
using SignInResult = BudgetManager.Data.Abstraction.Services.SignInResult;

namespace BudgetManager.Data.Implementations.EntityFramework.Services
{
    public class EFIdentityService : IIdentityService
    {
        private readonly UserManager<EFUserEntity> _userManager;
        private readonly SignInManager<EFUserEntity> _signInManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EFIdentityService(
            UserManager<EFUserEntity> userManager,
            SignInManager<EFUserEntity> signInManager,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityResult> CreateUserAsync(string email, string password, string? firstName = null, string? lastName = null)
        {
            var user = new EFUserEntity
            {
                UserName = email,
                Email = email,
                FirstName = firstName ?? string.Empty,
                LastName = lastName ?? string.Empty
            };

            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
                return IdentityResult.Success();
            
            return IdentityResult.Failed(result.Errors.Select(e => e.Description).ToArray());
        }

        public async Task<SignInResult> SignInAsync(string email, string password, bool rememberMe = false)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            
            if (result.Succeeded)
                return SignInResult.Success();
            
            if (result.IsLockedOut)
                return SignInResult.LockedOut();
            
            return SignInResult.Failed();
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserDM?> GetCurrentUserAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
            return user != null ? _mapper.Map<UserDM>(user) : null;
        }

        public async Task<UserDM?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null ? _mapper.Map<UserDM>(user) : null;
        }

        public async Task<UserDM?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null ? _mapper.Map<UserDM>(user) : null;
        }

        public async Task<bool> CheckPasswordAsync(UserDM user, string password)
        {
            var efUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (efUser == null) return false;
            
            return await _userManager.CheckPasswordAsync(efUser, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(UserDM user)
        {
            var efUser = await _userManager.FindByIdAsync(user.Id.ToString());
            if (efUser == null)
                return IdentityResult.Failed("User not found");
            
            _mapper.Map(user, efUser);
            var result = await _userManager.UpdateAsync(efUser);
            
            if (result.Succeeded)
                return IdentityResult.Success();
            
            return IdentityResult.Failed(result.Errors.Select(e => e.Description).ToArray());
        }
    }
}