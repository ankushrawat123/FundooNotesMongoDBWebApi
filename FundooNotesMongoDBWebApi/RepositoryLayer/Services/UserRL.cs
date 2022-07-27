using DatabaseLayer.User;
using Experimental.System.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services.Entity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly IConfiguration configuration;
        private readonly IMongoCollection<User> users;

        public UserRL(IConfiguration configuration, IDBSetting dBSetting)
        {
            this.configuration = configuration;
            var UserClient = new MongoClient(dBSetting.ConnectionString);
            var Database = UserClient.GetDatabase(dBSetting.DatabaseName);
            users = Database.GetCollection<User>("users");
        }

        public async Task<User> AddUser(UserModel userModel)
        {
            try
            {
                User user = new User();
                user.FirstName = userModel.FirstName;
                user.LastName = userModel.LastName;
                user.Email = userModel.Email;
                user.Password = PwdEncryptDecryptService.EncryptPassword(userModel.Password);
                user.CreatedDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;

                var userCheck = await users.AsQueryable().Where(x => x.Email == userModel.Email).FirstOrDefaultAsync();


                if (userCheck == null)
                {
                    await this.users.InsertOneAsync(user);

                }
                return user;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<string> LoginUser(string email, string password)
        {
            try
            {
                var pwd = PwdEncryptDecryptService.EncryptPassword(password);
                var userData = await users.Find(x => x.Email == email && x.Password == pwd).FirstOrDefaultAsync();
                var userid = userData.UserId;
                if (userData.Email == email)
                {
                    if (userData != null)
                    {
                        return GenerateJwtToken(email, userid);
                    }
                    throw new Exception("Password is Invalid");
                }
                throw new Exception("Email Doesn't Exists");
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public async Task<bool> ForgetPassword(string Email)
        {
            try
            {
                var check = await users.AsQueryable().Where(x => x.Email == Email).FirstOrDefaultAsync();
                var userid = check.UserId;
                if (check == null)
                {
                    return false;
                }
                else
                {

                    MessageQueue queue;
                    //ADD MESSAGE TO QUEUE
                    if (MessageQueue.Exists(@".\Private$\BookstoreQueue"))
                    {
                        queue = new MessageQueue(@".\Private$\BookstoreQueue");
                    }
                    else
                    {
                        queue = MessageQueue.Create(@".\Private$\BookstoreQueue");
                    }

                    Message MyMessage = new Message();
                    MyMessage.Formatter = new BinaryMessageFormatter();
                    MyMessage.Body = GenerateJwtToken(Email, userid);
                    MyMessage.Label = "Forget Password Email";
                    queue.Send(MyMessage);


                    Message msg = queue.Receive();
                    msg.Formatter = new BinaryMessageFormatter();
                    EmailService.SendEmail(Email, msg.Body.ToString());
                    queue.ReceiveCompleted += new ReceiveCompletedEventHandler(msmqQueue_ReceiveCompleted);
                    queue.BeginReceive();
                    queue.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private void msmqQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                MessageQueue queue = (MessageQueue)sender;
                Message msg = queue.EndReceive(e.AsyncResult);
                EmailService.SendEmail(e.Message.ToString(), GenerateToken(e.Message.ToString()));
                queue.BeginReceive();

            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode ==
                    MessageQueueErrorCode.AccessDenied)
                {
                    Console.WriteLine("Access is denied. " +
                        "Queue might be a system queue.");
                }
            }
        }

        private string GenerateToken(string Email)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("this is my secret key");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Email,Email)

                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string GenerateJwtToken(string email, object userId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("this is my secret key");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Email, email),
                    new Claim("UserId", userId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> ResetPassword(string email,PasswordPostModel passwordPostModel)
        {
            try
            {
                var userCheck = users.AsQueryable().Where(x => x.Email == email).FirstOrDefaultAsync();
                if (userCheck == null)
                {
                    return false;
                }
                if (passwordPostModel.ConfirmPassword == passwordPostModel.Password)
                { 
                    string cnfpwd = PwdEncryptDecryptService.EncryptPassword(passwordPostModel.Password);
                    await users.UpdateOneAsync(x=>x.Email==email,
                        Builders<User>.Update.Set(x => x.Password, cnfpwd));       
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
    
}
