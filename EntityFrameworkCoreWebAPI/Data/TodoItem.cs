using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCoreWebAPI.Data
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public bool IsComplete { get; set; }

    }

    public class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    }
}
