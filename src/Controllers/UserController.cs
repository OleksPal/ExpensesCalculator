using ExpensesCalculator.Dtos.User;
using ExpensesCalculator.Models;
using ExpensesCalculator.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpensesCalculator.Controllers
{
    [Route("/user")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!TryValidateModel(registerDto))
                    return BadRequest(ModelState);

                var newUser = new User
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(newUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(newUser, "User");

                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                UserName = newUser.UserName,
                                Email = newUser.Email
                            }
                        );
                    }
                    return StatusCode(500, roleResult.Errors);
                }

                return StatusCode(500, createdUser.Errors);
            }
            catch (DbUpdateException)
            {
                var emailError = IdentityResult.Failed(
                    new IdentityError[]
                    {
                        new IdentityError
                        {
                            Code = "DuplicateEmail",
                            Description = $"Email '{registerDto.Email}' is already taken."
                        }
                    }
                );
                return StatusCode(500, emailError.Errors);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("loginWithUsername")]
        public IActionResult LoginWithUsername()
        {
            return View();
        }

        [HttpGet("getLoginPartialView")]
        public IActionResult GetLoginPartialView()
        {
            return PartialView("_LoginPartial");
        }

        [HttpPost("loginWithUsername")]
        public async Task<IActionResult> LoginWithUsername([FromBody] LoginWithUsernameDto loginDto)
        {
            if (!TryValidateModel(loginDto))
                return BadRequest(ModelState);

            var user = await GetUserByUsername(loginDto.UserName);

            if (user is null)
                return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized("UserName not found and/or password is wrong");
            }

            return Ok(
                new NewUserDto
                {
                    UserName = loginDto.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
            );
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Index), "Home");
        }

        protected virtual async Task<User> GetUserByUsername(string userName)
        {
            return await _userManager.Users.FirstOrDefaultAsync(user => user.UserName == userName.ToLower());
        }
    }
}
