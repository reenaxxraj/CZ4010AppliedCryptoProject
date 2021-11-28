﻿// <auto-generated />
using ApplicationServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ApplicationServer.Migrations
{
    [DbContext(typeof(CoreDbContext))]
    [Migration("20211126152848_.")]
    partial class _
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("ApplicationServer.Models.FileDataModel", b =>
                {
                    b.Property<string>("URL")
                        .HasColumnType("text");

                    b.Property<string>("EncryptedFile")
                        .HasColumnType("text");

                    b.HasKey("URL");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("ApplicationServer.Models.SharingDataModel", b =>
                {
                    b.Property<string>("URL")
                        .HasColumnType("text");

                    b.Property<string>("TaggedUsername")
                        .HasColumnType("text");

                    b.Property<string>("EncryptedKey")
                        .HasColumnType("text");

                    b.Property<bool>("IsOwner")
                        .HasColumnType("boolean");

                    b.HasKey("URL", "TaggedUsername");

                    b.ToTable("Sharing");
                });
#pragma warning restore 612, 618
        }
    }
}
