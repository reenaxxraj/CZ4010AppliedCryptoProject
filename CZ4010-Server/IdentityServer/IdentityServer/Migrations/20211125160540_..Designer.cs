﻿// <auto-generated />
using IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace IdentityServer.Migrations
{
    [DbContext(typeof(IdentityDbContext))]
    [Migration("20211125160540_.")]
    partial class _
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("IdentityServer.Models.IdentityModel", b =>
                {
                    b.Property<string>("TaggedUsername")
                        .HasColumnType("text");

                    b.HasKey("TaggedUsername");

                    b.ToTable("Identities");
                });

            modelBuilder.Entity("IdentityServer.Models.IdentityModel", b =>
                {
                    b.OwnsOne("IdentityServer.Models.RSAPubKey", "PublicKey", b1 =>
                        {
                            b1.Property<string>("IdentityModelTaggedUsername")
                                .HasColumnType("text");

                            b1.Property<string>("Exponent")
                                .HasColumnType("text");

                            b1.Property<string>("Modulus")
                                .HasColumnType("text");

                            b1.HasKey("IdentityModelTaggedUsername");

                            b1.ToTable("Identities");

                            b1.WithOwner()
                                .HasForeignKey("IdentityModelTaggedUsername");
                        });

                    b.Navigation("PublicKey");
                });
#pragma warning restore 612, 618
        }
    }
}
