using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using ChatService.Domain;
using MassTransit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ConversationUser> ConversationUsers { get; set; }
        public DbSet<ConversationMessage> ConversationMessages { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

            modelBuilder.AddQuartz(builder => builder.UsePostgreSql());

            modelBuilder.Entity<User>(m =>
            {
                m.HasAlternateKey(x => x.Email);
            });

            modelBuilder.Entity<ConversationUser>(m =>
            {
                m.HasIndex(x => new { x.ConversationId, x.UserId })
                    .IsUnique();
            });

            modelBuilder.Entity<ChatGroup>(m =>
            {
                m.HasOne(x => x.CreatedBy)
                    .WithMany(x => x.ChatGroups)
                    .HasForeignKey(x => x.CreatedById);
            });

            modelBuilder.Entity<Conversation>(m =>
            {
                m.HasOne(x => x.ChatGroup)
                    .WithOne(x => x.Conversation)
                    .HasForeignKey<ChatGroup>(x => x.ConversationId);
            });
        }
    }
}
