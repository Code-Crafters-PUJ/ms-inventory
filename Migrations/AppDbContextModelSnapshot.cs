﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ms_inventary.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Branch", b =>
                {
                    b.Property<int>("BranchId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("BranchId"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<int>("CompanyId")
                        .HasColumnType("integer");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.HasKey("BranchId");

                    b.ToTable("Branch");
                });

            modelBuilder.Entity("BranchHasProduct", b =>
                {
                    b.Property<int>("BranchId")
                        .HasColumnType("integer")
                        .HasColumnOrder(1);

                    b.Property<int>("ProductId")
                        .HasColumnType("integer")
                        .HasColumnOrder(2);

                    b.Property<int>("Discount")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("BranchId", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("BranchHasProduct");
                });

            modelBuilder.Entity("Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.HasKey("CategoryId");

                    b.ToTable("Category");
                });

            modelBuilder.Entity("Company", b =>
                {
                    b.Property<int>("CompanyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("CompanyId"));

                    b.Property<string>("NIT")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<string>("businessArea")
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<string>("employeeNumber")
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.HasKey("CompanyId");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ProductId"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<double>("CostPrice")
                        .HasColumnType("double precision");

                    b.Property<string>("Description")
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<double>("SalePrice")
                        .HasColumnType("double precision");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("ProductHasSupplier", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("integer")
                        .HasColumnOrder(1);

                    b.Property<int>("SupplierId")
                        .HasColumnType("integer")
                        .HasColumnOrder(2);

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnOrder(3);

                    b.Property<double>("CostPrice")
                        .HasColumnType("double precision")
                        .HasColumnOrder(4);

                    b.Property<int>("Quantity")
                        .HasColumnType("integer")
                        .HasColumnOrder(5);

                    b.Property<int>("OrderId")
                        .HasColumnType("integer")
                        .HasColumnOrder(6);

                    b.Property<int>("BranchId")
                        .HasColumnType("integer")
                        .HasColumnOrder(7);

                    b.HasKey("ProductId", "SupplierId", "PurchaseDate", "CostPrice", "Quantity", "OrderId", "BranchId");

                    b.HasIndex("SupplierId");

                    b.ToTable("ProductHasSupplier");
                });

            modelBuilder.Entity("ServiceType", b =>
                {
                    b.Property<int>("ServiceTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ServiceTypeId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.HasKey("ServiceTypeId");

                    b.ToTable("ServiceType");
                });

            modelBuilder.Entity("Supplier", b =>
                {
                    b.Property<int>("SupplierId")
                        .HasColumnType("integer");

                    b.Property<string>("Address")
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<int>("CompanyId")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<string>("Phone")
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.Property<int>("ServiceTypeId")
                        .HasColumnType("integer");

                    b.Property<int>("SupplierTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("UrlPage")
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.HasKey("SupplierId");

                    b.HasIndex("ServiceTypeId");

                    b.HasIndex("SupplierTypeId");

                    b.ToTable("Supplier");
                });

            modelBuilder.Entity("SupplierType", b =>
                {
                    b.Property<int>("SupplierTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("SupplierTypeId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("character varying(45)");

                    b.HasKey("SupplierTypeId");

                    b.ToTable("SupplierType");
                });

            modelBuilder.Entity("BranchHasProduct", b =>
                {
                    b.HasOne("Branch", "Branch")
                        .WithMany("BranchHasProducts")
                        .HasForeignKey("BranchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Product", "Product")
                        .WithMany("BranchHasProducts")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Product", b =>
                {
                    b.HasOne("Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("ProductHasSupplier", b =>
                {
                    b.HasOne("Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Supplier", "Supplier")
                        .WithMany()
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("Supplier", b =>
                {
                    b.HasOne("ServiceType", "ServiceType")
                        .WithMany()
                        .HasForeignKey("ServiceTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SupplierType", "SupplierType")
                        .WithMany()
                        .HasForeignKey("SupplierTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ServiceType");

                    b.Navigation("SupplierType");
                });

            modelBuilder.Entity("Branch", b =>
                {
                    b.Navigation("BranchHasProducts");
                });

            modelBuilder.Entity("Product", b =>
                {
                    b.Navigation("BranchHasProducts");
                });
#pragma warning restore 612, 618
        }
    }
}
