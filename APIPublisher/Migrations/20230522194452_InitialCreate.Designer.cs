﻿// <auto-generated />
using APIPublisher.Infraestructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace APIPublisher.Migrations
{
    [DbContext(typeof(APIPublisherDbContext))]
    [Migration("20230522194452_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("APIPublisher.Domain.Books.Book", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PublisherId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("PublisherId");

                    b.ToTable("Books", "APIPublisher");
                });

            modelBuilder.Entity("APIPublisher.Domain.Publishers.Publisher", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.HasKey("Id");

                    b.ToTable("Publishers", "APIPublisher");
                });

            modelBuilder.Entity("APIPublisher.Domain.Books.Book", b =>
                {
                    b.HasOne("APIPublisher.Domain.Publishers.Publisher", "Publisher")
                        .WithMany("Books")
                        .HasForeignKey("PublisherId");

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("APIPublisher.Domain.Publishers.Publisher", b =>
                {
                    b.OwnsOne("System.Globalization.RegionInfo", "Country", b1 =>
                        {
                            b1.Property<string>("PublisherId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Country");

                            b1.HasKey("PublisherId");

                            b1.ToTable("Publishers", "APIPublisher");

                            b1.WithOwner()
                                .HasForeignKey("PublisherId");
                        });

                    b.Navigation("Country")
                        .IsRequired();
                });

            modelBuilder.Entity("APIPublisher.Domain.Publishers.Publisher", b =>
                {
                    b.Navigation("Books");
                });
#pragma warning restore 612, 618
        }
    }
}
