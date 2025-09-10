using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthWebAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthWebAPI.Persistance.Context
{
    public class WebAPIDbContext(DbContextOptions<WebAPIDbContext> options) : DbContext(options)
    {
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ChatData> Chats { get; set; }
        public virtual DbSet<ChatGroup> ChatGroups { get; set; }
        public virtual DbSet<GroupMembership> GroupMemberships { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<ChatData>()
            //.HasOne(m => m.SenderEmail)
            //.WithMany()
            //.HasForeignKey(m => m.SenderId)
            //.OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<ChatData>()
            //.HasOne(m => m.ReceiverEmail)
            //.WithMany()
            //.HasForeignKey(m => m.ReceiverId)
            //.OnDelete(DeleteBehavior.Restrict);
        }
    }


}
