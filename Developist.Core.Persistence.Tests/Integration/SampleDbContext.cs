using Developist.Core.Persistence.Tests.Fixture;

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.Tests
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext() : base() { }
        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Book>();

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("People");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.FamilyName).HasMaxLength(50);
                entity.Property(p => p.GivenName).HasMaxLength(50);
                entity.HasMany(p => p.ReceivedMessages).WithMany(m => m.Recipients).UsingEntity<ReceivedMessage>(
                    x => x.HasOne(y => y.Message).WithMany(),
                    x => x.HasOne(y => y.Recipient).WithMany()
                ).ToTable("ReceivedMessages");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Messages");
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Text).HasMaxLength(150);
                entity.HasOne(m => m.Sender).WithMany(p => p.SentMessages).HasForeignKey("SenderId");

            });

            base.OnModelCreating(modelBuilder);
        }

        internal class ReceivedMessage
        {
            public Person? Recipient { get; set; }
            public Message? Message { get; set; }
        }
    }
}
