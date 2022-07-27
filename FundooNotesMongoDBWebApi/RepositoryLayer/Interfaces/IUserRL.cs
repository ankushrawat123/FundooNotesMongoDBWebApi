using DatabaseLayer.User;
using RepositoryLayer.Services.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Interfaces
{
    public interface IUserRL
    {
        Task<User> AddUser(UserModel userModel);
        Task<string> LoginUser(string email,string password);
        Task<bool> ForgetPassword(string email);
        Task<bool> ResetPassword(string email,PasswordPostModel passwordPostModel);
    }
}
