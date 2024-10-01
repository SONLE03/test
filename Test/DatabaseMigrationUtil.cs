using Microsoft.EntityFrameworkCore;

namespace Test
{
    public class DatabaseMigrationUtil
    {
        public static void DataBaseMigrationInstallation(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.ServiceProvider.GetService<ApplicationDBContext>()
                    .Database.Migrate();

            }
        }
    }
}
