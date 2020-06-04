using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repository;
        private readonly IConfiguration config;
        public AuthController(IAuthRepository repository, IConfiguration config)
        {
            this.config = config;
            this.repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
        {

            //validate request

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower(); //convert the inputted username to lowercase so as to compare with existing usernames for validation

            if (await this.repository.UserExists(userForRegisterDto.Username)) // if a user with this username already exists
                return BadRequest("Username already exists"); // return a bad request alert

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username //assigns to the new user the inputted username
            };

            //register the user using the method from the AuthRepository
            var createdUser = await this.repository.Register(userToCreate, userForRegisterDto.Password);

            //return a simulation of the success message
            return StatusCode(201);

        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {

            var userFromRepo = await this.repository.Login(userForLoginDto.Username.ToLower, userForLoginDto.Password);

            if (userFromRepo == null) //user not found
                return Unauthorized();
            
            //FROM HERE, WE START BUILDING THE TOKEN
            //Build up a token that contains the user's id and user's username
            //it will validate by the Id and the Username of the logging user
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()), //correlate to the Id of the user
                new Claim(ClaimTypes.Name, userFromRepo.Username)        //correlates to the name of the user
            };

            //TO MAKE SURE THAT THE TOKEN IS A VALID TOKEN, THE SERVER NEEDS TO SIGN THE TOKEN
            //SO WE CREATE A SECURITY KEY AND WE USE THIS KEY AS A PART OF THE SIGNING CREDENTIALS
            //AND ENCRYPTING THIS KEY USING A HASHING ALGORITHM
            //key to sign our token (this key is hashed)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config.GetSection("AppSetting:Token").Value));

            //the signing credentials that we store using the key
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);


            //WE PASS OUR CLAIMS AS THE SUBJECT, SET AN EXPIRY DATE, AND PASS THE SIGNING CREDENTIALS
            //Descriptor containing the expiry date for the token and the signing credentials
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                UnhandledExceptionEventArgs = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            }

            //Token Handler that can CREATE tokens and pass in the Token Descriptor
            var tokenHandler = new JwtSecurityTokenHandler();

            //We created a JWT token that we want to return to the client as an object
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }


    }
}