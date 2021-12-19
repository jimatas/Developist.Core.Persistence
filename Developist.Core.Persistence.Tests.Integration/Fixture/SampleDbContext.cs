// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.Tests.Integration.Fixture
{
    public class SampleDbContext : DbContext
    {
        public SampleDbContext() : base() { }
        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Person>(entity =>
            {
                entity.ToTable("People");
                entity.Property(p => p.Id).UseIdentityColumn();
                entity.Property(p => p.FamilyName).HasMaxLength(50);
                entity.Property(p => p.GivenName).HasMaxLength(50);
                entity.HasMany(p => p.ReceivedMessages).WithMany(m => m.Recipients).UsingEntity<ReceivedMessage>(
                    x => x.HasOne(y => y.Message).WithMany(),
                    x => x.HasOne(y => y.Recipient).WithMany()
                ).ToTable("ReceivedMessages");
            });

            builder.Entity<Message>(entity =>
            {
                entity.Property(m => m.Id).UseIdentityColumn();
                entity.Property(m => m.Text).HasMaxLength(150);
                entity.HasOne(m => m.Sender).WithMany(p => p.SentMessages).HasForeignKey("SenderId");

            });

            base.OnModelCreating(builder);
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Message> Messages { get; set; }

        internal class ReceivedMessage
        {
            public Person Recipient { get; set; }
            public Message Message { get; set; }
        }
    }
}
