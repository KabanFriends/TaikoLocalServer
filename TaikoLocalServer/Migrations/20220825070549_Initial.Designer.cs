﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaikoLocalServer.Context;

#nullable disable

namespace TaikoLocalServer.Migrations
{
    [DbContext(typeof(TaikoDbContext))]
    [Migration("20220825070549_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.0-preview.7.22376.2");

            modelBuilder.Entity("TaikoLocalServer.Entities.Card", b =>
                {
                    b.Property<string>("AccessCode")
                        .HasColumnType("TEXT");

                    b.Property<uint>("Baid")
                        .HasColumnType("INTEGER");

                    b.HasKey("AccessCode");

                    b.HasIndex(new[] { "Baid" }, "IX_Card_Baid")
                        .IsUnique();

                    b.ToTable("Card", (string)null);
                });

            modelBuilder.Entity("TaikoLocalServer.Entities.SongBestDatum", b =>
                {
                    b.Property<uint>("Baid")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("SongId")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Difficulty")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("BestCrown")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("BestRate")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("BestScore")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("BestScoreRank")
                        .HasColumnType("INTEGER");

                    b.HasKey("Baid", "SongId", "Difficulty");

                    b.ToTable("SongBestData");
                });

            modelBuilder.Entity("TaikoLocalServer.Entities.SongPlayDatum", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Baid")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("ComboCount")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Crown")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Difficulty")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("GoodCount")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("HitCount")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("MissCount")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("OkCount")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("PlayTime")
                        .HasColumnType("datetime");

                    b.Property<uint>("Score")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("ScoreRank")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("ScoreRate")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Skipped")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("SongId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Baid");

                    b.ToTable("SongPlayData");
                });

            modelBuilder.Entity("TaikoLocalServer.Entities.UserDatum", b =>
                {
                    b.Property<uint>("Baid")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("AchievementDisplayDifficulty")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("ColorBody")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("ColorFace")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("ColorLimb")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CostumeData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("DisplayAchievement")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("DisplayDan")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FavoriteSongsArray")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSkipOn")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsVoiceOn")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("LastPlayDatetime")
                        .HasColumnType("datetime");

                    b.Property<long>("LastPlayMode")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MyDonName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("OptionSetting")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<uint>("TitlePlateId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Baid");

                    b.ToTable("UserData");
                });

            modelBuilder.Entity("TaikoLocalServer.Entities.SongBestDatum", b =>
                {
                    b.HasOne("TaikoLocalServer.Entities.Card", "Ba")
                        .WithMany()
                        .HasForeignKey("Baid")
                        .HasPrincipalKey("Baid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ba");
                });

            modelBuilder.Entity("TaikoLocalServer.Entities.SongPlayDatum", b =>
                {
                    b.HasOne("TaikoLocalServer.Entities.Card", "Ba")
                        .WithMany()
                        .HasForeignKey("Baid")
                        .HasPrincipalKey("Baid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ba");
                });

            modelBuilder.Entity("TaikoLocalServer.Entities.UserDatum", b =>
                {
                    b.HasOne("TaikoLocalServer.Entities.Card", "Ba")
                        .WithMany()
                        .HasForeignKey("Baid")
                        .HasPrincipalKey("Baid")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ba");
                });
#pragma warning restore 612, 618
        }
    }
}
