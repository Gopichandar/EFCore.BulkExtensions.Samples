using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EFCore.BulkExtensions.Samples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var context = new CompanyContext())
            {
                context.Database.Migrate();
                var cust = new Customer()
                {
                    Name = "Kayle"
                };

                context.Customers.Add(cust);
                context.SaveChanges();
            }


            var customers = new List<Customer>();
            customers.Add(new Customer() { Name = "John" });
            customers.Add(new Customer() { Name = "Smith" });
            customers.Add(new Customer() { Name = "Kayle" });


            using (var context = new CompanyContext())
            {
                context.BulkInsertOrUpdate(customers, b =>
                {
                    b.UpdateByProperties = new List<string> { nameof(Customer.Name) };
                });
            }

            //I'm expecting the following output as a response in customer;  How to achieve this? Any Idea

            Console.WriteLine($"{customers[0].Id}, {customers[0].Name}");    //   2, John
            Console.WriteLine($"{customers[1].Id}, {customers[1].Name}");    //   3, Smith
            Console.WriteLine($"{customers[2].Id}, {customers[2].Name}");    //   1, Kayle
        }
    }

    public class CompanyContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=sample.db");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Customer>()
                .HasIndex(u => u.Name)
                .IsUnique();
        }
    }

    //Model
    public class Customer
    {
        public int Id { get; set; }        
        public string Name { get; set; }
    }
}
