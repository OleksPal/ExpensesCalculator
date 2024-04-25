﻿// <auto-generated />
using System;
using ExpensesCalculator.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ExpensesCalculator.Migrations
{
    [DbContext(typeof(ExpensesContext))]
    [Migration("20240425163518_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("ExpensesCalculator.Models.Check", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DayExpensesId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Sum")
                        .HasColumnType("INTEGER");

                    b.Property<string>("VerificationPath")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DayExpensesId");

                    b.ToTable("Checks");
                });

            modelBuilder.Entity("ExpensesCalculator.Models.DayExpenses", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateOnly>("Date")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Days");
                });

            modelBuilder.Entity("ExpensesCalculator.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CheckId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Price")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CheckId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("ExpensesCalculator.Models.Check", b =>
                {
                    b.HasOne("ExpensesCalculator.Models.DayExpenses", null)
                        .WithMany("Checks")
                        .HasForeignKey("DayExpensesId");
                });

            modelBuilder.Entity("ExpensesCalculator.Models.Item", b =>
                {
                    b.HasOne("ExpensesCalculator.Models.Check", null)
                        .WithMany("Items")
                        .HasForeignKey("CheckId");
                });

            modelBuilder.Entity("ExpensesCalculator.Models.Check", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("ExpensesCalculator.Models.DayExpenses", b =>
                {
                    b.Navigation("Checks");
                });
#pragma warning restore 612, 618
        }
    }
}
