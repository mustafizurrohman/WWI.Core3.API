﻿// ***********************************************************************
// Assembly         : WWI.Core3.Models
// Author           : Mustafizur Rohman
// Created          : 05-01-2020
//
// Last Modified By : Mustafizur Rohman
// Last Modified On : 05-15-2020
// ***********************************************************************
// <copyright file="DocAppointmentContext.cs" company="WWI.Core3.Models">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.EntityFrameworkCore;
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
        /// <summary>
        /// The base path generated seed
        /// </summary>
        private const string BasePathGeneratedSeed = "../WWI.Core3.Models/Seed/Generated";
        /// <summary>
        /// The base path
        /// </summary>
        private const string BasePath = "../WWI.Core3.Models/Seed/";

        /// <summary>
        /// The doctor file name
        /// </summary>
        private const string DoctorFileName = "doctors.json";
        /// <summary>
        /// The addresses file name
        /// </summary>
        private const string AddressesFileName = "addresses.json";
        /// <summary>
        /// The first names file name
        /// </summary>
        private const string FirstNamesFileName = "firstnames.json";
        /// <summary>
        /// The middle names file name
        /// </summary>
        private const string MiddleNamesFileName = "middlenames.json";
        /// <summary>
        /// The last names file name
        /// </summary>
        private const string LastNamesFileName = "lastnames.json";
        /// <summary>
        /// The hospitals file name
        /// </summary>
        private const string HospitalsFileName = "hospitals.json";
        /// <summary>
        /// The specialities file name
        /// </summary>
        private const string SpecialitiesFileName = "specialities.json";
        /// <summary>
        /// The hospital doctors file name
        /// </summary>
        private const string HospitalDoctorsFileName = "hospitalDoctors.json";
        /// <summary>
        /// The hospital speciality file name
        /// </summary>
        private const string HospitalSpecialityFileName = "hospitalSpecialities.json";

        /// <summary>
        /// The default json serializer settings
        /// </summary>
        private readonly JsonSerializerSettings _defaultJsonSerializerSettings = new JsonSerializerSettings { Formatting = Formatting.Indented };

        /// <summary>
        /// Initializes a new instance of the <see cref="DocAppointmentContext"/> class.
        /// </summary>
        public DocAppointmentContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocAppointmentContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public DocAppointmentContext(DbContextOptions<DocAppointmentContext> options)
            : base(options)
        {

        }

        #region -- Tables --

        /// <summary>
        /// Doctors Table
        /// </summary>
        /// <value>The doctors.</value>
        public virtual DbSet<Doctor> Doctors { get; set; }

        /// <summary>
        /// Specialities Table
        /// </summary>
        /// <value>The specialities.</value>
        public virtual DbSet<Speciality> Specialities { get; set; }

        /// <summary>
        /// Addresses Table
        /// </summary>
        /// <value>The addresses.</value>
        public virtual DbSet<Address> Addresses { get; set; }

        /// <summary>
        /// Hospitals Table
        /// </summary>
        /// <value>The hospitals.</value>
        public virtual DbSet<Hospital> Hospitals { get; set; }

        /// <summary>
        /// Hospital Doctors Table
        /// </summary>
        /// <value>The hospital doctors.</value>
        public virtual DbSet<HospitalDoctor> HospitalDoctors { get; set; }

        #endregion

        /// <summary>
        /// <para>
        /// Override this method to configure the database (and other options) to be used for this context.
        /// This method is called for each instance of the context that is created.
        /// The base implementation does nothing.
        /// </para>
        /// <para>
        /// In situations where an instance of <see cref="T:Microsoft.EntityFrameworkCore.DbContextOptions" /> may or may not have been passed
        /// to the constructor, you can use <see cref="P:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.IsConfigured" /> to determine if
        /// the options have already been set, and skip some or all of the logic in
        /// <see cref="M:Microsoft.EntityFrameworkCore.DbContext.OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder)" />.
        /// </para>
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context. Databases (and other extensions)
        /// typically define extension methods on this object that allow you to configure the context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        /// <summary>
        /// Override this method to further configure the model that was discovered by convention from the entity types
        /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
        /// and re-used for subsequent instances of your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context. Databases (and other extensions) typically
        /// define extension methods on this object that allow you to configure aspects of the model that are specific
        /// to a given database.</param>
        /// <remarks>If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
        /// then this method will not be run.</remarks>
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
        /// Generates the seed data.
        /// </summary>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
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

            Log.Debug("Completed generation of seed data");
        }

        /// <summary>
        /// Seeds the data.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
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
