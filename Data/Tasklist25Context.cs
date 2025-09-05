using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TaskList_Server.Models;

namespace TaskList_Server.Data;

public partial class Tasklist25Context : DbContext
{
    public Tasklist25Context()
    {
    }

    public Tasklist25Context(DbContextOptions<Tasklist25Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Priority> Priorities { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<TaskList_Server.Models.Task> Tasks { get; set; }

    public virtual DbSet<TasksOld> TasksOlds { get; set; }

    public virtual DbSet<TblAdmin> TblAdmins { get; set; }

    public virtual DbSet<TblApplication> TblApplications { get; set; } 

    public virtual DbSet<TblCustomer> TblCustomers { get; set; }

    public virtual DbSet<TblPermission> TblPermissions { get; set; }

    public virtual DbSet<TblUploadedFile> TblUploadedFiles { get; set; }

    public virtual DbSet<TraceMarch> TraceMarches { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskList_Server.Models.Task>()
        .ToTable("Tasks");

        modelBuilder.Entity<TblCustomer>()
            .ToTable("tblCustomer");

        modelBuilder.Entity<Status>()
            .ToTable("Status");  

        modelBuilder.Entity<Priority>()
            .ToTable("Priority"); 

        modelBuilder.Entity<TblApplication>()
            .ToTable("tblApplication");

        modelBuilder.Entity<TblPermission>()
            .ToTable("tblPermission");
    }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Server=(local);Database=tasklist_25;Trusted_Connection=True;TrustServerCertificate=True;");

    //    protected override void OnModelCreating(ModelBuilder modelBuilder)
    //    {
    //        modelBuilder.UseCollation("Finnish_Swedish_CI_AS");

    //        modelBuilder.Entity<Priority>(entity =>
    //        {
    //            entity.HasKey(e => e.PriorityId).HasName("PK_PriorityCodes");

    //            entity.ToTable("Priority");

    //            entity.Property(e => e.Name).HasMaxLength(50);
    //        });

    //        modelBuilder.Entity<Status>(entity =>
    //        {
    //            entity.HasKey(e => e.StatusId).HasName("PK_StatusCodes");

    //            entity.ToTable("Status");

    //            entity.Property(e => e.Name).HasMaxLength(50);
    //        });

    //        modelBuilder.Entity<Task>(entity =>
    //        {
    //            entity.Property(e => e.IntDisplayNo).HasColumnName("intDisplayNo");
    //            entity.Property(e => e.LastChangeDate).HasColumnType("datetime");
    //            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
    //            entity.Property(e => e.SeriousBug).HasDefaultValue(false);
    //            entity.Property(e => e.SmallBug).HasDefaultValue(false);
    //        });

    //        modelBuilder.Entity<TasksOld>(entity =>
    //        {
    //            entity
    //                .HasNoKey()
    //                .ToTable("Tasks_OLD");

    //            entity.Property(e => e.ApplicationName).HasMaxLength(200);
    //            entity.Property(e => e.DelegatedTo).HasMaxLength(200);
    //            entity.Property(e => e.Description).HasMaxLength(2000);
    //            entity.Property(e => e.LastChangeDate).HasColumnType("datetime");
    //            entity.Property(e => e.RegistrationDate).HasColumnType("datetime");
    //        });

    //        modelBuilder.Entity<TblAdmin>(entity =>
    //        {
    //            entity.HasKey(e => e.IntId);

    //            entity.ToTable("tblAdmin");

    //            entity.Property(e => e.IntId).HasColumnName("intId");
    //            entity.Property(e => e.ChrFirstName)
    //                .HasMaxLength(50)
    //                .IsUnicode(false)
    //                .HasColumnName("chrFirstName");
    //            entity.Property(e => e.ChrLastName)
    //                .HasMaxLength(50)
    //                .IsUnicode(false)
    //                .HasColumnName("chrLastName");
    //            entity.Property(e => e.ChrPassword)
    //                .HasMaxLength(50)
    //                .IsUnicode(false)
    //                .HasColumnName("chrPassword");
    //            entity.Property(e => e.ChrUserName)
    //                .HasMaxLength(50)
    //                .IsUnicode(false)
    //                .HasColumnName("chrUserName");
    //            entity.Property(e => e.DtLastLogin)
    //                .HasColumnType("datetime")
    //                .HasColumnName("dtLastLogin");
    //        });

    //        modelBuilder.Entity<TblApplication>(entity =>
    //        {
    //            entity.HasKey(e => e.IntId);

    //            entity.ToTable("tblApplication");

    //            entity.Property(e => e.IntId).HasColumnName("intId");
    //            entity.Property(e => e.ChrApplicationName)
    //                .HasMaxLength(50)
    //                .IsUnicode(false)
    //                .HasColumnName("chrApplicationName");
    //            entity.Property(e => e.IntCustomerId).HasColumnName("intCustomerId");
    //        });

    //        modelBuilder.Entity<TblCustomer>(entity =>
    //        {
    //            entity.HasKey(e => e.IntId);

    //            entity.ToTable("tblCustomer");

    //            entity.Property(e => e.IntId).HasColumnName("intId");
    //            entity.Property(e => e.ChrCustomerCode)
    //                .HasMaxLength(50)
    //                .IsUnicode(false)
    //                .HasColumnName("chrCustomerCode");
    //            entity.Property(e => e.ChrCustomerName)
    //                .HasMaxLength(50)
    //                .IsUnicode(false)
    //                .HasColumnName("chrCustomerName");
    //        });

    //        modelBuilder.Entity<TblPermission>(entity =>
    //        {
    //            entity.HasKey(e => e.IntId);

    //            entity.ToTable("tblPermission");

    //            entity.Property(e => e.IntId).HasColumnName("intId");
    //            entity.Property(e => e.ChrPermission)
    //                .HasMaxLength(50)
    //                .IsUnicode(false)
    //                .HasColumnName("chrPermission");
    //        });

    //        modelBuilder.Entity<TblUploadedFile>(entity =>
    //        {
    //            entity.HasKey(e => e.IntId);

    //            entity.ToTable("tblUploadedFiles");

    //            entity.Property(e => e.IntId).HasColumnName("intId");
    //            entity.Property(e => e.ChrOriginalFileName)
    //                .HasMaxLength(255)
    //                .HasColumnName("chrOriginalFileName");
    //            entity.Property(e => e.ChrSavedFileName)
    //                .HasMaxLength(255)
    //                .HasColumnName("chrSavedFileName");
    //            entity.Property(e => e.IntTaskId).HasColumnName("intTaskId");
    //        });

    //        modelBuilder.Entity<TraceMarch>(entity =>
    //        {
    //            entity.HasKey(e => e.RowNumber).HasName("PK__TraceMar__AAAC09D82D27B809");

    //            entity.ToTable("TraceMarch");

    //            entity.Property(e => e.ApplicationName).HasMaxLength(128);
    //            entity.Property(e => e.BinaryData).HasColumnType("image");
    //            entity.Property(e => e.ClientProcessId).HasColumnName("ClientProcessID");
    //            entity.Property(e => e.Cpu).HasColumnName("CPU");
    //            entity.Property(e => e.DatabaseId).HasColumnName("DatabaseID");
    //            entity.Property(e => e.DatabaseName).HasMaxLength(128);
    //            entity.Property(e => e.EndTime).HasColumnType("datetime");
    //            entity.Property(e => e.GroupId).HasColumnName("GroupID");
    //            entity.Property(e => e.HostName).HasMaxLength(128);
    //            entity.Property(e => e.LoginName).HasMaxLength(128);
    //            entity.Property(e => e.LoginSid).HasColumnType("image");
    //            entity.Property(e => e.NtdomainName)
    //                .HasMaxLength(128)
    //                .HasColumnName("NTDomainName");
    //            entity.Property(e => e.NtuserName)
    //                .HasMaxLength(128)
    //                .HasColumnName("NTUserName");
    //            entity.Property(e => e.ObjectName).HasMaxLength(128);
    //            entity.Property(e => e.RequestId).HasColumnName("RequestID");
    //            entity.Property(e => e.ServerName).HasMaxLength(128);
    //            entity.Property(e => e.SessionLoginName).HasMaxLength(128);
    //            entity.Property(e => e.Spid).HasColumnName("SPID");
    //            entity.Property(e => e.StartTime).HasColumnType("datetime");
    //            entity.Property(e => e.TextData).HasColumnType("ntext");
    //            entity.Property(e => e.TransactionId).HasColumnName("TransactionID");
    //        });

    //        modelBuilder.Entity<User>(entity =>
    //        {
    //            entity.Property(e => e.BitShowUser).HasColumnName("bitShowUser");
    //            entity.Property(e => e.Email)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);
    //            entity.Property(e => e.FirstName)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);
    //            entity.Property(e => e.IntCustomerId).HasColumnName("intCustomerId");
    //            entity.Property(e => e.LastName)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);
    //            entity.Property(e => e.Maccess).HasColumnName("MAccess");
    //            entity.Property(e => e.PassWord)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);
    //            entity.Property(e => e.UserName)
    //                .HasMaxLength(50)
    //                .IsUnicode(false);
    //        });

    //        OnModelCreatingPartial(modelBuilder);
    //    }

    //    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
