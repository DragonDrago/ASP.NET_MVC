using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class CategoryRepository : IRepository<Category>
    {
        private readonly ProductContext db = new ProductContext();

        public IQueryable<Category> GetAll()
        {
            return db.Categories;
        }

        public Category GetById(int id)
        {
            return db.Categories.SingleOrDefault(e => e.Id == id);
        }


        public void Create(Category entity)
        {
            db.Categories.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            Category category = GetById(id);
            db.Categories.Remove(category);
            db.SaveChanges();
        }

        public void Update(Category entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
