﻿// <auto-generated />
using System;
using ElisBackend.Gateways.Dal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ElisBackend.Migrations
{
    [DbContext(typeof(ElisContext))]
    [Migration("20240424095016_AddTimeSerieData")]
    partial class AddTimeSerieData
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.CurrencyDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Short")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("CurrencyId_PK");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.DateDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateTimeUtc")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id")
                        .HasName("DateId_PK");

                    b.ToTable("Dates");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.ExchangeDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExchangeUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("ExchangeId_PK");

                    b.ToTable("Exchanges");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.StockDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CurrencyId")
                        .HasColumnType("integer");

                    b.Property<int>("ExchangeId")
                        .HasColumnType("integer");

                    b.Property<string>("Isin")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("StockId_PK");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("ExchangeId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.StockSearchResultDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.HasKey("Id");

                    b.ToTable("StockResults", null, t =>
                        {
                            t.ExcludeFromMigrations();
                        });
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.TimeSerieDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("TimeSerieId_PK");

                    b.ToTable("TimeSeries");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.TimeSerieFactDao", b =>
                {
                    b.Property<int>("TimeSerieId")
                        .HasColumnType("integer");

                    b.Property<int>("StockId")
                        .HasColumnType("integer");

                    b.Property<int>("DateId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Volume")
                        .HasColumnType("numeric");

                    b.HasKey("TimeSerieId", "StockId", "DateId")
                        .HasName("TimeSerieFact_PK");

                    b.HasIndex("DateId");

                    b.HasIndex("StockId");

                    b.ToTable("TimeSerieFacts");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.TimeZoneDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ExchangeId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("FromUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Offset")
                        .HasColumnType("integer");

                    b.Property<DateTime>("ToUtc")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id")
                        .HasName("TimeZoneId_PK");

                    b.HasIndex("ExchangeId");

                    b.ToTable("TimeZones");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.StockDao", b =>
                {
                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.CurrencyDao", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.ExchangeDao", "Exchange")
                        .WithMany()
                        .HasForeignKey("ExchangeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");

                    b.Navigation("Exchange");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.TimeSerieFactDao", b =>
                {
                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.DateDao", "Date")
                        .WithMany()
                        .HasForeignKey("DateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.StockDao", "Stock")
                        .WithMany()
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.TimeSerieDao", "TimeSerie")
                        .WithMany()
                        .HasForeignKey("TimeSerieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Date");

                    b.Navigation("Stock");

                    b.Navigation("TimeSerie");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.TimeZoneDao", b =>
                {
                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.ExchangeDao", "Exchange")
                        .WithMany()
                        .HasForeignKey("ExchangeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exchange");
                });
#pragma warning restore 612, 618
        }
    }
}
