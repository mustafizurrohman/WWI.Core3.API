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

namespace WWI.Core3.Models.DbContext
{
    /// <summary>
    /// Database context
    /// </summary>
    public partial class DocAppointmentContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private const string BasePathGeneratedSeed = "../WWI.Core3.Models/Seed/Generated";
        private const string BasePath = "../WWI.Core3.Models/Seed/";

        private const string DoctorFileName = "doctors.json";
        private const string AddressesFileName = "addresses.json";
        private const string FirstNamesFileName = "firstnames.json";
        private const string MiddleNamesFileName = "middlenames.json";
        private const string LastNamesFileName = "lastnames.json";
        private const string HospitalsFileName = "hospitals.json";
        private const string SpecialitiesFileName = "specialities.json";
        private const string HospitalDoctorsFileName = "hospitalDoctors.json";
        private const string HospitalSpecialityFileName = "hospitalSpecialities.json";

        private readonly JsonSerializerSettings _defaultJsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };

        /// <summary>
        /// 
        /// </summary>
        public DocAppointmentContext()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public DocAppointmentContext(DbContextOptions<DocAppointmentContext> options)
            : base(options)
        {

        }

        #region -- Tables --

        /// <summary>
        /// Doctors Table
        /// </summary>
        public virtual DbSet<Doctor> Doctors { get; set; }

        /// <summary>
        /// Specialities Table
        /// </summary>
        public virtual DbSet<Speciality> Specialities { get; set; }

        /// <summary>
        /// Addresses Table
        /// </summary>
        public virtual DbSet<Address> Addresses { get; set; }

        /// <summary>
        /// Hospitals Table
        /// </summary>
        public virtual DbSet<Hospital> Hospitals { get; set; }

        /// <summary>
        /// Hospital Doctors Table
        /// </summary>
        public virtual DbSet<HospitalDoctor> HospitalDoctors { get; set; }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
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


            modelBuilder.Entity<HospitalSpeciality>()
                .HasOne(hs => hs.Hospital)
                .WithMany(h => h.Specialities)
                .HasForeignKey(hs => hs.HospitalID);

            modelBuilder.Entity<HospitalSpeciality>()
                .HasOne(hs => hs.Speciality)
                .WithMany(h => h.Hospitals)
                .HasForeignKey(hs => hs.SpecialtyID);


            #endregion

            GenerateSeedData();
            SeedData(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="overwrite"></param>
        private void GenerateSeedData(bool overwrite = false)
        {
            Log.Debug($"Generating seed data with overwrite set to {overwrite}");

            Directory.CreateDirectory(BasePathGeneratedSeed);

            #region -- Generate Seed for Speciality --

            List<Speciality> specialityList = SeedHelper.ParseSourceFile<Speciality>(SpecialitiesFileName);

            var specialitiesJsonString = JsonConvert.SerializeObject(specialityList, new JsonSerializerSettings { Formatting = Formatting.Indented });

            SeedHelper.SaveOrOverwriteGeneratedFile(SpecialitiesFileName, specialitiesJsonString, overwrite);

            #endregion

            #region -- Generate Seed for doctors -- 

            List<string> firstNames = SeedHelper.ParseSourceFile<string>(FirstNamesFileName)
                .Select(fn => fn.Trim()).Distinct().Shuffle().ToList();

            List<string> middleNames = SeedHelper.ParseSourceFile<string>(MiddleNamesFileName)
                .Select(mn => mn.Trim()).Distinct().Shuffle().ToList();

            List<string> lastNames = SeedHelper.ParseSourceFile<string>(LastNamesFileName)
                .Select(ln => ln.Trim()).Distinct().Shuffle().ToList();

            var doctorList = new List<Doctor>();

            for (int i = 1; i < 500; i++)
            {
                doctorList.Add(GetRandomDoctor(i));
            }

            var doctorListJsonString = JsonConvert.SerializeObject(doctorList, _defaultJsonSerializerSettings);

            SeedHelper.SaveOrOverwriteGeneratedFile(DoctorFileName, doctorListJsonString, overwrite);

            // Local function
            Doctor GetRandomDoctor(int doctorID)
            {
                return new Doctor
                {
                    DoctorID = doctorID,
                    SpecialityID = specialityList.GetRandomShuffled().SpecialtyID,
                    Firstname = firstNames.GetRandomShuffled(),
                    Middlename = middleNames.GetRandomShuffled(),
                    Lastname = lastNames.GetRandomShuffled()
                };
            }

            #endregion

            #region -- Generate Seed for Addresses --

            List<Address> addressList = SeedHelper.ParseSourceFile<Address>(AddressesFileName);

            var addressJsonString = JsonConvert.SerializeObject(addressList, _defaultJsonSerializerSettings);

            SeedHelper.SaveOrOverwriteGeneratedFile(AddressesFileName, addressJsonString, overwrite);

            #endregion

            #region -- Generate Seed for Hospital --

            List<Hospital> hospitalList = SeedHelper.ParseSourceFile<Hospital>(HospitalsFileName);

            var hospitalJsonString = JsonConvert.SerializeObject(hospitalList, _defaultJsonSerializerSettings);

            SeedHelper.SaveOrOverwriteGeneratedFile(HospitalsFileName, hospitalJsonString, overwrite);

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

            SeedHelper.SaveOrOverwriteGeneratedFile(HospitalDoctorsFileName, hospitalDoctorJsonString, overwrite);

            #endregion

            #region  -- Generate Seed for HospitalSpeciality -- 

            List<HospitalSpeciality> hospitalSpecialities = new List<HospitalSpeciality>();

            int countHospitalSpeciality = 1;

            hospitalList.ForEach(hospital =>
            {
                var randomNumberOfSpecialities = RandomHelpers.Next(specialityList.Count);

                for (int i = 0; i < randomNumberOfSpecialities; i++)
                {
                    var hs = new HospitalSpeciality()
                    {
                        HospitalSpecialityID = countHospitalSpeciality++,
                        HospitalID = hospital.HospitalID,
                        SpecialtyID = specialityList.GetRandomShuffled().SpecialtyID
                    };

                    hospitalSpecialities.Add(hs);
                }
            });

            int currentHs = 1;

            hospitalSpecialities = hospitalSpecialities
                .DistinctBy(hs => new { hs.HospitalID, hs.SpecialtyID })
                .Select(hs => new HospitalSpeciality()
                {
                    HospitalSpecialityID = currentHs++,
                    HospitalID = hs.HospitalID,
                    SpecialtyID = hs.SpecialtyID
                })
                .ToList();

            var hospitalSpecialitiesJsonString = JsonConvert.SerializeObject(hospitalSpecialities, _defaultJsonSerializerSettings);

            SeedHelper.SaveOrOverwriteGeneratedFile(HospitalSpecialityFileName, hospitalSpecialitiesJsonString, overwrite);


            #endregion

            Log.Debug($"Completed generation of seed data");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        [ExcludeFromCodeCoverage]
        private void SeedData(ModelBuilder modelBuilder)
        {
            List<Speciality> specialityList = SeedHelper.ParseGeneratedFile<Speciality>(SpecialitiesFileName);
            List<Doctor> doctorList = SeedHelper.ParseGeneratedFile<Doctor>(DoctorFileName);
            List<Address> addressList = SeedHelper.ParseGeneratedFile<Address>(AddressesFileName);
            List<Hospital> hospitalList = SeedHelper.ParseGeneratedFile<Hospital>(HospitalsFileName);
            List<HospitalDoctor> hospitalDoctorList = SeedHelper.ParseGeneratedFile<HospitalDoctor>(HospitalDoctorsFileName);
            List<HospitalSpeciality> hospitalSpecialityList = SeedHelper.ParseGeneratedFile<HospitalSpeciality>(HospitalSpecialityFileName);

            modelBuilder.Entity<Speciality>().HasData(specialityList);
            modelBuilder.Entity<Doctor>().HasData(doctorList);
            modelBuilder.Entity<Address>().HasData(addressList);
            modelBuilder.Entity<Hospital>().HasData(hospitalList);
            modelBuilder.Entity<HospitalDoctor>().HasData(hospitalDoctorList);
            modelBuilder.Entity<HospitalSpeciality>().HasData(hospitalSpecialityList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
