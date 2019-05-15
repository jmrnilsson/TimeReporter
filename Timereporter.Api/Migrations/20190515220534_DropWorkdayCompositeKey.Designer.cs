﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Timereporter.Api.Models;

namespace Timereporter.Api.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20190515220534_DropWorkdayCompositeKey")]
    partial class DropWorkdayCompositeKey
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Timereporter.Api.Models.CursorDo", b =>
                {
                    b.Property<string>("CursorType")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(25);

                    b.Property<DateTime>("Added");

                    b.Property<DateTime>("Changed");

                    b.Property<long>("Position");

                    b.HasKey("CursorType");

                    b.ToTable("Cursors");
                });

            modelBuilder.Entity("Timereporter.Api.Models.EventDo", b =>
                {
                    b.Property<long>("Timestamp");

                    b.Property<string>("Kind");

                    b.Property<DateTime>("Added");

                    b.Property<DateTime>("Changed");

                    b.HasKey("Timestamp", "Kind");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Timereporter.Api.Models.WorkdayDo", b =>
                {
                    b.Property<string>("Kind")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(25);

                    b.Property<DateTime>("Added");

                    b.Property<long?>("Arrival");

                    b.Property<long?>("Break");

                    b.Property<DateTime>("Changed");

                    b.Property<int>("Date");

                    b.Property<long?>("Departure");

                    b.Property<string>("HashCode")
                        .IsRequired();

                    b.HasKey("Kind");

                    b.ToTable("Workdays");
                });
#pragma warning restore 612, 618
        }
    }
}
