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
    [Migration("20241002090407_TimeSerieFactsCombinedKeyRemoved")]
    partial class TimeSerieFactsCombinedKeyRemoved
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.CurrencyDao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("CurrencyId_PK");

                    b.HasIndex("Code")
                        .IsUnique();

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

                    b.HasIndex("DateTimeUtc")
                        .IsUnique();

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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id")
                        .HasName("ExchangeId_PK");

                    b.HasIndex("Name")
                        .IsUnique();

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

                    b.Property<string>("InstrumentCode")
                        .IsRequired()
                        .HasColumnType("text");

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

                    b.HasIndex("Isin")
                        .IsUnique();

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

                    b.Property<int>("StockId")
                        .HasColumnType("integer");

                    b.HasKey("Id")
                        .HasName("TimeSerieId_PK");

                    b.HasIndex("StockId");

                    b.HasIndex("Name", "StockId")
                        .IsUnique();

                    b.ToTable("TimeSeries");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.TimeSerieFactDao", b =>
                {
                    b.Property<int>("TimeSerieId")
                        .HasColumnType("integer");

                    b.Property<int>("DateId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Volume")
                        .HasColumnType("numeric");

                    b.HasKey("TimeSerieId", "DateId")
                        .HasName("TimeSerieFact_PK");

                    b.HasIndex("DateId");

                    b.ToTable("TimeSerieFacts");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.StockDao", b =>
                {
                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.CurrencyDao", "Currency")
                        .WithMany("Stocks")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.ExchangeDao", "Exchange")
                        .WithMany("Stocks")
                        .HasForeignKey("ExchangeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");

                    b.Navigation("Exchange");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.TimeSerieDao", b =>
                {
                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.StockDao", "Stock")
                        .WithMany("TimeSeries")
                        .HasForeignKey("StockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.TimeSerieFactDao", b =>
                {
                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.DateDao", "Date")
                        .WithMany("Facts")
                        .HasForeignKey("DateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ElisBackend.Gateways.Repositories.Daos.TimeSerieDao", "TimeSerie")
                        .WithMany("Facts")
                        .HasForeignKey("TimeSerieId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Date");

                    b.Navigation("TimeSerie");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.CurrencyDao", b =>
                {
                    b.Navigation("Stocks");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.DateDao", b =>
                {
                    b.Navigation("Facts");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.ExchangeDao", b =>
                {
                    b.Navigation("Stocks");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.StockDao", b =>
                {
                    b.Navigation("TimeSeries");
                });

            modelBuilder.Entity("ElisBackend.Gateways.Repositories.Daos.TimeSerieDao", b =>
                {
                    b.Navigation("Facts");
                });
#pragma warning restore 612, 618
        }
    }
}
