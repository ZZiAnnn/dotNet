using Microsoft.EntityFrameworkCore;

namespace ExamSchedulingSystem.db
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options)
            : base(options)
        { }

        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<Exam> Exam { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<ExamRoom> ExamRoom { get; set; }
    }
}
