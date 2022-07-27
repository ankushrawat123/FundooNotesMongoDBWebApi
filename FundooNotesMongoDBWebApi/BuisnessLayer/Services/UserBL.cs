using BuisnessLayer.Interfaces;
using DatabaseLayer.User;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Services
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL userRL;

        public UserBL(IUserRL userRL)
        {
            this.userRL = userRL;   
        }
        public async Task<User> AddUser(UserModel userModel)
        {
            try
            {

                return await userRL.AddUser(userModel);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> LoginUser(string email, string password)
        {
            try
            {
                return await userRL.LoginUser(email, password);
                
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> ForgetPassword(string email)
        {
            try
            {
                return await userRL.ForgetPassword(email);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> ResetPassword(string email,PasswordPostModel passwordPostModel)
        {
            try
            {
            return await userRL.ResetPassword(email,passwordPostModel);
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
