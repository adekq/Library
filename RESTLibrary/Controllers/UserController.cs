using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JwtSecurity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RESTLibrary.Models;

namespace RESTLibrary.Controllers
{
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly IJwtTokenGenerator jwtTokenGenerator;
        private readonly IUserService userService;        

        public UserController(ILogger<UserController> logger, IJwtTokenGenerator jwtTokenGenerator, IUserService userService)
        {
            this.logger = logger;            
            this.jwtTokenGenerator = jwtTokenGenerator;
            this.userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!userService.ContainUser(request.Email, request.Password))
            {
                return Unauthorized();
            }

            var role = userService.UserRole(request.Email);

            var jwtToken = jwtTokenGenerator.GenerateJwtToken(request.Email, Enum.GetName(typeof(Role), role));

            return Ok(new LoginResponse
            {
                Email = request.Email,
                Role = role,
                JwtToken = jwtToken                
            });
        }

        public class LoginRequest
        {
            [Required]
            [JsonPropertyName("email")]
            public string Email { get; set; }
            
            [Required]
            [JsonPropertyName("password")]
            public string Password { get; set; }
        }

        public class LoginResponse
        {
            [Required]
            [JsonPropertyName("email")]
            public string Email { get; set; }

            [Required]
            [JsonPropertyName("role")]
            public Role Role { get; set; }

            [Required]
            [JsonPropertyName("jwtToken")]
            public string JwtToken { get; set; }
        }


        [Authorize(Roles = nameof(Role.Librarian))]
        [HttpPost("add/user")]
        public ActionResult<AddUserResponse> AddUser([FromBody] AddUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var userAdded = userService.AddUser(request.User);
            return Ok(new AddUserResponse { UserAdded = userAdded });
        }

        public class AddUserRequest
        {
            public User User { get; set; }
        }

        public class AddUserResponse
        {
            public bool UserAdded { get; set; }
        }
    }
}