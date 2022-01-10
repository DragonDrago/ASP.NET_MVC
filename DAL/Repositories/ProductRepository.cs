using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly ProductContext db = new ProductContext();

        public IQueryable<Product> GetAll()
        {
            return db.Products;
        }

        public Product GetById(int id)
        {
            return db.Products.SingleOrDefault(e => e.Id == id);
        }

        public void Create(Product entity)
        {
            db.Products.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            Product product = GetById(id);
            db.Products.Remove(product);
            db.SaveChanges();
        }

        public void Update(Product entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
