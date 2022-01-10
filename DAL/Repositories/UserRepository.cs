using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
   public class UserRepository : IRepository<User>
    {
        private readonly ProductContext db = new ProductContext();

        public IQueryable<User> GetAll()
        {
            return db.Users;
        }

        public User GetById(int id)
        {
            return db.Users.SingleOrDefault(e => e.Id == id);
        }


        public void Create(User entity)
        {
            db.Users.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            User user = GetById(id);
            db.Users.Remove(user);
            db.SaveChanges();
        }

        

        public void Update(User entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
