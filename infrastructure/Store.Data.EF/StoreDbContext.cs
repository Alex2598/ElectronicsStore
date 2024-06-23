using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Store.Data.EF
{
    public class StoreDbContext : DbContext
    {
        public DbSet<ComponentDto> Components { get; set; }

        public DbSet<OrderDto> Orders { get; set; }

        public DbSet<OrderItemDto> OrderItems { get; set; }

        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BuildComponent(modelBuilder);
            BuildOrders(modelBuilder);
            BuildOrderItems(modelBuilder);
        }

        private void BuildOrderItems(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItemDto>(action =>
            {
                action.Property(dto => dto.Price)
                      .HasColumnType("money");

                action.HasOne(dto => dto.Order)
                      .WithMany(dto => dto.Items)
                      .IsRequired();
            });
        }

        private static void BuildOrders(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDto>(action =>
            {
                action.Property(dto => dto.CellPhone)
                      .HasMaxLength(20);

                action.Property(dto => dto.DeliveryUniqueCode)
                      .HasMaxLength(40);

                action.Property(dto => dto.DeliveryPrice)
                      .HasColumnType("money");

                action.Property(dto => dto.PaymentServiceName)
                      .HasMaxLength(40);

                action.Property(dto => dto.DeliveryParameters)
                      .HasConversion(
                          value => JsonConvert.SerializeObject(value),
                          value => JsonConvert.DeserializeObject<Dictionary<string, string>>(value))
                      .Metadata.SetValueComparer(DictionaryComparer);

                action.Property(dto => dto.PaymentParameters)
                      .HasConversion(
                          value => JsonConvert.SerializeObject(value),
                          value => JsonConvert.DeserializeObject<Dictionary<string, string>>(value))
                      .Metadata.SetValueComparer(DictionaryComparer);
            });
        }

        private static readonly ValueComparer DictionaryComparer =
            new ValueComparer<Dictionary<string, string>>(
                (dictionary1, dictionary2) => dictionary1.SequenceEqual(dictionary2),
                dictionary => dictionary.Aggregate(
                    0,
                    (a, p) => HashCode.Combine(HashCode.Combine(a, p.Key.GetHashCode()), p.Value.GetHashCode())
                )
            );

        private static void BuildComponent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ComponentDto>(action =>
            {
                action.Property(dto => dto.UId)
                      .HasMaxLength(17)
                      .IsRequired();

                action.Property(dto => dto.NameOfComponent)
                      .IsRequired();

                action.Property(dto => dto.Price)
                      .HasColumnType("money");

                action.HasData(
                    new ComponentDto
                    {
                        Id = 1,
                        UId = "10001",
                        Package = "0805",
                        NameOfComponent = "Resistor",
                        Description = "Set of resistors of different values",
                        Price = 7.19m,
                    },
                    new ComponentDto
                    {
                        Id = 2,
                        UId = "10002",
                        Package = "0805",
                        NameOfComponent = "Capacitor",
                        Description = "Set of capacitors of different values",
                        Price = 12.45m,
                    },
                    new ComponentDto
                    {
                        Id = 3,
                        UId = "10003",
                        Package = "0805",
                        NameOfComponent = "Inductor",
                        Description = "Set of Inductors of different values",
                        Price = 14.98m,
                    }
                );
            });
        }
    }
}
