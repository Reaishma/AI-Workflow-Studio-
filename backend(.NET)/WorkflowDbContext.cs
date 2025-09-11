using Microsoft.EntityFrameworkCore;
using WorkflowApi.Models;

namespace WorkflowApi.Data
{
    public class WorkflowDbContext : DbContext
    {
        public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Workflow> Workflows { get; set; }
        public DbSet<WorkflowExecution> WorkflowExecutions { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            
            // Workflow configuration
            modelBuilder.Entity<Workflow>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.UserId, e.Name });
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Definition).HasColumnType("TEXT");
                
                entity.HasOne(w => w.User)
                      .WithMany(u => u.Workflows)
                      .HasForeignKey(w => w.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // WorkflowExecution configuration
            modelBuilder.Entity<WorkflowExecution>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.WorkflowId, e.StartedAt });
                entity.Property(e => e.StartedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.Input).HasColumnType("TEXT");
                entity.Property(e => e.Output).HasColumnType("TEXT");
                entity.Property(e => e.ExecutionLog).HasColumnType("TEXT");
                
                entity.HasOne(e => e.Workflow)
                      .WithMany(w => w.Executions)
                      .HasForeignKey(e => e.WorkflowId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}