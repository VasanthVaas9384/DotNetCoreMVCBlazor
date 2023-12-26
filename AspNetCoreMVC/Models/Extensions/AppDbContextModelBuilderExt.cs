using AspNetCoreMVC.Models.ViewModel;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMVC.Models.Extensions
{
    public static class AppDbContextModelBuilderExt
    {
        public static void seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 2,
                    Name = "John",
                    Email = "John@Test.com",
                    Department = Enums.Department.IT,
                    PhotoPath = ""
                },
                 new Employee
                 {
                     Id = 3,
                     Name = "Josh",
                     Email = "Josh@Test.com",
                     Department = Enums.Department.HR,
                     PhotoPath = ""
                 }
                );
        }
    }
}
