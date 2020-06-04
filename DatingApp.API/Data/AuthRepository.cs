using System;
using System.Threading.Tasks;
using DatingApp.API.Controllers;
using DatingApp.API.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;

        public AuthRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<User> Login(User user, string password)
        {
            var user1 = await this.context.Users.FirstOrDefaultAsync(x => x.Username == user.Username);

            if (user == null)  //no user found in the database
                return null;

            //if a user with the given username has been found, verify that the password created is the right one
            if (!VerifyPasswordHash(password, user1.PasswordHash, user1.PasswordSalt)) //wrong password entered
                return null;

                return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) //we are passing passwordSalt (the key to the password)
            {

                //this returns our password as an array of bytes
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                //loop over the byte array to verify the validity of the bytes in the hashcode of the entered password and the one in the database
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])  //if an element doesn't match, return false
                        return false;
                }
            }
            return false;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt); //"out" keyword indicates that 
                                                                              //we are passing these variables by references
                                                                              //instead of by value                    

            
            user.PasswordHash = passwordHash;   //modified inside the createPasswordHash method
             user.PasswordSalt = passwordSalt;    //modified inside the createPasswordHash method
            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();

            return user;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {

            //to ensure this method is called when we get the hmac class instance, we use "using" keyword
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {

                //creating a key that we can use to authenticate/find the real password in the database
                passwordSalt = hmac.Key;

                //this returns our password as an array of bytes
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await this.context.Users.AnyAsync(x => x.Username == username))
                return true;

            return false;
        }
    }
}