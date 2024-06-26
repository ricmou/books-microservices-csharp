﻿// <auto-generated />
using System;
using APIExemplar.Infraestructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace apiexemplar.Migrations
{
    [DbContext(typeof(APIExemplarDbContext))]
    partial class APIExemplarDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("APIExemplar.Domain.Exemplars.Exemplar", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("DateOfAcquisition")
                        .HasColumnType("date")
                        .HasColumnName("DateOfAcquisition");

                    b.HasKey("Id");

                    b.ToTable("Exemplar", "APIExemplar");
                });

            modelBuilder.Entity("APIExemplar.Domain.Exemplars.Exemplar", b =>
                {
                    b.OwnsOne("APIExemplar.Domain.Clients.ClientId", "SellerId", b1 =>
                        {
                            b1.Property<string>("ExemplarId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("SellerId");

                            b1.HasKey("ExemplarId");

                            b1.ToTable("Exemplar", "APIExemplar");

                            b1.WithOwner()
                                .HasForeignKey("ExemplarId");
                        });

                    b.OwnsOne("APIExemplar.Domain.Exemplars.BookId", "Book", b1 =>
                        {
                            b1.Property<string>("ExemplarId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("BookId");

                            b1.HasKey("ExemplarId");

                            b1.ToTable("Exemplar", "APIExemplar");

                            b1.WithOwner()
                                .HasForeignKey("ExemplarId");
                        });

                    b.OwnsOne("APIExemplar.Domain.Exemplars.ExemplarState", "BookState", b1 =>
                        {
                            b1.Property<string>("ExemplarId")
                                .HasColumnType("nvarchar(450)");

                            b1.Property<int>("State")
                                .HasColumnType("int")
                                .HasColumnName("BookState");

                            b1.HasKey("ExemplarId");

                            b1.ToTable("Exemplar", "APIExemplar");

                            b1.WithOwner()
                                .HasForeignKey("ExemplarId");
                        });

                    b.Navigation("Book")
                        .IsRequired();

                    b.Navigation("BookState")
                        .IsRequired();

                    b.Navigation("SellerId")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
