﻿using Microsoft.EntityFrameworkCore;
using TFT_API.Helpers;
using TFT_API.Models.Augments;
using TFT_API.Models.AutoGeneratedGuide;
using TFT_API.Models.Item;
using TFT_API.Models.Match;
using TFT_API.Models.Stats;
using TFT_API.Models.Stats.AugmentStats;
using TFT_API.Models.Stats.CompStats;
using TFT_API.Models.Stats.CoOccurrence;
using TFT_API.Models.Stats.ItemStats;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;
using TFT_API.Models.Trait;
using TFT_API.Models.Unit;
using TFT_API.Models.User;
using TFT_API.Models.UserGuides;
using TFT_API.Models.Votes;

namespace TFT_API.Data
{
    public class TFTContext : DbContext
    {
        public TFTContext() { }

        public TFTContext(DbContextOptions<TFTContext> options) : base(options) { }

        public DbSet<PersistedUnit> Units { get; set; }
        public DbSet<PersistedItem> Items { get; set; }
        public DbSet<PersistedTrait> Traits { get; set; }
        public DbSet<UnitTrait> UnitTraits { get; set; }
        public DbSet<PersistedUser> Users { get; set; }
        public DbSet<UserGuide> UserGuides { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PersistedAugment> Augments { get; set; }
        public DbSet<Stat> Stats { get; set; }
        public DbSet<UnitStat> UnitStats { get; set; }
        public DbSet<StarredUnitStat> StarredUnitStats { get; set; }
        public DbSet<BaseUnitStat> BaseUnitStat { get; set; }
        public DbSet<ItemStat> ItemStats { get; set; }
        public DbSet<BaseItemStat> BaseItemStat { get; set; }
        public DbSet<TraitStat> TraitStats { get; set; }
        public DbSet<BaseTraitStat> BaseTraitStat { get; set; }
        public DbSet<AugmentStat> AugmentStats { get; set; }
        public DbSet<BaseAugmentStat> BaseAugmentStat { get; set; }
        public DbSet<CompStat> CompStats { get; set; }  
        public DbSet<BaseCompStat> BaseCompStat { get; set; }
        public DbSet<Match> Matches {  get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Hex> Hexes { get; set; }
        public DbSet<GuideTrait> GuideTraits { get; set; }
        public DbSet<GuideAugment> GuideAugments { get; set; }
        public DbSet<HexItem> HexItems { get; set; }
        public DbSet<CoOccurrence> CoOccurrences { get; set; }
        public DbSet<BaseCoOccurrence> BaseCoOccurrences { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitTrait>()
                .HasKey(ut => new { ut.UnitId, ut.TraitId });

            modelBuilder.Entity<UnitTrait>()
                .HasOne(ut => ut.Unit)
                .WithMany(u => u.Traits)
                .HasForeignKey(ut => ut.UnitId);

            modelBuilder.Entity<Match>()
                .HasIndex(m => m.Placement)
                .HasDatabaseName("IX_Match_Placement");

            modelBuilder.Entity<Match>()
                .HasIndex(m => m.Augments)
                .HasDatabaseName("IX_Match_Augments");

            modelBuilder.Entity<MatchUnit>()
                .HasIndex(mu => mu.CharacterId)
                .HasDatabaseName("IX_MatchUnit_CharacterId");

            modelBuilder.Entity<MatchTrait>()
                .HasIndex(mt => mt.Name)
                .HasDatabaseName("IX_MatchTrait_Name");

            modelBuilder.Entity<MatchTrait>()
                .HasIndex(mt => mt.NumUnits)
                .HasDatabaseName("IX_MatchTrait_NumUnits");

            modelBuilder.Entity<MatchUnit>()
                .HasIndex(mu => mu.Tier)
                .HasDatabaseName("IX_MatchUnit_Tier");

            modelBuilder.Entity<BaseAugmentStat>()
                .HasMany(a => a.AugmentStats)
                .WithOne(a => a.BaseAugmentStat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaseCompStat>()
                .HasMany(b => b.CompStats)
                .WithOne(a => a.BaseCompStat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaseUnitStat>()
                .HasMany(b => b.UnitStats)
                .WithOne(a => a.BaseUnitStat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaseUnitStat>()
                .HasMany(b => b.StarredUnitStats)
                .WithOne(a => a.BaseUnitStat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaseTraitStat>()
                .HasMany(b => b.TraitStats)
                .WithOne(a => a.BaseTraitStat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaseItemStat>()
                .HasMany(b => b.ItemStats)
                .WithOne(a => a.BaseItemStat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AugmentStat>()
                .HasMany(a => a.Stats)
                .WithOne(s => s.AugmentStat)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BaseCoOccurrence>()
                .HasMany(a => a.CoOccurrences)
                .WithOne(s => s.BaseCoOccurrence)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGuide>()
                .HasMany(a => a.Traits)
                .WithOne(s => s.UserGuide)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGuide>()
                .HasMany(a => a.Augments)
                .WithOne(s => s.UserGuide)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGuide>()
                .HasMany(a => a.Hexes)
                .WithOne(s => s.UserGuide)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGuide>()
                .HasMany(a => a.Comments)
                .WithOne(s => s.UserGuide)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserGuide>()
                .HasMany(a => a.Votes)
                .WithOne(s => s.UserGuide)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Hex>()
                .HasMany(a => a.CurrentItems)
                .WithOne(s => s.Hex)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PersistedUser>()
                .HasMany(a => a.Comments)
                .WithOne(s => s.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PersistedUser>()
                .HasMany(a => a.Votes)
                .WithOne(s => s.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PersistedUser>()
                .HasMany(a => a.UserGuides)
                .WithOne(s => s.User)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PersistedTrait>()
                .HasMany(a => a.Tiers)
                .WithOne(s => s.Trait)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PersistedTrait>()
                .Property(t => t.Stats)
                .HasConversion(new DictionaryToJsonConverter())
                .Metadata.SetValueComparer(new DictionaryComparer());
        }
    }
}
