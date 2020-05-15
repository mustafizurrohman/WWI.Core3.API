﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using WWI.Core3.Models.Models;
using WWI.Core3.Models.Seed.Helper;
using WWI.Core3.Models.Utils;

namespace WWI.Core3.Models.DatabaseContext
{
    public partial class DocAppointmentContext : Microsoft.EntityFrameworkCore.DbContext
    {

        const string basePathGeneratedSeed = "../WWI.Core3.Models/Seed/Generated";
        const string basePath = "../WWI.Core3.Models/Seed/";

        const string doctorFileName = "doctors.json";
        const string addressesFileName = "addresses.json";
        const string firstNamesFileName = "firstnames.json";
        const string middleNamesFileName = "middlenames.json";
        const string lastNamesFileName = "lastnames.json";
        const string hospitalsFileName = "hospitals.json";
        const string specialitiesFileName = "specialities.json";
        const string hospitalDoctorsFileName = "hospitalDoctors.json";

        private readonly JsonSerializerSettings _defaultJsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };

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
                .HasForeignKey(hospital => hospital.HospitalID);

            modelBuilder.Entity<HospitalDoctor>()
                .HasOne(hd => hd.Doctor)
                .WithMany(doctor => doctor.Hospitals)
                .HasForeignKey(doctor => doctor.DoctorID);

            #endregion

            GenerateSeedData(true);
            SeedData(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        private void GenerateSeedData(bool overwrite = false)
        {
            Log.Debug($"Generating seed data with overwrite set to {overwrite}");

            Directory.CreateDirectory(basePathGeneratedSeed);

            #region -- Generate Seed for Speciality --

            List<Speciality> specialityList = SeedHelper.ParseSourceFile<Speciality>(specialitiesFileName);

            var specialitiesJsonString = JsonConvert.SerializeObject(specialityList, new JsonSerializerSettings { Formatting = Formatting.Indented });

            SeedHelper.SaveOrOverwriteGeneratedFile(specialitiesFileName, specialitiesJsonString, overwrite);

            #endregion

            #region -- Generate Seed for doctors -- 

            List<string> firstNames = SeedHelper.ParseSourceFile<string>(firstNamesFileName)
                .Select(fn => fn.Trim()).Distinct().Shuffle().ToList();

            List<string> middlenames = SeedHelper.ParseSourceFile<string>(middleNamesFileName)
                .Select(mn => mn.Trim()).Distinct().Shuffle().ToList();

            List<string> lastNames = SeedHelper.ParseSourceFile<string>(lastNamesFileName)
                .Select(ln => ln.Trim()).Distinct().Shuffle().ToList();

            var doctorList = new List<Doctor>();

            for (int i = 1; i < 500; i++)
            {
                doctorList.Add(GetRandomDoctor(i));
            }

            var doctorListJsonString = JsonConvert.SerializeObject(doctorList, _defaultJsonSerializerSettings);

            SeedHelper.SaveOrOverwriteGeneratedFile(doctorFileName, doctorListJsonString, overwrite);

            // Local function
            Doctor GetRandomDoctor(int doctorID)
            {
                return new Doctor
                {
                    DoctorID = doctorID,
                    SpecialityID = specialityList.GetRandomShuffled().SpecialityID,
                    Firstname = firstNames.GetRandomShuffled(),
                    Middlename = middlenames.GetRandomShuffled(),
                    Lastname = lastNames.GetRandomShuffled()
                };
            }

            #endregion

            #region -- Generate Seed for Addresses --

            List<Address> addressList = SeedHelper.ParseSourceFile<Address>(addressesFileName);

            var addressJsonString = JsonConvert.SerializeObject(addressList, _defaultJsonSerializerSettings);

            SeedHelper.SaveOrOverwriteGeneratedFile(addressesFileName, addressJsonString, overwrite);

            #endregion

            #region -- Generate Seed for Hospital --

            List<Hospital> hospitalList = SeedHelper.ParseSourceFile<Hospital>(hospitalsFileName);

            var hospitalJsonString = JsonConvert.SerializeObject(hospitalList, _defaultJsonSerializerSettings);

            SeedHelper.SaveOrOverwriteGeneratedFile(hospitalsFileName, hospitalJsonString, overwrite);

            #endregion

            #region -- Generate Seed for HospitalDoctor --

            List<HospitalDoctor> hospitalDoctorList = new List<HospitalDoctor>();

            for (int i = 1; i < 10000; i++)
            {
                hospitalDoctorList.Add(GetRandomHospitalDoctor(i));
            }

            HospitalDoctor GetRandomHospitalDoctor(int hospitalDoctorID)
            {
                return new HospitalDoctor
                {
                    HospitalDoctorID = hospitalDoctorID,
                    HospitalID = hospitalList.GetRandomShuffled().HospitalID,
                    DoctorID = doctorList.GetRandomShuffled().DoctorID
                };
            }


            var hospitalDoctorJsonString = JsonConvert.SerializeObject(hospitalDoctorList, _defaultJsonSerializerSettings);

            SeedHelper.SaveOrOverwriteGeneratedFile(hospitalDoctorsFileName, hospitalDoctorJsonString, overwrite);

            #endregion

            Log.Debug($"Completed generation of seed data");
        }


        [ExcludeFromCodeCoverage]
        private void SeedData(ModelBuilder modelBuilder)
        {
            List<Speciality> specialityList = SeedHelper.ParseGeneratedFile<Speciality>(specialitiesFileName);
            List<Doctor> doctorList = SeedHelper.ParseGeneratedFile<Doctor>(doctorFileName);
            List<Address> addressList = SeedHelper.ParseGeneratedFile<Address>(addressesFileName);
            List<Hospital> hospitalList = SeedHelper.ParseGeneratedFile<Hospital>(hospitalsFileName);
            List<HospitalDoctor> hospitalDdoctorList = SeedHelper.ParseGeneratedFile<HospitalDoctor>(hospitalDoctorsFileName);

            modelBuilder.Entity<Speciality>().HasData(specialityList);
            modelBuilder.Entity<Doctor>().HasData(doctorList);
            modelBuilder.Entity<Address>().HasData(addressList);
            modelBuilder.Entity<Hospital>().HasData(hospitalList);
            modelBuilder.Entity<HospitalDoctor>().HasData(hospitalDdoctorList);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
