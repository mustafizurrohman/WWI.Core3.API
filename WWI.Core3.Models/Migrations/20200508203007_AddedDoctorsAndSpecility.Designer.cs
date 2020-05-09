﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WWI.Core3.Models.DatabaseContext;

namespace WWI.Core3.Models.Migrations
{
    [DbContext(typeof(DocAppointmentContext))]
    [Migration("20200508203007_AddedDoctorsAndSpecility")]
    partial class AddedDoctorsAndSpecility
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WWI.Core3.Models.DbContext.Speciality", b =>
                {
                    b.Property<int>("SpecialityID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.HasKey("SpecialityID");

                    b.ToTable("Specialities");
                });

            modelBuilder.Entity("WWI.Core3.Models.Doctor", b =>
                {
                    b.Property<int>("DoctorID")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Firstname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Lastname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Middlename")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SpecialityID")
                        .HasColumnType("int");

                    b.HasKey("DoctorID");

                    b.HasIndex("SpecialityID");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("WWI.Core3.Models.Doctor", b =>
                {
                    b.HasOne("WWI.Core3.Models.DbContext.Speciality", "Speciality")
                        .WithMany("Doctors")
                        .HasForeignKey("SpecialityID");
                });
#pragma warning restore 612, 618
        }
    }
}