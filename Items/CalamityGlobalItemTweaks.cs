using System.Collections.Generic;
using CalamityMod.Balancing;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    // TODO -- The item tweaks database and functions should be stored in a ModSystem.
    // ApplyTweaks(ref Item item) would be the one exposed function, which CalamityGlobalItem would call in SetDefaults.
    public partial class CalamityGlobalItem : GlobalItem
    {
        // 02AUG2023: Ozzatron: Having Overhaul enabled turns off all scaling changes, per direct request from Mirsario on 8/2/23.
        private static bool DisableScalingForOverhaul => ExternalMods.overhaul is not null;

        #region Database and Initialization
        internal static SortedDictionary<int, IItemTweak[]> currentTweaks = null;

        internal static void LoadTweaks()
        {
            // Various shorthands for items which receive very simple changes, or are repeated multiple times.
            IItemTweak[] trueMelee = Do(TrueMelee);
            IItemTweak[] trueMeleeNoSpeed = Do(TrueMeleeNoSpeed);
            IItemTweak[] nonConsumableBossSummon = Do(MaxStack(1), NotConsumable);
            IItemTweak[] phaseblade = Do(UseTurn, DamageExact(36)); // VANILLA: 26 DMG
            IItemTweak[] phasesaber = Do(DamageExact(132)); // VANILLA: 48 DMG

            // SORTING NOTES:
            // 1. Sort tweaks by categories first, then sort by the internal name in alphabetical order. Navigate through categories and names using the search function.
            // 2. Higher categories hold priority over lower ones (ie. Balancing with price tweaks belong in balancing, rather than price)
            // 3. All balancing tweaks should have comments with the vanilla stats for convenience and reference.
            // 4. Items whose internal names are vastly different from their actual names should have comments for ease of access.
            currentTweaks = new SortedDictionary<int, IItemTweak[]>
            {
                #region CATEGORY 1: Weapon Balancing
                { ItemID.AdamantiteGlaive, Do(TrueMelee, DamageExact(66), UseExact(7), ShootSpeedExact(18f)) }, // VANILLA: 49 DMG, 25 UT, 5 VEL
                { ItemID.AdamantiteRepeater, Do(UseExact(14)) }, // VANILLA: 18 UT
                { ItemID.AdamantiteSword, Do(UseTurn, DamageExact(72), UseExact(7)) }, // VANILLA: 61 DMG, 21 UT
                { ItemID.AmberStaff, Do(UseTimeExact(18), UseAnimationExact(36), ReuseDelayExact(15)) }, // VANILLA: 28 UT/UA, 0 RUD
                { ItemID.AmethystStaff, Do(ManaExact(2)) }, // VANILLA: 5 MANA
                { ItemID.Anchor, Do(DamageExact(107)) }, // VANILLA: 70 DMG
                { ItemID.AntlionClaw, Do(UseExact(10)) }, // Mandible Blade. VANILLA: 18 UT
                { ItemID.AquaScepter, Do(DamageRatio(0.9f)) }, // VANILLA: 27 DMG. Uses ratios due to remix seed
                { ItemID.Bananarang, Do(DamageExact(60)) }, // VANILLA: 45 DMG
                { ItemID.BatScepter, Do(DamageExact(50)) }, // VANILLA: 45 DMG
                { ItemID.BeamSword, Do(UseMeleeSpeed, DamageExact(131), UseAnimationExact(40), KnockbackExact(8)) }, // VANILLA: 52 DMG, 20 UT, 6.5 KB
                { ItemID.BeeGun, Do(DamageExact(11)) }, // VANILLA: 9 DMG
                { ItemID.Beenade, Do(UseTimeExact(22), ShootSpeedExact(10f)) }, // VANILLA: 15 UT, 6 VEL
                { ItemID.BeesKnees, Do(DamageExact(21), UseExact(35)) }, // VANILLA: 23 DMG, 23 UT
                { ItemID.Bladetongue, Do(UseTurn, DamageExact(120), UseExact(22)) }, // VANILLA: 55 DMG, 28 UT
                { ItemID.BlizzardStaff, Do(DamageExact(41)) }, // VANILLA: 58 DMG
                { ItemID.BloodyMachete, Do(DamageExact(24)) }, // VANILLA: 20 DMG
                { ItemID.Blowgun, Do(DamageExact(40), ShootSpeedExact(15f)) }, // VANILLA: 27 DMG, 13 VEL
                { ItemID.BluePhaseblade, phaseblade },
                { ItemID.BluePhasesaber, phasesaber },
                { ItemID.BoneSword, Do(UseTurn, DamageExact(25)) }, // VANILLA: 19 DMG
                { ItemID.BookofSkulls, Do(ShootSpeedExact(5.5f)) }, // VANILLA: 3.5 VEL
                { ItemID.Boomstick, Do(DamageExact(11)) }, // VANILLA: 14 DMG
                { ItemID.BreakerBlade, Do(UseTurn, DamageExact(160)) }, // VANILLA: 70 DMG
                { ItemID.CandyCornRifle, Do(DamageExact(80)) }, // VANILLA: 44 DMG
                { ItemID.Cascade, Do(DamageExact(31)) }, // VANILLA: 27 DMG
                { ItemID.ChainGuillotines, Do(DamageExact(75)) }, // VANILLA: 59 DMG
                { ItemID.ChainGun, Do(DamageExact(35)) }, // VANILLA: 31 DMG
                { ItemID.ChainKnife, Do(DamageRatio(1.34f)) },  // VANILLA: 12 DMG. Uses ratios due to remix seed
                { ItemID.ChlorophyteClaymore, Do(UseMeleeSpeed, DamageExact(180), UseExact(45), ShootSpeedExact(22f)) }, // VANILLA: 95 DMG, 26 UT, 8 VEL
                { ItemID.ChlorophytePartisan, Do(UseMeleeSpeed, DamageExact(80)) }, // VANILLA: 49 DMG
                { ItemID.ChlorophyteSaber, Do(UseMeleeSpeed, DamageExact(92), UseExact(10)) }, // VANILLA: 57 DMG, 16 UT
                { ItemID.ChristmasTreeSword, Do(UseTurn, UseMeleeSpeed, DamageExact(80), UseExact(30)) }, // VANILLA: 86 DMG, 23 UT
                { ItemID.ClockworkAssaultRifle, Do(DamageExact(24)) }, // VANILLA: 17 DMG
                { ItemID.CobaltNaginata, Do(TrueMelee, DamageExact(60), UseExact(9), ShootSpeedExact(12f)) }, // VANILLA: 44 DMG, 28 UT, 4.3 VEL
                { ItemID.CobaltRepeater, Do(UseExact(17)) }, // VANILLA: 23 UT
                { ItemID.CobaltSword, Do(DamageExact(69), UseExact(9)) }, // VANILLA: 40 DMG, 19 UT
                { ItemID.Code2, Do(DamageExact(43)) }, // VANILLA: 54 DMG
                { ItemID.CoinGun, Do(UseExact(12)) }, // VANILLA: 8 UT
                { ItemID.CorruptYoyo, Do(DamageExact(20)) }, // Malaise. VANILLA: 16 DMG
                { ItemID.CrimsonYoyo, Do(DamageExact(20)) }, // Artery. VANILLA: 17 DMG
                { ItemID.CrystalBullet, Do(DamageExact(6)) }, // VANILLA: 9 DMG
                { ItemID.CrystalDart, Do(DamageExact(20)) }, // VANILLA: 14 DMG
                { ItemID.CrystalStorm, Do(DamageExact(40)) }, // VANILLA: 32 DMG
                { ItemID.CrystalVileShard, Do(DamageExact(35)) }, // VANILLA: 25 DMG
                { ItemID.CursedArrow, Do(DamageExact(14)) }, // VANILLA: 17 DMG
                { ItemID.CursedDart, Do(DamageExact(25)) }, // VANILLA: 9 DMG
                { ItemID.Cutlass, Do(DamageExact(90), UseExact(11)) }, // VANILLA: 53 DMG, 16 UT
                { ItemID.DaedalusStormbow, Do(DamageExact(30)) }, // VANILLA: 38 DMG
                { ItemID.DaoofPow, Do(DamageExact(85)) }, // Displays as 170 damage. VANILLA: 50 DMG (displays as 100)
                { ItemID.DayBreak, Do(DamageExact(125), UseExact(22)) }, // VANILLA: 150 DMG, 16 UT
                { ItemID.DD2BetsyBow, Do(DamageExact(42)) }, // Aerial Bane. VANILLA: 39 DMG. Compensation for removing aerial multiplier
                { ItemID.DD2SquireBetsySword, Do(UseMeleeSpeed, DamageExact(150)) }, // Flying Dragon. VANILLA: 180 DMG
                { ItemID.DeadlySphereStaff, Do(DamageExact(50)) }, // VANILLA: 40 DMG
                { ItemID.DeathSickle, Do(UseMeleeSpeed, DamageExact(65), ShootSpeedExact(15f)) }, // VANILLA: 57 DMG, 9 VEL
                { ItemID.DemonBow, Do(DamageExact(12)) }, // VANILLA: 14 DMG
                { ItemID.DemonScythe, Do(DamageExact(28)) }, // VANILLA: 35 DMG
                { ItemID.DyeTradersScimitar, Do(UseTurn, DamageExact(24)) }, // Exotic Scimitar. VANILLA: 20 DMG
                { ItemID.ElfMelter, Do(ShootSpeedDelta(+5f)) }, // VANILLA: 8.5 VEL
                { ItemID.EmeraldStaff, Do(DamageExact(27)) }, // VANILLA: 19 DMG
                { ItemID.EmpressBlade, Do(DamageExact(68)) }, // Terraprisma. VANILLA: 90 DMG
                { ItemID.EnchantedBoomerang, Do(DamageExact(21)) }, // VANILLA: 17 DMG
                { ItemID.EnchantedSword, Do(UseMeleeSpeed) },
                { ItemID.EndlessMusketPouch, Do(DamageExact(8)) }, // VANILLA: 7 DMG. Gives it the same damage as Tungsten Bullet
                { ItemID.Excalibur, Do(TrueMelee, DamageExact(170)) }, // VANILLA: 72 DMG
                { ItemID.FairyQueenMagicItem, Do(DamageExact(54)) }, // Nightglow. VANILLA: 50 DMG
                { ItemID.FalconBlade, Do(UseTurn, UseExact(13)) }, // VANILLA: 20 UT
                { ItemID.FireworksLauncher, Do(DamageExact(50), UseExact(25)) }, // Celebration. VANILLA: 25 DMG, 30 UT
                { ItemID.Flamarang, Do(DamageExact(40)) }, // VANILLA: 49 DMG
                { ItemID.Flamelash, Do(DamageExact(36)) }, // VANILLA: 32 DMG
                { ItemID.Flamethrower, Do(DamageExact(21), ShootSpeedDelta(+3f)) }, // VANILLA: 35 DMG, 7 VEL
                { ItemID.FlowerofFire, Do(DamageRatio(0.78f)) }, // VANILLA: 48 DMG. Uses ratios due to remix seed
                { ItemID.FlowerPow, Do(DamageExact(80)) }, // Displays as 160 damage. VANILLA: 65 DMG (displays as 130)
                { ItemID.FlyingKnife, Do(DamageExact(53)) }, // VANILLA: 40 DMG
                { ItemID.Frostbrand, Do(UseMeleeSpeed, DamageExact(110)) }, // VANILLA: 49 DMG
                { ItemID.Gatligator, Do(UseExact(6)) }, // VANILLA: 7 UT
                { ItemID.GoldShortsword, Do(TrueMelee, DamageExact(17)) }, // VANILLA: 12 DMG
                { ItemID.GolemFist, Do(DamageExact(150)) }, // VANILLA: 90 DMG
                { ItemID.Gradient, Do(DamageExact(39)) }, // VANILLA: 49 DMG
                { ItemID.GreenPhaseblade, phaseblade },
                { ItemID.GreenPhasesaber, phasesaber },
                { ItemID.GrenadeLauncher, Do(DamageExact(105)) }, // VANILLA: 60 DMG
                { ItemID.Gungnir, Do(TrueMelee, DamageExact(130), ShootSpeedExact(7f)) }, // VANILLA: 61 DMG, 5.6 VEL
                { ItemID.HallowedRepeater, Do(UseExact(12)) }, // VANILLA: 17 UT
                { ItemID.Handgun, Do(UseExact(20)) }, // VANILLA: 15 UT
                { ItemID.HighVelocityBullet, Do(DamageExact(13)) }, // VANILLA: 11 DMG
                { ItemID.HiveFive, Do(DamageExact(27)) }, // VANILLA: 24 DMG
                { ItemID.HornetStaff, Do(DamageExact(18)) }, // VANILLA: 12 DMG
                { ItemID.IceBlade, Do(UseMeleeSpeed) },
                { ItemID.IceBoomerang, Do(ShootSpeedExact(9), UseExact(25)) }, // VANILLA: 20 UT, 11.5 VEL
                { ItemID.IceRod, Do(ShootSpeedExact(20), UseExact(6)) }, // VANILLA: 9 UT, 12 VEL
                { ItemID.IceSickle, Do(UseMeleeSpeed, DamageExact(75), ShootSpeedExact(20f)) }, // VANILLA: 50 DMG, 12 VEL
                { ItemID.IchorArrow, Do(DamageExact(13)) }, // VANILLA: 16 DMG
                { ItemID.IchorBullet, Do(DamageExact(11)) }, // VANILLA: 13 DMG
                { ItemID.ImpStaff, Do(DamageExact(25)) }, // VANILLA: 17 DMG
                { ItemID.InfernoFork, Do(DamageExact(83), ShootSpeedExact(11)) }, // VANILLA: 70 DMG, 8 VEL
                { ItemID.InfluxWaver, Do(UseMeleeSpeed, DamageExact(80), UseExact(25)) }, // VANILLA: 100 DMG, 20 UT
                { ItemID.IronShortsword, Do(TrueMelee, DamageExact(10)) }, // VANILLA: 8 DMG
                { ItemID.Keybrand, Do(UseTurn) },
                { ItemID.KOCannon, Do(DamageRatio(3f)) }, // VANILLA: 40 DMG. Uses ratios due to remix seed
                { ItemID.Kraken, Do(DamageExact(65)) }, // VANILLA: 95 DMG
                { ItemID.LaserMachinegun, Do(DamageExact(49)) }, // VANILLA: 60 DMG
                { ItemID.LaserRifle, Do(DamageExact(46)) }, // VANILLA: 29 DMG
                { ItemID.LastPrism, Do(DamageExact(57)) }, // VANILLA: 100 DMG
                { ItemID.LeadShortsword, Do(TrueMelee, DamageExact(11)) }, // VANILLA: 9 DMG
                { ItemID.LeafBlower, Do(DamageExact(61)) }, // VANILLA: 48 DMG
                { ItemID.LightDisc, Do(DamageExact(80), ShootSpeedExact(18)) }, // VANILLA: 60 DMG, 16 VEL
                { ItemID.LunarFlareBook, Do(DamageExact(110)) }, // VANILLA: 100 DMG
                { ItemID.MagicalHarp, Do(DamageExact(50), ShootSpeedExact(12f)) }, // VANILLA: 42 DMG, 4.5 VEL
                { ItemID.MagicMissile, Do(DamageExact(23), UseAnimationExact(20), UseTimeExact(10)) }, // VANILLA: 35 DMG, 22 UT/UA
                { ItemID.Marrow, Do(DamageExact(60)) }, // VANILLA: 53 DMG
                { ItemID.MedusaHead, Do(DamageExact(75)) }, // VANILLA: 40 DMG
                { ItemID.Meowmere, Do(UseMeleeSpeed) },
                { ItemID.MiniNukeI, Do(DamageExact(90)) }, // VANILLA: 75 DMG
                { ItemID.MiniNukeII, Do(DamageExact(90)) },
                { ItemID.Minishark, Do(DamageExact(4)) }, // VANILLA: 6 DMG
                { ItemID.MoltenFury, Do(UseExact(28)) }, // VANILLA: 22 UT
                { ItemID.MonkStaffT1, Do(TrueMeleeNoSpeed, DamageExact(83)) }, // Sleepy Octopod. VANILLA: 50 DMG
                { ItemID.MonkStaffT2, Do(TrueMelee, DamageExact(90)) }, // Ghastly Glaive. VANILLA: 45 DMG
                { ItemID.MonkStaffT3, Do(DamageExact(225)) }, // Sky Dragon's Fury. VANILLA: 140 DMG
                { ItemID.MoonlordBullet, Do(DamageExact(17)) }, // Luminite Bullet. VANILLA: 20 DMG
                { ItemID.MoonlordTurretStaff, Do(DamageExact(50)) }, // Lunar Portal Staff. VANILLA: 100 DMG
                { ItemID.MushroomSpear, Do(TrueMelee, DamageExact(100), UseExact(32)) }, // VANILLA: 60 DMG, 40 UT
                { ItemID.Musket, Do(DamageExact(22)) }, // VANILLA: 31 DMG
                { ItemID.MythrilHalberd, Do(TrueMelee, DamageExact(65), UseExact(8), ShootSpeedExact(15f)) }, // VANILLA: 45 DMG, 26 UT, 4.5 VEL
                { ItemID.MythrilRepeater, Do(UseExact(16)) }, // VANILLA: 20 UT
                { ItemID.MythrilSword, Do(UseTurn, DamageExact(70), UseExact(8)) }, // VANILLA: 50 DMG, 20 UT
                { ItemID.NailGun, Do(DamageExact(77)) }, // VANILLA: 85 DMG
                { ItemID.NettleBurst, Do(DamageExact(65)) }, // VANILLA: 35 DMG
                { ItemID.NightsEdge, Do(TrueMelee, DamageExact(45)) }, // VANILLA: 40 DMG
                { ItemID.NorthPole, Do(UseMeleeSpeed) },
                { ItemID.OrangePhaseblade, phaseblade },
                { ItemID.OrangePhasesaber, phasesaber },
                { ItemID.OrichalcumHalberd, Do(TrueMelee, DamageExact(128), ShootSpeedExact(6f)) }, // VANILLA: 46 DMG, 4.5 VEL
                { ItemID.OrichalcumRepeater, Do(DamageExact(50)) }, // VANILLA: 40 DMG
                { ItemID.OrichalcumSword, Do(UseTurn, DamageExact(175)) }, // VANILLA: 59 DMG
                { ItemID.PaladinsHammer, Do(DamageExact(95), ShootSpeedExact(28)) }, // VANILLA: 90 DMG, 14 VEL
                { ItemID.PalladiumPike, Do(TrueMelee, DamageExact(120), ShootSpeedExact(5.4f)) }, // VANILLA: 44 DMG, 4.4 VEL
                { ItemID.PalladiumRepeater, Do(DamageExact(48)) }, // VANILLA: 37 DMG
                { ItemID.PalladiumSword, Do(DamageExact(150)) }, // VANILLA: 49 DMG
                { ItemID.PearlwoodBow, Do(DamageExact(32)) }, // VANILLA: 12 DMG
                { ItemID.PearlwoodSword, Do(UseTurn, DamageExact(45)) }, // VANILLA: 30 DMG
                { ItemID.PewMaticHorn, Do(DamageExact(25), ShootSpeedExact(15)) }, // VANILLA: 20 DMG, 14 VEL
                { ItemID.PhoenixBlaster, Do(UseExact(20)) }, // VANILLA: 14 UT
                { ItemID.PlatinumShortsword, Do(TrueMelee, DamageExact(18)) }, // VANILLA: 13 DMG
                { ItemID.PoisonStaff, Do(DamageExact(57)) }, // VANILLA: 43 DMG
                { ItemID.PossessedHatchet, Do(DamageExact(135)) }, // VANILLA: 80 DMG
                { ItemID.PrincessWeapon, Do(DamageExact(80)) }, // Resonance Scepter. VANILLA: 70 DMG
                { ItemID.PsychoKnife, Do(UseTurn, DamageExact(200), UseExact(11)) }, // VANILLA: 85 DMG, 8 UT
                { ItemID.PurpleClubberfish, Do(UseTurn, KnockbackExact(10f)) }, // VANILLA: 8 KB
                { ItemID.PurplePhaseblade, phaseblade },
                { ItemID.PurplePhasesaber, phasesaber },
                { ItemID.PygmyStaff, Do(DamageExact(60)) }, // VANILLA: 40 DMG
                { ItemID.RainbowGun, Do(DamageExact(60)) }, // VANILLA: 45 DMG
                { ItemID.RainbowRod, Do(DamageExact(40), KnockbackExact(8f)) }, // VANILLA: 50 DMG, 6 KB
                { ItemID.Rally, Do(DamageExact(18)) }, // VANILLA: 14 DMG
                { ItemID.RavenStaff, Do(DamageExact(36)) }, // VANILLA: 55 DMG
                { ItemID.RazorbladeTyphoon, Do(DamageExact(103)) }, // VANILLA: 85 DMG
                { ItemID.Razorpine, Do(DamageExact(40)) }, // VANILLA: 48 DMG
                { ItemID.RedPhaseblade, phaseblade },
                { ItemID.RedPhasesaber, phasesaber },
                { ItemID.RedRyder, Do(DamageExact(24)) }, // VANILLA: 20 DMG
                { ItemID.RedsYoyo, Do(DamageExact(48)) }, // VANILLA: 70 DMG. Has the same stats as Valkyrie Yoyo
                { ItemID.RocketLauncher, Do(DamageExact(60), ShootSpeedExact(9)) }, // VANILLA: 55 DMG, 5 VEL
                { ItemID.Sandgun, Do(UseExact(20)) }, // VANILLA: 16 UT
                { ItemID.SapphireStaff, Do(DamageExact(25)) }, // VANILLA: 18 DMG
                { ItemID.ScourgeoftheCorruptor, Do(DamageExact(63)) }, // VANILLA: 70 DMG
                { ItemID.Seedler, Do(UseMeleeSpeed, DamageExact(45), ShootSpeedExact(16)) }, // VANILLA: 50 DMG, 12 VEL
                { ItemID.ShadowbeamStaff, Do(DamageExact(100)) }, // VANILLA: 80 DMG
                { ItemID.ShadowFlameBow, Do(DamageExact(55)) }, // VANILLA: 47 DMG
                { ItemID.ShadowFlameHexDoll, Do(DamageExact(40), ShootSpeedExact(30)) }, // VANILLA: 32 DMG, 9 VEL
                { ItemID.ShadowFlameKnife, Do(DamageExact(50)) }, // VANILLA: 38 DMG
                { ItemID.SharpTears, Do(DamageExact(49)) }, // Blood Thorn. VANILLA: 34 DMG
                { ItemID.Shotgun, Do(DamageExact(36)) }, // VANILLA: 24 DMG
                { ItemID.SilverBullet, Do(DamageExact(8)) }, // VANILLA: 9 DMG
                { ItemID.SilverShortsword, Do(TrueMelee, DamageExact(14)) }, // VANILLA: 9 DMG
                { ItemID.SkyFracture, Do(DamageExact(46)) }, // VANILLA: 38 DMG
                { ItemID.SlapHand, Do(UseTurn, DamageExact(120)) }, // VANILLA: 55 DMG
                { ItemID.Smolstar, Do(DamageExact(9)) }, // Blade Staff. VANILLA: 6 DMG
                { ItemID.SniperRifle, Do(DamageExact(200), UseExact(40)) }, // VANILLA: 185 DMG, 36 UT
                { ItemID.SoulDrain, Do(DamageExact(38)) }, // Life Drain. VANILLA: 35 DMG
                { ItemID.SpaceGun, Do(DamageExact(23)) }, // VANILLA: 17 DMG
                { ItemID.Spear, Do(TrueMelee, DamageExact(14)) }, // VANILLA: 8 DMG
                { ItemID.SpectreStaff, Do(DamageExact(72)) }, // VANILLA: 65 DMG
                { ItemID.SpiritFlame, Do(UseExact(20), ShootSpeedExact(2f)) }, // VANILLA: 22 UT, 3 VEL
                { ItemID.StaffofEarth, Do(DamageExact(150)) }, // VANILLA: 125 DMG
                { ItemID.StarCannon, Do(UseExact(18)) }, // VANILLA: 12 UT
                { ItemID.StardustDragonStaff, Do(DamageExact(24)) }, // VANILLA: 40 DMG
                { ItemID.StormTigerStaff, Do(DamageExact(49)) }, // Desert Tiger Staff. VANILLA: 41 DMG
                { ItemID.StylistKilLaKillScissorsIWish, Do(UseTurn, DamageExact(21)) }, // Stylish Scissors. VANILLA: 14 DMG
                { ItemID.Stynger, Do(DamageExact(75)) }, // VANILLA: 45 DMG
                { ItemID.Swordfish, Do(TrueMelee, DamageExact(21)) }, // VANILLA: 19 DMG
                { ItemID.TacticalShotgun, Do(DamageExact(34)) }, // VANILLA: 29 DMG
                { ItemID.TaxCollectorsStickOfDoom, Do(UseTurn, DamageExact(80), UseExact(10)) }, // Classy Cane. VANILLA: 16 DMG, 15 UT
                { ItemID.TendonBow, Do(DamageExact(17)) }, // VANILLA: 19 DMG
                { ItemID.TerraBlade, Do(DamageExact(95)) }, // VANILLA: 85 DMG
                { ItemID.Terragrim, Do(TrueMeleeNoSpeed, DamageExact(13)) }, // VANILLA: 17 DMG
                { ItemID.Terrarian, Do(DamageExact(90)) }, // VANILLA: 190 DMG. Required due to fixing iframes so yoyo and shots can hit simultaneously
                { ItemID.TheEyeOfCthulhu, Do(DamageExact(80)) }, // VANILLA: 115 DMG
                { ItemID.TheMeatball, Do(DamageExact(24)) }, // Displays as 48 damage. VANILLA: 17 DMG (displays as 34)
                { ItemID.TheRottedFork, Do(TrueMelee, DamageExact(20)) }, // VANILLA: 17 DMG
                { ItemID.TheUndertaker, Do(DamageExact(15)) }, // VANILLA: 19 DMG
                { ItemID.ThunderSpear, Do(UseMeleeSpeed) }, // Storm Spear
                { ItemID.ThunderStaff, Do(DamageExact(15)) }, // Thunder Zapper. VANILLA: 20 DMG
                { ItemID.TitaniumRepeater, Do(DamageExact(52)) }, // VANILLA: 43 DMG
                { ItemID.TitaniumSword, Do(UseTurn, DamageExact(185)) }, // VANILLA: 61 DMG
                { ItemID.TitaniumTrident, Do(TrueMelee, DamageExact(144), ShootSpeedExact(6.5f)) }, // VANILLA: 48 DMG, 5 VEL
                { ItemID.TopazStaff, Do(ManaExact(2)) }, // VANILLA: 5 MANA
                { ItemID.ToxicFlask, Do(DamageExact(62), UseExact(33)) }, // VANILLA: 52 DMG, 45 UT
                { ItemID.Toxikarp, Do(UseExact(9)) }, // VANILLA: 10 UT
                { ItemID.Trident, Do(TrueMelee, DamageExact(20)) }, // VANILLA: 14 DMG
                { ItemID.TrueExcalibur, Do(TrueMelee, DamageExact(112)) }, // VANILLA: 72 DMG
                { ItemID.TrueNightsEdge, Do(DamageExact(105)) }, // VANILLA: 70 DMG
                { ItemID.Tsunami, Do(DamageExact(45)) }, // VANILLA: 53 DMG
                { ItemID.TungstenBullet, Do(DamageExact(8)) }, // VANILLA: 9 DMG
                { ItemID.TungstenShortsword, Do(TrueMelee, DamageExact(15)) }, // VANILLA: 10 DMG
                { ItemID.UnholyArrow, Do(DamageExact(11)) }, // VANILLA: 12 DMG
                { ItemID.UnholyTrident, Do(DamageRatio(0.91f)) }, // VANILLA: 88 DMG. Uses ratios due to remix seed
                { ItemID.Uzi, Do(UseExact(8)) }, // VANILLA: 9 UT
                { ItemID.ValkyrieYoyo, Do(DamageExact(48)) }, // VANILLA: 70 DMG. Has the same stats as Red's Throw
                { ItemID.WaspGun, Do(DamageExact(41)) }, // VANILLA: 31 DMG
                { ItemID.WaterBolt, Do(DamageExact(23)) }, // VANILLA: 19 DMG
                { ItemID.WhitePhaseblade, phaseblade },
                { ItemID.WhitePhasesaber, phasesaber },
                { ItemID.WoodenBoomerang, Do(DamageExact(16), Value(Item.sellPrice(copper: 20))) }, // VANILLA: 10 DMG
                { ItemID.Yelets, Do(DamageExact(53)) }, // VANILLA: 60 DMG
                { ItemID.YellowPhaseblade, phaseblade },
                { ItemID.YellowPhasesaber, phasesaber },
                { ItemID.Zenith, Do(DamageExact(210)) }, // VANILLA: 190 DMG
                { ItemID.ZombieArm, Do(UseTurn, KnockbackExact(12f)) }, // VANILLA: 5.5 KB
                #endregion

                #region CATEGORY 2: Defense Balancing
                // Valhalla Knight armor
                { ItemID.SquireAltHead, Do(DefenseDelta(-2)) }, // VANILLA: 20 DEF
                { ItemID.SquireAltPants, Do(DefenseDelta(-3)) }, // VANILLA: 24 DEF
                { ItemID.SquireAltShirt, Do(DefenseDelta(-3)) }, // VANILLA: 24 DEF
                { ItemID.SquireGreatHelm, Do(DefenseDelta(-1)) }, // VANILLA: 13 DEF
                { ItemID.SquireGreaves, Do(DefenseDelta(-2)) }, // VANILLA: 18 DEF
                { ItemID.SquirePlating, Do(DefenseDelta(-4)) }, // VANILLA: 27 DEF
                #endregion

                #region CATEGORY 3: Tool Balancing
                { ItemID.AcornAxe, Do(AxePower(100)) }, // Axe of Regrowth. VANILLA: 150% AXE
                { ItemID.AdamantiteChainsaw, Do(TrueMeleeNoSpeed, DamageExact(58), AxePower(90), TileBoostExact(+0)) }, // VANILLA: 33 DMG, 100 AXE, -1 TILE
                { ItemID.AdamantiteDrill, Do(TrueMeleeNoSpeed, DamageExact(33), TileBoostExact(+0)) }, // VANILLA: 20 DMG, -1 TILE
                { ItemID.AdamantitePickaxe, Do(TileBoostExact(+1)) }, // VANILLA: 0 TILE
                { ItemID.AdamantiteWaraxe, Do(UseTimeExact(10), AxePower(160), TileBoostExact(+1)) }, // VANILLA: 8 SPD, 100 AXE, 0 TILE
                { ItemID.BloodLustCluster, Do(UseTimeExact(13), AxePower(100)) }, // VANILLA: 15 SPD, 75 AXE
                { ItemID.BonePickaxe, Do(UseTimeExact(6)) }, // VANILLA: 11 SPD
                { ItemID.BorealWoodHammer, Do(UseTimeExact(11), HammerPower(25)) }, // VANILLA: 23 SPD, 35 HAM
                { ItemID.ButchersChainsaw, Do(TrueMeleeNoSpeed, DamageExact(100), UseTimeExact(3), TileBoostExact(+0)) }, // VANILLA: 4 SPD, -1 TILE
                { ItemID.CactusPickaxe, Do(UseTimeExact(9)) }, // VANILLA: 16 SPD
                { ItemID.CnadyCanePickaxe, Do(UseTimeExact(9), TileBoostExact(+1)) }, // Candy Cane Pickaxe. VANILLA: 16 SPD
                { ItemID.ChlorophyteChainsaw, Do(TrueMeleeNoSpeed, DamageExact(88), UseTimeExact(3), AxePower(120), TileBoostExact(+1)) }, // VANILLA: 50 DMG, 4 SPD, 115 AXE, 0 TILE
                { ItemID.ChlorophyteDrill, Do(TrueMeleeNoSpeed, DamageExact(45), TileBoostExact(+1)) }, // VANILLA: 35 DMG, 0 TILE
                { ItemID.ChlorophyteGreataxe, Do(AxePower(165), TileBoostExact(+2)) }, // VANILLA: 115 AXE, +1 TILE
                { ItemID.ChlorophyteJackhammer, Do(TrueMeleeNoSpeed, DamageExact(58), UseTimeExact(5), TileBoostExact(+1)) }, // VANILLA: 45 DMG, 4 SPD, 0 TILE
                { ItemID.ChlorophytePickaxe, Do(TileBoostExact(+2)) }, // VANILLA: +1 TILE
                { ItemID.ChlorophyteWarhammer, Do(UseTimeExact(8), TileBoostExact(+2)) }, // VANILLA: 14 SPD, +1 TILE
                { ItemID.CobaltChainsaw, Do(TrueMeleeNoSpeed, DamageExact(42), UseTimeExact(4)) }, // VANILLA: 23 DMG, 7 SPD
                { ItemID.CobaltDrill, Do(TrueMeleeNoSpeed, DamageExact(13), UseTimeExact(5), PickPower(130)) }, // VANILLA: 10 DMG, 7 SPD, 110 PICK
                { ItemID.CobaltPickaxe, Do(UseTimeExact(9), PickPower(130)) }, // VANILLA: 13 SPD, 110 PICK
                { ItemID.CobaltWaraxe, Do(UseTimeExact(12), AxePower(125)) }, // VANILLA: 13 SPD, 70 AXE
                { ItemID.CopperAxe, Do(UseTimeExact(16), AxePower(50), TileBoostExact(+0)) }, // VANILLA: 21 SPD, 35 AXE, -1 TILE
                { ItemID.CopperHammer, Do(UseTimeExact(12), TileBoostExact(+0)) }, // VANILLA: 23 SPD, -1 TILE
                { ItemID.CopperPickaxe, Do(UseTimeExact(10), TileBoostExact(+0)) }, // VANILLA: 15 SPD, -1 TILE
                { ItemID.DeathbringerPickaxe, Do(UseTimeExact(10)) }, // VANILLA: 14 SPD
                { ItemID.Drax, Do(TrueMeleeNoSpeed, DamageExact(62), TileBoostExact(+0)) }, // VANILLA: 35 DMG, -1 TILE
                { ItemID.EbonwoodHammer, Do(UseTimeExact(9), HammerPower(25)) }, // VANILLA: 20 SPD, 40 HAM
                { ItemID.FleshGrinder, Do(UseTimeExact(13), HammerPower(70)) }, // VANILLA: 19 SPD, 55 HAM
                { ItemID.GoldAxe, Do(UseTimeExact(14), AxePower(80)) }, // VANILLA: 18 SPD, 55 AXE
                { ItemID.GoldHammer, Do(UseTimeExact(9), HammerPower(60)) }, // VANILLA: 23 SPD, 55 HAM
                { ItemID.GoldPickaxe, Do(UseTimeExact(9)) }, // VANILLA: 17 SPD
                { ItemID.Hammush, Do(UseTimeExact(10), TileBoostExact(+1)) }, // VANILLA: 14 SPD, 0 TILE
                { ItemID.IronAxe, Do(UseTimeExact(15), AxePower(60)) }, // VANILLA: 19 SPD, 45 AXE
                { ItemID.IronHammer, Do(UseTimeExact(11), HammerPower(45)) }, // VANILLA: 20 SPD, 40 HAM
                { ItemID.IronPickaxe, Do(UseTimeExact(8)) }, // VANILLA: 13 SPD
                { ItemID.LaserDrill, Do(DamageExact(50), UseTimeExact(4), PickPower(220)) }, // VANILLA: 35 DMG, 6 SPD, 230 PICK
                { ItemID.LeadAxe, Do(UseTimeExact(15), AxePower(60)) }, // VANILLA: 19 SPD, 50 AXE
                { ItemID.LeadHammer, Do(UseTimeExact(11), HammerPower(45)) }, // VANILLA: 19 SPD, 43 HAM
                { ItemID.LeadPickaxe, Do(UseTimeExact(8), PickPower(40)) }, // VANILLA: 12 SPD, 43 PICK
                { ItemID.LucyTheAxe, Do(UseExact(13), TileBoostExact(+1)) }, // VANILLA: 15 SPD/UT, 0 TILE
                { ItemID.LunarHamaxeNebula, Do(UseTimeExact(5), AxePower(175)) }, // VANILLA: 7 SPD, 150 AXE
                { ItemID.LunarHamaxeSolar, Do(UseTimeExact(5), AxePower(175)) },
                { ItemID.LunarHamaxeStardust, Do(UseTimeExact(5), AxePower(175)) },
                { ItemID.LunarHamaxeVortex, Do(UseTimeExact(5), AxePower(175)) },
                { ItemID.MeteorHamaxe, Do(HammerPower(70)) }, // VANILLA: 60 HAM
                { ItemID.MoltenHamaxe, Do(AxePower(125), HammerPower(75)) }, // VANILLA: 150 AXE, 70 HAM
                { ItemID.MoltenPickaxe, Do(UseTimeExact(10)) }, // VANILLA: 18 SPD
                { ItemID.MythrilChainsaw, Do(TrueMeleeNoSpeed, DamageExact(52), UseTimeExact(4), AxePower(80)) }, // VANILLA: 29 DMG, 6 SPD, 85 AXE
                { ItemID.MythrilDrill, Do(TrueMeleeNoSpeed, DamageExact(20), UseTimeExact(4), PickPower(160)) }, // VANILLA: 15 DMG, 6 SPD, 150 PICK
                { ItemID.MythrilPickaxe, Do(UseTimeExact(8), PickPower(160)) }, // VANILLA: 10 SPD, 150 PICK
                { ItemID.MythrilWaraxe, Do(UseTimeExact(11), AxePower(140)) }, // VANILLA: 10 SPD, 85 AXE
                { ItemID.NebulaDrill, Do(TrueMeleeNoSpeed, DamageExact(70), UseTimeExact(3), TileBoostExact(+3)) }, // VANILLA: 50 DMG, 2 SPD, +2 TILE
                { ItemID.NightmarePickaxe, Do(UseTimeExact(9), PickPower(66)) }, // VANILLA: 15 SPD, 65 PICK
                { ItemID.OrichalcumChainsaw, Do(TrueMeleeNoSpeed, DamageExact(54), UseTimeExact(4), AxePower(80)) }, // VANILLA: 31 DMG, 5 SPD, 90 AXE
                { ItemID.OrichalcumDrill, Do(TrueMeleeNoSpeed, DamageExact(22), UseTimeExact(4), PickPower(160)) }, // VANILLA: 17 DMG, 5 SPD, 165 PICK
                { ItemID.OrichalcumPickaxe, Do(UseTimeExact(8), PickPower(160)) }, // VANILLA: 9 SPD, 165 PICK
                { ItemID.OrichalcumWaraxe, Do(UseTimeExact(11), AxePower(140)) }, // VANILLA: 9 SPD, 90 AXE
                { ItemID.PalladiumChainsaw, Do(TrueMeleeNoSpeed, DamageExact(44), UseTimeExact(4), AxePower(70)) }, // VANILLA: 26 DMG, 7 SPD, 75 AXE
                { ItemID.PalladiumDrill, Do(TrueMeleeNoSpeed, DamageExact(15), UseTimeExact(5)) }, // VANILLA: 12 DMG, 7 SPD
                { ItemID.PalladiumPickaxe, Do(UseTimeExact(9)) }, // VANILLA: 12 SPD
                { ItemID.PalladiumWaraxe, Do(AxePower(125)) }, // VANILLA: 75 AXE
                { ItemID.PalmWoodHammer, Do(UseTimeExact(11), HammerPower(25)) }, // VANILLA: 23 SPD, 35 HAM
                { ItemID.PearlwoodHammer, Do(DamageExact(36), UseTimeExact(4), UseAnimationExact(20), HammerPower(25)) }, // VANILLA: 10 DMG, 19 SPD/UT, 55 HAM
                { ItemID.PickaxeAxe, Do(TileBoostExact(+1)) }, // VANILLA: 0 TILE
                { ItemID.PlatinumAxe, Do(UseTimeExact(14), AxePower(80)) }, // VANILLA: 17 SPD, 60 AXE
                { ItemID.PlatinumHammer, Do(UseTimeExact(9), HammerPower(60)) }, // VANILLA: 21 SPD, 59 HAM
                { ItemID.PlatinumPickaxe, Do(UseTimeExact(9), PickPower(55)) }, // VANILLA: 15 SPD, 59 PICK
                { ItemID.Pwnhammer, Do(UseTimeExact(11), TileBoostExact(+1)) }, // VANILLA: 14 SPD, 0 TILE
                { ItemID.RichMahoganyHammer, Do(UseTimeExact(10), HammerPower(25)) }, // VANILLA: 23 SPD, 35 HAM
                { ItemID.Rockfish, Do(UseTimeExact(10), HammerPower(50)) }, // VANILLA: 14 SPD, 70 HAM
                { ItemID.SawtoothShark, Do(TrueMeleeNoSpeed, AxePower(45)) }, // VANILLA: 70 AXE
                { ItemID.ShadewoodHammer, Do(UseTimeExact(9), HammerPower(25)) }, // VANILLA: 20 SPD, 40 HAM
                { ItemID.SilverAxe, Do(UseTimeExact(14), AxePower(70)) }, // VANILLA: 18 SPD, 50 AXE
                { ItemID.SilverHammer, Do(UseTimeExact(10), HammerPower(55)) }, // VANILLA: 19 SPD, 45 HAM
                { ItemID.SilverPickaxe, Do(PickPower(50)) }, // VANILLA: 45 PICK
                { ItemID.SolarFlareDrill, Do(TrueMeleeNoSpeed, DamageExact(70), UseTimeExact(3), TileBoostExact(+3)) }, // VANILLA: 50 DMG, 2 SPD, +2 TILE
                { ItemID.SpectreHamaxe, Do(AxePower(170), TileBoostExact(+4)) }, // VANILLA: 150 AXE, +3 TILE
                { ItemID.SpectrePickaxe, Do(TileBoostExact(+4)) }, // VANILLA: +3 TILE
                { ItemID.StardustDrill, Do(TrueMeleeNoSpeed, DamageExact(70), UseTimeExact(3), TileBoostExact(+3)) }, // VANILLA: 50 DMG, 2 SPD, +2 TILE
                { ItemID.TheBreaker, Do(UseTimeExact(13), HammerPower(70)) }, // VANILLA: 19 SPD, 55 HAM
                { ItemID.TinAxe, Do(UseTimeExact(16), AxePower(50)) }, // VANILLA: 20 SPD, 40 AXE
                { ItemID.TinHammer, Do(UseTimeExact(12), HammerPower(35)) }, // VANILLA: 21 SPD, 38 HAM
                { ItemID.TinPickaxe, Do(UseTimeExact(10)) }, // VANILLA: 14 SPD
                { ItemID.TitaniumChainsaw, Do(TrueMeleeNoSpeed, DamageExact(60), AxePower(90), TileBoostExact(+0)) }, // VANILLA: 34 DMG, 105 AXE, -1 TILE
                { ItemID.TitaniumDrill, Do(TrueMeleeNoSpeed, DamageExact(35), PickPower(180), TileBoostExact(+0)) }, // VANILLA: 27 DMG, 190 PICK, -1 TILE
                { ItemID.TitaniumPickaxe, Do(PickPower(180), UseTimeExact(8), TileBoostExact(+1)) }, // VANILLA: 7 SPD, 190 PICK, 0 TILE
                { ItemID.TitaniumWaraxe, Do(AxePower(160), UseTimeExact(10), TileBoostExact(+1)) }, // VANILLA: 7 SPD, 105 AXE, 0 TILE
                { ItemID.TungstenAxe, Do(UseTimeExact(14), AxePower(70)) }, // VANILLA: 18 SPD, 55 AXE
                { ItemID.TungstenHammer, Do(UseTimeExact(10), HammerPower(55)) }, // VANILLA: 25 SPD, 50 HAM
                { ItemID.TungstenPickaxe, Do(UseTimeExact(11)) }, // VANILLA: 19 SPD
                { ItemID.VortexDrill, Do(TrueMeleeNoSpeed, DamageExact(70), UseTimeExact(3), TileBoostExact(+3)) }, // VANILLA: 50 DMG, 2 SPD, +2 TILE
                { ItemID.WarAxeoftheNight, Do(UseTimeExact(13), AxePower(100)) }, // VANILLA: 15 SPD, 75 AXE
                { ItemID.WoodenHammer, Do(UseTimeExact(11), TileBoostExact(+0)) }, // VANILLA: 25 SPD, -1 TILE
                #endregion

                #region CATEGORY 4: True Melee support
                { ItemID.Arkhalis, trueMeleeNoSpeed },
                { ItemID.CopperShortsword, trueMelee },
                { ItemID.DarkLance, trueMelee },
                { ItemID.Gladius, trueMelee },
                { ItemID.HallowJoustingLance, trueMelee },
                { ItemID.JoustingLance, trueMelee },
                { ItemID.NebulaChainsaw, trueMeleeNoSpeed },
                { ItemID.ObsidianSwordfish, trueMelee },
                { ItemID.PiercingStarlight, trueMelee }, // Starlight
                { ItemID.Ruler, trueMelee },
                { ItemID.ShadowJoustingLance, trueMelee },
                { ItemID.SolarFlareChainsaw, trueMeleeNoSpeed },
                { ItemID.StardustChainsaw, trueMeleeNoSpeed },
                { ItemID.TheHorsemansBlade, trueMelee },
                { ItemID.TinShortsword, trueMelee },
                { ItemID.VortexChainsaw, trueMeleeNoSpeed },
                #endregion

                #region CATEGORY 5: UseTurn
                { ItemID.BeeKeeper, Do(UseTurn) },
                { ItemID.BladeofGrass, Do(UseTurn) },
                { ItemID.BloodButcherer, Do(UseTurn) },
                { ItemID.BorealWoodSword, Do(UseTurn) },
                { ItemID.CactusSword, Do(UseTurn) },
                { ItemID.CandyCaneSword, Do(UseTurn) },
                { ItemID.CopperBroadsword, Do(UseTurn) },
                { ItemID.EbonwoodSword, Do(UseTurn) },
                { ItemID.FieryGreatsword, Do(UseTurn) }, // Volcano
                { ItemID.GoldBroadsword, Do(UseTurn) },
                { ItemID.IronBroadsword, Do(UseTurn) },
                { ItemID.LeadBroadsword, Do(UseTurn) },
                { ItemID.LightsBane, Do(UseTurn) },
                { ItemID.PalmWoodSword, Do(UseTurn) },
                { ItemID.PlatinumBroadsword, Do(UseTurn) },
                { ItemID.RichMahoganySword, Do(UseTurn) },
                { ItemID.ShadewoodSword, Do(UseTurn) },
                { ItemID.SilverBroadsword, Do(UseTurn) },
                { ItemID.TinBroadsword, Do(UseTurn) },
                { ItemID.TungstenBroadsword, Do(UseTurn) },
                { ItemID.WoodenSword, Do(UseTurn) },
                #endregion

                #region CATEGORY 6: Summoning Item Quality of Life
                { ItemID.Abeemination, nonConsumableBossSummon },
                { ItemID.BloodySpine, nonConsumableBossSummon },
                { ItemID.CelestialSigil, nonConsumableBossSummon },
                { ItemID.DeerThing, nonConsumableBossSummon },
                { ItemID.MechanicalEye, nonConsumableBossSummon },
                { ItemID.MechanicalSkull, nonConsumableBossSummon },
                { ItemID.MechanicalWorm, nonConsumableBossSummon },
                { ItemID.MechdusaSummon, nonConsumableBossSummon }, // Ocram's Razor
                { ItemID.QueenSlimeCrystal, nonConsumableBossSummon }, // Gelatin Crystal
                { ItemID.SlimeCrown, nonConsumableBossSummon },
                { ItemID.SuspiciousLookingEye, nonConsumableBossSummon },
                { ItemID.WormFood, nonConsumableBossSummon },
                #endregion

                #region CATEGORY 7: Sell Prices
                { ItemID.EncumberingStone, Do(Worthless) },
                { ItemID.FlareGun, Do(Value(Item.sellPrice(silver: 10))) },
                { ItemID.GlowingMushroom, Do(Worthless) },
                { ItemID.Mushroom, Do(Worthless) },
                { ItemID.PortableStool, Do(Value(Item.sellPrice(copper: 20))) }, // Step Stool
                { ItemID.UncumberingStone, Do(Worthless) },
                { ItemID.ViciousMushroom, Do(Worthless) },
                { ItemID.VileMushroom, Do(Worthless) },
                #endregion
            };
        }

        internal static void UnloadTweaks()
        {
            currentTweaks?.Clear();
            currentTweaks = null;
        }
        #endregion

        #region SetDefaults (Item Tweaks Applied Here)
        internal static void SetDefaults_ApplyTweaks(Item item)
        {
            // Do nothing if the tweaks database is not defined.
            if (currentTweaks is null)
                return;

            // Grab the tweaking or balancing to apply, if any. If nothing comes back, do nothing.
            bool needsTweaking = currentTweaks.TryGetValue(item.type, out IItemTweak[] tweaks);
            if (!needsTweaking)
                return;

            // Apply all alterations sequentially, assuming they are relevant.
            foreach (IItemTweak tweak in tweaks)
                if (tweak.AppliesTo(item))
                    tweak.ApplyTweak(item);
        }
        #endregion

        #region Internal Structures

        // This function simply concatenates a bunch of Item Tweaks into an array.
        // It looks a lot nicer than constantly typing "new IItemTweak[]".
        internal static IItemTweak[] Do(params IItemTweak[] r) => r;

        #region Applicability Lambdas
        internal static bool DealsDamage(Item it) => it.damage > 0;
        internal static bool HasDefense(Item it) => it.defense > 0;
        internal static bool HasKnockback(Item it) => !it.accessory & !it.vanity; // how to check if something is wearable armor?
        internal static bool IsAxe(Item it) => it.axe > 0;
        internal static bool IsHammer(Item it) => it.hammer > 0;
        internal static bool IsMelee(Item it) => it.CountsAsClass<MeleeDamageClass>() || it.CountsAsClass<MeleeNoSpeedDamageClass>(); // true melee is included by extension
        internal static bool IsPickaxe(Item it) => it.pick > 0;
        internal static bool IsScalable(Item it) => it.damage > 0 && IsMelee(it); // sanity check: only melee weapons get scaled
        internal static bool IsUsable(Item it) => it.useStyle != ItemUseStyleID.None && it.useTime > 0 && it.useAnimation > 0;
        internal static bool UsesMana(Item it) => IsUsable(it); // Only usable items cost mana, but items must be able to have their mana cost disabled or enabled at will.
        internal static bool UtilizesVelocity(Item it) => IsUsable(it) || it.ammo > AmmoID.None; // The item must either be usable or be an ammunition for its velocity stat to do anything.
        #endregion

        #region Item Tweak Definitions
        internal interface IItemTweak
        {
            bool AppliesTo(Item it);
            void ApplyTweak(Item it);
        }

        #region Attack Speed Ratio
        private static float CapAttackSpeed(float f) => MathHelper.Clamp(f, BalancingConstants.MinimumAllowedAttackSpeed, BalancingConstants.MaximumAllowedAttackSpeed);

        internal class AttackSpeedExactRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public AttackSpeedExactRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it) => ItemID.Sets.BonusAttackSpeedMultiplier[it.type] = CapAttackSpeed(ratio);
        }
        internal static IItemTweak AttackSpeedExact(float f) => new AttackSpeedExactRule(f);

        internal class AttackSpeedRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public AttackSpeedRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                float currentAttackSpeedRatio = ItemID.Sets.BonusAttackSpeedMultiplier[it.type];
                ItemID.Sets.BonusAttackSpeedMultiplier[it.type] = CapAttackSpeed(ratio * currentAttackSpeedRatio);
            }
        }
        internal static IItemTweak AttackSpeedRatio(float f) => new AttackSpeedRatioRule(f);
        #endregion

        #region Axe Power
        // Uses the values shown by Terraria, which are multiplied by 5, not the internal values
        internal class AxePowerRule : IItemTweak
        {
            internal readonly int newAxePower = 0;

            public AxePowerRule(int newDisplayedAxePower) => newAxePower = newDisplayedAxePower / 5;
            public bool AppliesTo(Item it) => IsAxe(it);
            public void ApplyTweak(Item it)
            {
                it.axe = newAxePower;
                if (it.axe < 0)
                    it.axe = 0;
            }
        }
        internal static IItemTweak AxePower(int a) => new AxePowerRule(a);
        #endregion

        #region Consumable
        internal class ConsumableRule : IItemTweak
        {
            internal readonly bool flag = false;

            public ConsumableRule(bool c) => flag = c;
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it) => it.consumable = flag;
        }
        internal static IItemTweak Consumable => new ConsumableRule(true);
        internal static IItemTweak NotConsumable => new ConsumableRule(false);
        #endregion

        #region Crit Chance
        internal class CritChanceDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public CritChanceDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => DealsDamage(it);
            public void ApplyTweak(Item it)
            {
                it.crit += delta;
                if (it.crit < 0)
                    it.crit = 0;
            }
        }
        internal static IItemTweak CritDelta(int d) => new CritChanceDeltaRule(d);

        internal class CritChanceExactRule : IItemTweak
        {
            internal readonly int newCrit = 0;

            public CritChanceExactRule(int crit) => newCrit = crit;
            public bool AppliesTo(Item it) => DealsDamage(it);
            public void ApplyTweak(Item it)
            {
                it.crit = newCrit;
                if (it.crit < 0)
                    it.crit = 0;
            }
        }
        internal static IItemTweak CritExact(int crit) => new CritChanceExactRule(crit);
        #endregion

        #region Damage
        internal class DamageDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public DamageDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => DealsDamage(it);
            public void ApplyTweak(Item it)
            {
                it.damage += delta;
                if (it.damage < 0)
                    it.damage = 0;
            }
        }
        internal static IItemTweak DamageDelta(int d) => new DamageDeltaRule(d);

        internal class DamageExactRule : IItemTweak
        {
            internal readonly int newDamage = 0;

            public DamageExactRule(int dmg) => newDamage = dmg;
            public bool AppliesTo(Item it) => DealsDamage(it);
            public void ApplyTweak(Item it)
            {
                it.damage = newDamage;
                if (it.damage < 0)
                    it.damage = 0;
            }
        }
        internal static IItemTweak DamageExact(int d) => new DamageExactRule(d);

        internal class DamageRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public DamageRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => DealsDamage(it);
            public void ApplyTweak(Item it)
            {
                it.damage = (int)(it.damage * ratio);
                if (it.damage < 0)
                    it.damage = 0;
            }
        }
        internal static IItemTweak DamageRatio(float f) => new DamageRatioRule(f);
        #endregion

        #region Defense
        internal class DefenseDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public DefenseDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => HasDefense(it);
            public void ApplyTweak(Item it)
            {
                it.defense += delta;
                if (it.defense < 0)
                    it.defense = 0;
            }
        }
        internal static IItemTweak DefenseDelta(int d) => new DefenseDeltaRule(d);

        internal class DefenseExactRule : IItemTweak
        {
            internal readonly int newDefense = 0;

            public DefenseExactRule(int def) => newDefense = def;
            public bool AppliesTo(Item it) => HasDefense(it) || it.accessory;
            public void ApplyTweak(Item it)
            {
                it.defense = newDefense;
                if (it.defense < 0)
                    it.defense = 0;
            }
        }
        internal static IItemTweak DefenseExact(int d) => new DefenseExactRule(d);
        #endregion

        #region Hammer Power
        internal class HammerPowerRule : IItemTweak
        {
            internal readonly int newHammerPower = 0;

            public HammerPowerRule(int h) => newHammerPower = h;
            public bool AppliesTo(Item it) => IsHammer(it);
            public void ApplyTweak(Item it)
            {
                it.hammer = newHammerPower;
                if (it.hammer < 0)
                    it.hammer = 0;
            }
        }
        internal static IItemTweak HammerPower(int h) => new HammerPowerRule(h);
        #endregion

        #region Knockback
        internal class KnockbackDeltaRule : IItemTweak
        {
            internal readonly float delta = 0;

            public KnockbackDeltaRule(float d) => delta = d;
            public bool AppliesTo(Item it) => HasKnockback(it);
            public void ApplyTweak(Item it)
            {
                it.knockBack += delta;
                if (it.knockBack < 0f)
                    it.knockBack = 0f;
            }
        }
        internal static IItemTweak KnockbackDelta(float d) => new KnockbackDeltaRule(d);

        internal class KnockbackExactRule : IItemTweak
        {
            internal readonly float newKnockback = 0;

            public KnockbackExactRule(float kb) => newKnockback = kb;
            public bool AppliesTo(Item it) => HasKnockback(it);
            public void ApplyTweak(Item it)
            {
                it.knockBack = newKnockback;
                if (it.knockBack < 0f)
                    it.knockBack = 0f;
            }
        }
        internal static IItemTweak KnockbackExact(float kb) => new KnockbackExactRule(kb);

        internal class KnockbackRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public KnockbackRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => HasKnockback(it);
            public void ApplyTweak(Item it)
            {
                it.knockBack *= ratio;
                if (it.knockBack < 0f)
                    it.knockBack = 0f;
            }
        }
        internal static IItemTweak KnockbackRatio(float r) => new KnockbackRatioRule(r);
        #endregion

        #region Mana Cost
        internal class ManaDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public ManaDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => UsesMana(it);
            public void ApplyTweak(Item it)
            {
                it.mana += delta;
                if (it.mana < 0)
                    it.mana = 0;
            }
        }
        internal static IItemTweak ManaDelta(int d) => new ManaDeltaRule(d);

        internal class ManaExactRule : IItemTweak
        {
            internal readonly int newMana = 0;

            public ManaExactRule(int m) => newMana = m;
            public bool AppliesTo(Item it) => UsesMana(it);
            public void ApplyTweak(Item it)
            {
                it.mana = newMana;
                if (it.mana < 0)
                    it.mana = 0;
            }
        }
        internal static IItemTweak ManaExact(int m) => new ManaExactRule(m);

        internal class ManaRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public ManaRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => UsesMana(it);
            public void ApplyTweak(Item it)
            {
                it.mana = (int)(it.mana * ratio);
                if (it.mana < 0)
                    it.mana = 0;
            }
        }
        internal static IItemTweak ManaRatio(float f) => new ManaRatioRule(f);
        #endregion

        #region Max Stack
        internal class MaxStackRule : IItemTweak // max stack plus - calamity style
        {
            internal readonly int newMaxStack = 9999;

            public MaxStackRule(int stk) => newMaxStack = stk;
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it)
            {
                it.maxStack = newMaxStack;
                if (it.maxStack < 1)
                    it.maxStack = 1;
            }
        }
        internal static IItemTweak MaxStack(int stk) => new MaxStackRule(stk);
        #endregion

        #region Melee Settings (True Melee & Melee Speed)
        internal class MeleeSettingsRule : IItemTweak
        {
            // If true: Uses melee speed, which WILL apply to projectile fire rate.
            // If false: Does not use melee speed in any way.
            internal readonly bool speed = true;

            // If true: Counts as true melee, and benefits from True Melee specific bonuses.
            // If false: Does not count as true melee.
            internal readonly bool trueMelee = false;

            public MeleeSettingsRule(bool s, bool t = false)
            {
                speed = s;
                trueMelee = t;
            }
            public bool AppliesTo(Item it) => IsMelee(it);
            public void ApplyTweak(Item it)
            {
                // If set to use melee speed, the item's projectile fire rate now scales with melee speed.
                if (speed)
                    it.attackSpeedOnlyAffectsWeaponAnimation = false;

                // Set damage type appropriately.
                if (speed)
                    it.DamageType = trueMelee ? TrueMeleeDamageClass.Instance : DamageClass.Melee;
                else
                    it.DamageType = trueMelee ? TrueMeleeNoSpeedDamageClass.Instance : DamageClass.MeleeNoSpeed;
            }
        }
        internal static IItemTweak UseMeleeSpeed => new MeleeSettingsRule(true, false);
        internal static IItemTweak DontUseMeleeSpeed => new MeleeSettingsRule(false, false);
        internal static IItemTweak TrueMelee => new MeleeSettingsRule(true, true);
        internal static IItemTweak TrueMeleeNoSpeed => new MeleeSettingsRule(false, true);
        #endregion

        #region Pick Power
        internal class PickPowerRule : IItemTweak
        {
            internal readonly int newPickPower = 0;

            public PickPowerRule(int p) => newPickPower = p;
            public bool AppliesTo(Item it) => IsPickaxe(it);
            public void ApplyTweak(Item it)
            {
                it.pick = newPickPower;
                if (it.pick < 0)
                    it.pick = 0;
            }
        }
        internal static IItemTweak PickPower(int p) => new PickPowerRule(p);
        #endregion

        #region Scale (True Melee)
        internal class ScaleDeltaRule : IItemTweak
        {
            internal readonly float delta = 0;

            public ScaleDeltaRule(float d) => delta = d;
            public bool AppliesTo(Item it) => IsScalable(it);
            public void ApplyTweak(Item it)
            {
                if (DisableScalingForOverhaul)
                    return;
                it.scale += delta;
                if (it.scale < 0f)
                    it.scale = 0f;
            }
        }
        internal static IItemTweak ScaleDelta(float d) => new ScaleDeltaRule(d);

        internal class ScaleExactRule : IItemTweak
        {
            internal readonly float newScale = 0;

            public ScaleExactRule(float s) => newScale = s;
            public bool AppliesTo(Item it) => IsScalable(it);
            public void ApplyTweak(Item it)
            {
                if (DisableScalingForOverhaul)
                    return;
                it.scale = newScale;
                if (it.scale < 0f)
                    it.scale = 0f;
            }
        }
        internal static IItemTweak ScaleExact(float s) => new ScaleExactRule(s);

        internal class ScaleRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public ScaleRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsScalable(it);
            public void ApplyTweak(Item it)
            {
                if (DisableScalingForOverhaul)
                    return;
                it.scale *= ratio;
                if (it.scale < 0f)
                    it.scale = 0f;
            }
        }
        internal static IItemTweak ScaleRatio(float f) => new ScaleRatioRule(f);
        #endregion

        #region Shoot Speed (Velocity)
        internal class ShootSpeedDeltaRule : IItemTweak
        {
            internal readonly float delta = 0;

            public ShootSpeedDeltaRule(float d) => delta = d;
            public bool AppliesTo(Item it) => UtilizesVelocity(it);
            public void ApplyTweak(Item it)
            {
                it.shootSpeed += delta;
                if (it.shootSpeed < 0f)
                    it.shootSpeed = 0f;
            }
        }
        internal static IItemTweak ShootSpeedDelta(float d) => new ShootSpeedDeltaRule(d);

        internal class ShootSpeedExactRule : IItemTweak
        {
            internal readonly float newShootSpeed = 0;

            public ShootSpeedExactRule(float ss) => newShootSpeed = ss;
            public bool AppliesTo(Item it) => UtilizesVelocity(it);
            public void ApplyTweak(Item it)
            {
                it.shootSpeed = newShootSpeed;
                if (it.shootSpeed < 0f)
                    it.shootSpeed = 0f;
            }
        }
        internal static IItemTweak ShootSpeedExact(float s) => new ShootSpeedExactRule(s);

        internal class ShootSpeedRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public ShootSpeedRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => UtilizesVelocity(it);
            public void ApplyTweak(Item it)
            {
                it.shootSpeed *= ratio;
                if (it.shootSpeed < 0f)
                    it.shootSpeed = 0f;
            }
        }
        internal static IItemTweak ShootSpeedRatio(float f) => new ShootSpeedRatioRule(f);
        #endregion

        #region Tile Boost (Extra Tool Range)
        internal class TileBoostDeltaRule : IItemTweak
        {
            private readonly int delta = 0;

            public TileBoostDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it) => it.tileBoost += delta;
        }
        internal static IItemTweak TileBoostDelta(int d) => new TileBoostDeltaRule(d);

        internal class TileBoostExactRule : IItemTweak
        {
            private readonly int newTileBoost = 0;

            public TileBoostExactRule(int tb) => newTileBoost = tb;
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it) => it.tileBoost = newTileBoost;
        }
        internal static IItemTweak TileBoostExact(int tb) => new TileBoostExactRule(tb);
        #endregion

        #region Use Time and Use Animation
        internal class UseDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public UseDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation += delta;
                it.useTime += delta;
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseDelta(int d) => new UseDeltaRule(d);

        internal class UseExactRule : IItemTweak
        {
            internal readonly int newUseTime = 0;

            public UseExactRule(int ut) => newUseTime = ut;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation = newUseTime;
                it.useTime = newUseTime;
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseExact(int ut) => new UseExactRule(ut);

        internal class UseRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public UseRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation = (int)(it.useAnimation * ratio);
                it.useTime = (int)(it.useTime * ratio);
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseRatio(float f) => new UseRatioRule(f);

        internal class UseAnimationDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public UseAnimationDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation += delta;
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
            }
        }
        internal static IItemTweak UseAnimationDelta(int d) => new UseAnimationDeltaRule(d);

        internal class UseAnimationExactRule : IItemTweak
        {
            internal readonly int newUseAnimation = 0;

            public UseAnimationExactRule(int ua) => newUseAnimation = ua;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation = newUseAnimation;
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
            }
        }
        internal static IItemTweak UseAnimationExact(int ua) => new UseAnimationExactRule(ua);

        internal class UseAnimationRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public UseAnimationRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useAnimation = (int)(it.useAnimation * ratio);
                if (it.useAnimation < 1)
                    it.useAnimation = 1;
            }
        }
        internal static IItemTweak UseAnimationRatio(float f) => new UseAnimationRatioRule(f);

        internal class UseTimeDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public UseTimeDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useTime += delta;
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseTimeDelta(int d) => new UseTimeDeltaRule(d);

        internal class UseTimeExactRule : IItemTweak
        {
            internal readonly int newUseTime = 0;

            public UseTimeExactRule(int ut) => newUseTime = ut;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useTime = newUseTime;
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseTimeExact(int ut) => new UseTimeExactRule(ut);

        internal class UseTimeRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public UseTimeRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.useTime = (int)(it.useTime * ratio);
                if (it.useTime < 1)
                    it.useTime = 1;
            }
        }
        internal static IItemTweak UseTimeRatio(float f) => new UseTimeRatioRule(f);

        internal class ReuseDelayDeltaRule : IItemTweak
        {
            internal readonly int delta = 0;

            public ReuseDelayDeltaRule(int d) => delta = d;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.reuseDelay += delta;
                if (it.reuseDelay < 0)
                    it.reuseDelay = 0;
            }
        }
        internal static IItemTweak ReuseDelayDelta(int d) => new ReuseDelayDeltaRule(d);

        internal class ReuseDelayExactRule : IItemTweak
        {
            internal readonly int newReuseDelay = 0;

            public ReuseDelayExactRule(int rd) => newReuseDelay = rd;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.reuseDelay = newReuseDelay;
                if (it.reuseDelay < 0)
                    it.reuseDelay = 0;
            }
        }
        internal static IItemTweak ReuseDelayExact(int rd) => new ReuseDelayExactRule(rd);

        internal class ReuseDelayRatioRule : IItemTweak
        {
            internal readonly float ratio = 1f;

            public ReuseDelayRatioRule(float f) => ratio = f;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it)
            {
                it.reuseDelay = (int)(it.reuseDelay * ratio);
                if (it.reuseDelay < 0)
                    it.reuseDelay = 0;
            }
        }
        internal static IItemTweak ReuseDelayRatio(float f) => new ReuseDelayRatioRule(f);
        #endregion

        #region Use Turn
        internal class UseTurnRule : IItemTweak
        {
            internal readonly bool flag = true;

            public UseTurnRule(bool ut) => flag = ut;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it) => it.useTurn = flag;
        }
        internal static IItemTweak UseTurn => new UseTurnRule(true);
        internal static IItemTweak NoUseTurn => new UseTurnRule(false);
        #endregion

        #region Value (Sell Price)
        internal class ValueRule : IItemTweak
        {
            internal readonly int newValue = 0;

            public ValueRule(int v) => newValue = v;
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it)
            {
                it.value = newValue;
                if (it.value < 0)
                    it.value = 0;
            }
        }
        internal static IItemTweak Value(int v) => new ValueRule(v);
        internal static IItemTweak Worthless => new ValueRule(0);
        #endregion
        #endregion
        #endregion

        #region Shimmer Transmutations
        private void SetStaticDefaults_ShimmerRecipes()
        {
            var shimmerTransmute = ItemID.Sets.ShimmerTransformToItem;

            // Note: Making Luminite Ore -> Astral Ore makes Deus almost completely skippable with no (recipe-related) downsides.
            // Adding Cryonic Ore to the shimmer chain has the same issue.
            shimmerTransmute[ModContent.ItemType<AuricOre>()] = ModContent.ItemType<UelibloomOre>();
            shimmerTransmute[ModContent.ItemType<UelibloomOre>()] = ModContent.ItemType<ExodiumCluster>();
            shimmerTransmute[ModContent.ItemType<ExodiumCluster>()] = ItemID.LunarOre;
            shimmerTransmute[ModContent.ItemType<AstralOre>()] = ModContent.ItemType<ScoriaOre>();
            shimmerTransmute[ModContent.ItemType<ScoriaOre>()] = ModContent.ItemType<PerennialOre>();
            shimmerTransmute[ModContent.ItemType<PerennialOre>()] = shimmerTransmute[ItemID.LunarOre];
            shimmerTransmute[ModContent.ItemType<HallowedOre>()] = shimmerTransmute[ItemID.ChlorophyteOre];
            shimmerTransmute[ModContent.ItemType<AerialiteOre>()] = shimmerTransmute[ItemID.CobaltOre];

            //shimmerTransmute[ItemID.LunarOre] = ModContent.ItemType<AstralOre>();
            shimmerTransmute[ItemID.LunarOre] = ModContent.ItemType<ScoriaOre>();
            shimmerTransmute[ItemID.ChlorophyteOre] = ModContent.ItemType<HallowedOre>();
            shimmerTransmute[ItemID.CobaltOre] = ModContent.ItemType<AerialiteOre>();

            // Note: Not a part of the "main" ore shimmer chain
            shimmerTransmute[ModContent.ItemType<InfernalSuevite>()] = ItemID.Hellstone;

            //Fuck vanilla's stupid Giant Shelly, Crawdad, and Salamander exclusivity 
            shimmerTransmute[ModContent.ItemType<CrawCarapace>()] = ModContent.ItemType<GiantShell>();
            shimmerTransmute[ModContent.ItemType<GiantShell>()] = ModContent.ItemType<CrawCarapace>();

            //Jelly swap'n
            shimmerTransmute[ModContent.ItemType<LifeJelly>()] = ModContent.ItemType<CleansingJelly>();
            shimmerTransmute[ModContent.ItemType<CleansingJelly>()] = ModContent.ItemType<VitalJelly>();
            shimmerTransmute[ModContent.ItemType<VitalJelly>()] = ModContent.ItemType<LifeJelly>();

            //Astral Fishing Swap
            shimmerTransmute[ModContent.ItemType<PolarisParrotfish>()] = ModContent.ItemType<GacruxianMollusk>();
            shimmerTransmute[ModContent.ItemType<GacruxianMollusk>()] = ModContent.ItemType<UrsaSergeant>();
            shimmerTransmute[ModContent.ItemType<UrsaSergeant>()] = ModContent.ItemType<PolarisParrotfish>();
        }
        #endregion
    }
}
