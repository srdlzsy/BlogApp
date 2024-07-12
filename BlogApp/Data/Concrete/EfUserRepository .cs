using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete
{

    public class EfUserRepository : IUserRepository
    {
        readonly BlogContext _Context;
        public EfUserRepository(BlogContext Context)
        {
           _Context = Context;
        }
        public IQueryable<User> Users => _Context.Users;

        public void CreateUser(User user)
        {
           _Context.Users.Add(user);
            _Context.SaveChanges();
        }
    }
}