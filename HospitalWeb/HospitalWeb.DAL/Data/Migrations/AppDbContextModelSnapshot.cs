﻿// <auto-generated />
using System;
using HospitalWeb.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HospitalWeb.DAL.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AddressId"), 1L, 1);

                    b.Property<string>("FullAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LocalityId")
                        .HasColumnType("int");

                    b.HasKey("AddressId");

                    b.HasIndex("LocalityId");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Appointment", b =>
                {
                    b.Property<int>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AppointmentId"), 1L, 1);

                    b.Property<DateTime>("AppointmentDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DiagnosisId")
                        .HasColumnType("int");

                    b.Property<string>("DoctorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PatientId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Prescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.HasKey("AppointmentId");

                    b.HasIndex("DiagnosisId");

                    b.HasIndex("DoctorId");

                    b.HasIndex("PatientId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Diagnosis", b =>
                {
                    b.Property<int>("DiagnosisId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DiagnosisId"), 1L, 1);

                    b.Property<string>("DiagnosisName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DiagnosisId");

                    b.ToTable("Diagnoses");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Grade", b =>
                {
                    b.Property<int>("GradeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GradeId"), 1L, 1);

                    b.Property<string>("AuthorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Stars")
                        .HasColumnType("int");

                    b.Property<string>("TargetId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("GradeId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TargetId");

                    b.ToTable("Grades");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Hospital", b =>
                {
                    b.Property<int>("HospitalId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("HospitalId"), 1L, 1);

                    b.Property<int>("AddressId")
                        .HasColumnType("int");

                    b.Property<string>("HospitalName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("HospitalId");

                    b.HasIndex("AddressId");

                    b.ToTable("Hospitals");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Identity.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("CalendarId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<byte[]>("Image")
                        .HasColumnType("varbinary(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Locality", b =>
                {
                    b.Property<int>("LocalityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LocalityId"), 1L, 1);

                    b.Property<string>("LocalityName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LocalityId");

                    b.ToTable("Localities");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Meeting", b =>
                {
                    b.Property<int>("MeetingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MeetingId"), 1L, 1);

                    b.Property<int>("AppointmentId")
                        .HasColumnType("int");

                    b.Property<int>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("JoinLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Topic")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MeetingId");

                    b.HasIndex("AppointmentId");

                    b.ToTable("Meetings");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Notification", b =>
                {
                    b.Property<int>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NotificationId"), 1L, 1);

                    b.Property<string>("AppUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Topic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("NotificationId");

                    b.HasIndex("AppUserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Schedule", b =>
                {
                    b.Property<int>("ScheduleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ScheduleId"), 1L, 1);

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("int");

                    b.Property<string>("DoctorId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("ScheduleId");

                    b.HasIndex("DoctorId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Specialty", b =>
                {
                    b.Property<int>("SpecialtyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SpecialtyId"), 1L, 1);

                    b.Property<string>("SpecialtyName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SpecialtyId");

                    b.ToTable("Specialties");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Identity.Admin", b =>
                {
                    b.HasBaseType("HospitalWeb.DAL.Entities.Identity.AppUser");

                    b.Property<bool>("IsSuperAdmin")
                        .HasColumnType("bit");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Identity.Doctor", b =>
                {
                    b.HasBaseType("HospitalWeb.DAL.Entities.Identity.AppUser");

                    b.Property<int>("HospitalId")
                        .HasColumnType("int");

                    b.Property<int>("SpecialtyId")
                        .HasColumnType("int");

                    b.HasIndex("HospitalId");

                    b.HasIndex("SpecialtyId");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Identity.Patient", b =>
                {
                    b.HasBaseType("HospitalWeb.DAL.Entities.Identity.AppUser");

                    b.Property<int>("AddressId")
                        .HasColumnType("int");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Sex")
                        .HasColumnType("int");

                    b.HasIndex("AddressId");

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Address", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Locality", "Locality")
                        .WithMany("Addresses")
                        .HasForeignKey("LocalityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Locality");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Appointment", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Diagnosis", "Diagnosis")
                        .WithMany("Appointments")
                        .HasForeignKey("DiagnosisId");

                    b.HasOne("HospitalWeb.DAL.Entities.Identity.Doctor", "Doctor")
                        .WithMany("Appointments")
                        .HasForeignKey("DoctorId");

                    b.HasOne("HospitalWeb.DAL.Entities.Identity.Patient", "Patient")
                        .WithMany("Appointments")
                        .HasForeignKey("PatientId");

                    b.Navigation("Diagnosis");

                    b.Navigation("Doctor");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Grade", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Identity.Patient", "Author")
                        .WithMany("Grades")
                        .HasForeignKey("AuthorId");

                    b.HasOne("HospitalWeb.DAL.Entities.Identity.Doctor", "Target")
                        .WithMany("Grades")
                        .HasForeignKey("TargetId");

                    b.Navigation("Author");

                    b.Navigation("Target");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Hospital", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Address", "Address")
                        .WithMany("Hospitals")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Address");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Meeting", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Appointment", "Appointment")
                        .WithMany("Meetings")
                        .HasForeignKey("AppointmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Appointment");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Notification", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Identity.AppUser", "AppUser")
                        .WithMany("Notifications")
                        .HasForeignKey("AppUserId");

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Schedule", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Identity.Doctor", "Doctor")
                        .WithMany("Schedules")
                        .HasForeignKey("DoctorId");

                    b.Navigation("Doctor");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HospitalWeb.DAL.Entities.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Identity.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Identity.Admin", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Identity.AppUser", null)
                        .WithOne()
                        .HasForeignKey("HospitalWeb.DAL.Entities.Identity.Admin", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Identity.Doctor", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Hospital", "Hospital")
                        .WithMany("Doctors")
                        .HasForeignKey("HospitalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HospitalWeb.DAL.Entities.Identity.AppUser", null)
                        .WithOne()
                        .HasForeignKey("HospitalWeb.DAL.Entities.Identity.Doctor", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("HospitalWeb.DAL.Entities.Specialty", "Specialty")
                        .WithMany("Doctors")
                        .HasForeignKey("SpecialtyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hospital");

                    b.Navigation("Specialty");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Identity.Patient", b =>
                {
                    b.HasOne("HospitalWeb.DAL.Entities.Address", "Address")
                        .WithMany("Patients")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HospitalWeb.DAL.Entities.Identity.AppUser", null)
                        .WithOne()
                        .HasForeignKey("HospitalWeb.DAL.Entities.Identity.Patient", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("Address");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Address", b =>
                {
                    b.Navigation("Hospitals");

                    b.Navigation("Patients");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Appointment", b =>
                {
                    b.Navigation("Meetings");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Diagnosis", b =>
                {
                    b.Navigation("Appointments");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Hospital", b =>
                {
                    b.Navigation("Doctors");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Identity.AppUser", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Locality", b =>
                {
                    b.Navigation("Addresses");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Specialty", b =>
                {
                    b.Navigation("Doctors");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Identity.Doctor", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("Grades");

                    b.Navigation("Schedules");
                });

            modelBuilder.Entity("HospitalWeb.DAL.Entities.Identity.Patient", b =>
                {
                    b.Navigation("Appointments");

                    b.Navigation("Grades");
                });
#pragma warning restore 612, 618
        }
    }
}
