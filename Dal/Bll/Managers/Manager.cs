using System;
using Microsoft.EntityFrameworkCore;

namespace Threesixty.Dal.Bll.Managers
{
    public abstract class Manager
    {
        protected DbContextOptions<ThreesixtyContext> DbOptions { get; }

        protected Manager(DbContextOptions<ThreesixtyContext> options)
        {
            DbOptions = options;
        }

        public T ExecuteDb<T>(Func<ThreesixtyContext, T> invoke)
        {
            T result;
            using (var context = new ThreesixtyContext(DbOptions))
            {
                using (var tr = context.Database.BeginTransaction())
                {
                    result = invoke.Invoke(context);
                    context.SaveChanges();
                    tr.Commit();
                }
            }

            return result;
        }
    }
}
