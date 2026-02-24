using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Supportticketmanagment.Models;

public partial class SupportTicketContext : DbContext
{
    public SupportTicketContext()
    {
    }

    public SupportTicketContext(DbContextOptions<SupportTicketContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketComment> TicketComments { get; set; }

    public virtual DbSet<TicketStatusLog> TicketStatusLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC0721B2A7F3");

            entity.HasIndex(e => e.Name, "UQ__Roles__737584F63E53929D").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tickets__3214EC07A96EFA57");

            entity.Property(e => e.AssignedTo).HasColumnName("Assigned_To");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.CreatedBy).HasColumnName("Created_By");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("MEDIUM");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("OPEN");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.TicketAssignedToNavigations)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK__Tickets__Assigne__4316F928");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TicketCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tickets__Created__4222D4EF");
        });

        modelBuilder.Entity<TicketComment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketCo__3214EC0768942580");

            entity.Property(e => e.Comment).HasColumnType("text");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.TicketId).HasColumnName("Ticket_Id");
            entity.Property(e => e.UserId).HasColumnName("User_Id");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketComments)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TicketCom__Ticke__47DBAE45");

            entity.HasOne(d => d.User).WithMany(p => p.TicketComments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__TicketCom__User___48CFD27E");
        });

        modelBuilder.Entity<TicketStatusLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketSt__3214EC07BD824638");

            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Changed_At");
            entity.Property(e => e.ChangedBy).HasColumnName("Changed_By");
            entity.Property(e => e.NewStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("New_Status");
            entity.Property(e => e.OldStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("Old_Status");
            entity.Property(e => e.TicketId).HasColumnName("Ticket_Id");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.TicketStatusLogs)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("FK__TicketSta__Chang__4D94879B");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketStatusLogs)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TicketSta__Ticke__4CA06362");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0742A463FE");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534D8969E9E").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Created_At");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("Role_Id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__Role_Id__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
