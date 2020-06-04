using System;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repository;
        public AuthController(IAuthRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password) {
            
            //validate request

            username = username.ToLower(); //convert the inputted username to lowercase so as to compare with existing usernames for validation

            if (await this.repository.UserExists(username)) // if a user with this username already exists
                return BadRequest("Username already exists"); // return a bad request alert

            var userToCreate = new User {
                Username = username //assigns to the new user the inputted username
            };

            //register the user using the method from the AuthRepository
            var createdUser = await this.repository.Register(userToCreate, password);

            //return a simulation of the success message
            return StatusCode(201);

        }
    }
}