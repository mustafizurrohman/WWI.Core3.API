﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using WWI.Core3.Models.Models;
using WWI.Core3.Models.Utils;

namespace WWI.Core3.Models.DatabaseContext
{
    public partial class DocAppointmentContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DocAppointmentContext()
        {
        }

        public DocAppointmentContext(DbContextOptions<DocAppointmentContext> options)
            : base(options)
        {
        }

        #region -- Tables --

        public virtual DbSet<Doctor> Doctors { get; set; }

        public virtual DbSet<Speciality> Specialities { get; set; }

        public virtual DbSet<Address> Addresses { get; set; }

        public virtual DbSet<Hospital> Hospitals { get; set; }

        public virtual DbSet<HospitalDoctor> HospitalDoctors { get; set; }

        #endregion


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region -- Relationships -- 

            modelBuilder.Entity<HospitalDoctor>()
                .HasOne(hd => hd.Hospital)
                .WithMany(hospital => hospital.Doctors)
                .HasForeignKey(hospital => hospital.DoctorID);

            modelBuilder.Entity<HospitalDoctor>()
                .HasOne(hd => hd.Doctor)
                .WithMany(doctor => doctor.Hospitals)
                .HasForeignKey(doctor => doctor.HospitalID);

            #endregion

            SeedData(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        [ExcludeFromCodeCoverage]
        private void SeedData(ModelBuilder modelBuilder)
        {

            #region -- 'Speciality' Seed -- 

            modelBuilder.Entity<Speciality>().HasData(
                    new Speciality
                    {
                        SpecialityID = 1,
                        Name = "Pediatrics"
                    },
                    new Speciality
                    {
                        SpecialityID = 2,
                        Name = "Anesthesiology"
                    },
                    new Speciality
                    {
                        SpecialityID = 3,
                        Name = "Dermatology"
                    },
                    new Speciality
                    {
                        SpecialityID = 4,
                        Name = "Allergy and Immunology"
                    },
                    new Speciality
                    {
                        SpecialityID = 5,
                        Name = "Anesthesiology"
                    },
                    new Speciality
                    {
                        SpecialityID = 6,
                        Name = "Diagonistic Radiology"
                    },
                    new Speciality
                    {
                        SpecialityID = 7,
                        Name = "Emergency Medicine"
                    },
                    new Speciality
                    {
                        SpecialityID = 8,
                        Name = "Family Medicine"
                    },
                    new Speciality
                    {
                        SpecialityID = 9,
                        Name = "Internal Medicine"
                    },
                    new Speciality
                    {
                        SpecialityID = 10,
                        Name = "Medical Genetics"
                    },
                    new Speciality
                    {
                        SpecialityID = 11,
                        Name = "Neurology"
                    },
                    new Speciality
                    {
                        SpecialityID = 12,
                        Name = "Neuclear Medicine"
                    },
                    new Speciality
                    {
                        SpecialityID = 13,
                        Name = "Obstetrics and Gynecology"
                    },
                    new Speciality
                    {
                        SpecialityID = 14,
                        Name = "Opthalmology"
                    },
                    new Speciality
                    {
                        SpecialityID = 15,
                        Name = "Physical Medicine & Rehabilitation"
                    },
                    new Speciality
                    {
                        SpecialityID = 16,
                        Name = "Psychiatry"
                    },
                    new Speciality
                    {
                        SpecialityID = 17,
                        Name = "Radiation Oncology"
                    },
                    new Speciality
                    {
                        SpecialityID = 18,
                        Name = "Surgery"
                    },
                    new Speciality
                    {
                        SpecialityID = 19,
                        Name = "Urology"
                    }
                );

            #endregion

            #region -- 'Doctor Seed' --

            var basePath = "../WWI.Core3.Models/Seed/";

            var firstNamesContents = File.ReadAllText(basePath + "firstnames.json");
            var middleNamesContents = File.ReadAllText(basePath + "middlenames.json");
            var lastNamesContents = File.ReadAllText(basePath + "lastnames.json");

            List<string> firstNames = JsonConvert.DeserializeObject<List<string>>(firstNamesContents)
                .Select(fn => fn.Trim()).Distinct().Shuffle().ToList();

            List<string> middlenames = JsonConvert.DeserializeObject<List<string>>(middleNamesContents)
                .Select(mn => mn.Trim()).Distinct().Shuffle().ToList();

            List<string> lastNames = JsonConvert.DeserializeObject<List<string>>(lastNamesContents)
                .Select(ln => ln.Trim()).Distinct().Shuffle().ToList();

            var doctors = new List<Doctor>();

            for (int i = 1; i < 10000; i++)
            {
                doctors.Add(GetRandomDoctor(i));
            }

            // Local function
            Doctor GetRandomDoctor(int doctorID)
            {
                return new Doctor
                {
                    DoctorID = doctorID,
                    SpecialityID = RandomHelpers.Next(1, 19),
                    Firstname = firstNames.GetRandomShuffled(),
                    Middlename = middlenames.GetRandomShuffled(),
                    Lastname = lastNames.GetRandomShuffled()
                };
            }

            modelBuilder.Entity<Doctor>().HasData(doctors);



            #endregion

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
