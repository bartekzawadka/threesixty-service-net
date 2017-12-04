using Threesixty.Dal.Dll;

namespace ThreesixtyService
{
    public static class DbInitializer
    {
        public static void Initialize(ThreesixtyContext context)
        {
            context.Database.EnsureCreated();
            context.SaveChanges();
        }
    }
}
