using BuisnessLayer.Interfaces;
using BuisnessLayer.Services;
using DatabaseLayer.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using RepositoryLayer.Services.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FundooNotesMongoDBWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserBL userBL;
        private readonly IConfiguration configuration;
        private readonly IMongoCollection<User> users;
        public UserController(IUserBL userBL,IConfiguration configuration,IDBSetting dBSetting)
        {
            this.userBL = userBL;
            this.configuration = configuration;
            var MongoClient = new MongoClient(dBSetting.ConnectionString);
            var Database = MongoClient.GetDatabase(dBSetting.DatabaseName);
            users = Database.GetCollection<User>("users");
        }

        [HttpPost]
        [Route("AddUsers")]

        public async Task<IActionResult> AddUsers(UserModel userModel)
        {
            try
            {
                var userCheck = await userBL.AddUser(userModel);
                if (userCheck != null)
                {
                    return Ok(new { Status = true, message = "User Added Successfully" });
                }
                else
                    return BadRequest(new { Status = false, Message = "User Not Registered successfully " });
            }
            catch(Exception e)
            {
               return NotFound(new {Status=false,message=e.Message});
            }
        }

        [HttpPost]
        [Route("LoginUser/{Email}/{Password}")]
        public async Task<IActionResult> LoginUser(string Email,string Password)
        {
            try
            {
                var pwd = PwdEncryptDecryptService.EncryptPassword(Password);
                var userData = await users.AsQueryable().Where(x => x.Email == Email && x.Password == pwd).FirstOrDefaultAsync();
                if (userData != null)
                {
                    string token = await userBL.LoginUser(Email, Password);
                    return Ok(new { Status = true, Message = "Login is Successfull",data=token });
                }
                else
                {
                    return BadRequest(new { Status = false, Message = "Login got Failed" });
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        [Route("ForgetPassword/{Email}")]

        public async Task<IActionResult> ForgetPassword(string Email)
        {
            try
            {
                    var user = await userBL.ForgetPassword(Email);
                    if (user == true)
                    {
                        return Ok(new { Status = true, Message = "Forgot Password" });
                    }
                    else
                        return BadRequest(new { Status = false, Message = "Invalid Email" });
            }
            catch(Exception e)
            {
                return NotFound(new {status=false, message=e.Message});
            }
        }


        [HttpPut]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword(PasswordPostModel passwordPostModel)
        {
            try
            {

                var UserId = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("UserId", StringComparison.InvariantCultureIgnoreCase));
                string UserID = UserId.Value;

                var userdata = await users.AsQueryable().Where(x => x.UserId == UserID).FirstOrDefaultAsync();
                string email= userdata.Email;
                if(UserID!=null)
                {
                    if (passwordPostModel.Password == passwordPostModel.ConfirmPassword)
                    {
                        await userBL.ResetPassword(email,passwordPostModel);
                        return Ok(new { status = true, Message = "Password Updated Successfully" });
                    }
                    else
                        return BadRequest(new { status = false, Message = "Password and Confirmed Password Must be same" });
                }
                else
                    return BadRequest(new { status = false, Message = "Userid Not Found" });
            }
            catch(Exception e)
            {
                return NotFound(new { status = false, message = e.Message });
            }
        }
    }
}
