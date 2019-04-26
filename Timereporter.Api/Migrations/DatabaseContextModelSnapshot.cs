﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Timereporter.Api.Models;

namespace Timereporter.Api.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Timereporter.Api.Models.Cursor", b =>
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

            modelBuilder.Entity("Timereporter.Api.Models.Event", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Added");

                    b.Property<string>("Kind");

                    b.Property<DateTime>("Moment");

                    b.HasKey("EventId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Timereporter.Api.Models.Workday", b =>
                {
                    b.Property<int>("WorkdayId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Added");

                    b.Property<DateTime>("Arrival");

                    b.Property<int>("BreakDurationSeconds");

                    b.Property<DateTime>("Changed");

                    b.Property<string>("Date");

                    b.Property<DateTime>("Departure");

                    b.HasKey("WorkdayId");

                    b.ToTable("Workdays");
                });

            modelBuilder.Entity("Timereporter.Api.Models.WorkdayComment", b =>
                {
                    b.Property<int>("WorkdayCommentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Added");

                    b.Property<string>("Comment");

                    b.Property<int>("WorkdayId");

                    b.HasKey("WorkdayCommentId");

                    b.HasIndex("WorkdayId");

                    b.ToTable("WorkdayComments");
                });

            modelBuilder.Entity("Timereporter.Api.Models.WorkdayComment", b =>
                {
                    b.HasOne("Timereporter.Api.Models.Workday", "Workday")
                        .WithMany()
                        .HasForeignKey("WorkdayId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
