using System.Collections.Generic;
using CalamityMod.Balancing;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Ores;
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
        private static bool DisableScalingForOverhaul => CalamityMod.Instance.overhaul is not null;

        #region Database and Initialization
        internal static SortedDictionary<int, IItemTweak[]> currentTweaks = null;

        internal static void LoadTweaks()
        {
            // Various shorthands for items which receive very simple changes, such as setting one flag.
            IItemTweak[] trueMelee = Do(TrueMelee);
            IItemTweak[] trueMeleeNoSpeed = Do(TrueMeleeNoSpeed);
            IItemTweak[] pointBlank = Do(PointBlank);
            IItemTweak[] nonConsumableBossSummon = Do(MaxStack(1), NotConsumable, UseTimeExact(10));

            // SORTING NOTES:
            // 1. Sort tweaks by categories first, then sort by the internal name in alphabetical order. Navigate through categories and names using the search function.
            // 2. Higher categories hold priority over lower ones (ie. Balancing with price tweaks belong in balancing, rather than price)
            // 3. Items with different display names as opposed to internal ones should have comments for display names for ease of access.
            currentTweaks = new SortedDictionary<int, IItemTweak[]>
            {
                #region CATEGORY 1: Weapon Balancing
                { ItemID.AdamantiteGlaive, Do(TrueMelee, UseRatio(0.8f), DamageExact(65), ShootSpeedRatio(1.25f)) },
                { ItemID.AdamantiteRepeater, Do(PointBlank, UseExact(14)) },
                { ItemID.AdamantiteSword, Do(UseTurn, DamageExact(77)) },
                { ItemID.AmberStaff, Do(UseTimeExact(15), UseAnimationExact(45), ReuseDelayExact(15)) },
                { ItemID.AmethystStaff, Do(ManaExact(2)) },
                { ItemID.Anchor, Do(DamageExact(107), UseExact(30)) },
                { ItemID.AntlionClaw, Do(UseExact(14)) }, // Mandible Blade
                { ItemID.Bananarang, Do(DamageExact(76), UseExact(14)) },
                { ItemID.BatScepter, Do(DamageExact(56)) },
                { ItemID.BeamSword, Do(UseMeleeSpeed, DamageExact(142), UseAnimationExact(45), ShootSpeedExact(23f), KnockbackExact(8)) },
                { ItemID.BeeGun, Do(DamageExact(13), ManaExact(4)) },
                { ItemID.BeeKeeper, Do((UseTurn),DamageExact(32)) },
                { ItemID.BeesKnees, Do(PointBlank, DamageExact(24), UseExact(38)) },
                { ItemID.Bladetongue, Do(UseTurn, UseRatio(0.8f), DamageExact(120)) },
                { ItemID.BlizzardStaff, Do(DamageExact(41), ManaExact(7)) },
                { ItemID.BloodyMachete, Do(DamageExact(24)) },
                { ItemID.Blowgun, Do(PointBlank, DamageExact(40)) },
                { ItemID.BluePhaseblade, Do(UseTurn, DamageExact(32)) },
                { ItemID.BluePhasesaber, Do(DamageExact(60)) },
                { ItemID.BookofSkulls, Do(ManaExact(12), ShootSpeedExact(5.5f)) },
                { ItemID.BookStaff, Do(ManaExact(14)) }, // Tome of Infinite Wisdom
                { ItemID.Boomstick, Do(PointBlank, DamageExact(11)) },
                { ItemID.BreakerBlade, Do(UseTurn, DamageExact(97)) },
                { ItemID.CandyCornRifle, Do(PointBlank, DamageExact(66)) },
                { ItemID.Cascade, Do(DamageExact(30)) },
                { ItemID.ChainGuillotines, Do(DamageExact(100)) },
                { ItemID.ChainGun, Do(PointBlank, DamageExact(35)) },
                { ItemID.ChainKnife, Do(DamageRatio(1.34f)) },  // Uses ratios due to remix seed
                { ItemID.ChlorophyteClaymore, Do(UseMeleeSpeed, DamageExact(176), UseExact(35), UseAnimationExact(45), ShootSpeedExact(22f)) },
                { ItemID.ChlorophytePartisan, Do(UseMeleeSpeed, UseRatio(0.8f), DamageExact(70)) },
                { ItemID.ChlorophyteSaber, Do(UseMeleeSpeed, DamageExact(80), UseExact(10)) },
                { ItemID.ChristmasTreeSword, Do(UseTurn, UseMeleeSpeed, DamageExact(80), UseExact(30)) },
                { ItemID.ClingerStaff, Do(DamageExact(63)) },
                { ItemID.ClockworkAssaultRifle, Do(PointBlank, DamageExact(21)) },
                { ItemID.CobaltNaginata, Do(TrueMelee, UseRatio(0.8f), DamageExact(90)) },
                { ItemID.CobaltRepeater, Do(PointBlank, UseExact(18)) },
                { ItemID.CobaltSword, Do(DamageExact(80)) },
                { ItemID.Code2, Do(DamageExact(43)) },
                { ItemID.CorruptYoyo, Do(DamageExact(20)) }, // Malaise
                { ItemID.CrimsonYoyo, Do(DamageExact(20)) }, // Artery
                { ItemID.CrystalBullet, Do(DamageExact(8)) },
                { ItemID.CrystalDart, Do(DamageExact(20)) },
                { ItemID.CrystalSerpent, Do(DamageExact(45)) },
                { ItemID.CrystalStorm, Do(DamageExact(40)) },
                { ItemID.CursedArrow, Do(DamageExact(14)) },
                { ItemID.CursedDart, Do(DamageExact(25)) },
                { ItemID.Cutlass, Do(UseRatio(0.9f), DamageExact(90)) },
                { ItemID.DaedalusStormbow, Do(DamageExact(30)) },
                { ItemID.DaoofPow, Do(DamageExact(160)) },
                { ItemID.DarkLance, Do(TrueMelee, DamageExact(45)) },
                { ItemID.DartRifle, Do(PointBlank, DamageExact(58)) },
                { ItemID.DayBreak, Do(DamageExact(125), UseExact(20)) },
                { ItemID.DD2BetsyBow, Do(DamageExact(42)) }, // Aerial Bane's ridiculous multiplier is removed, so this compensates for that
                { ItemID.DD2SquireBetsySword, Do(UseMeleeSpeed, DamageExact(150)) }, // Flying Dragon
                { ItemID.DD2SquireDemonSword, Do(DamageExact(110), UseExact(25)) }, // Brand of the Inferno
                { ItemID.DeathSickle, Do(UseMeleeSpeed, DamageExact(82), ShootSpeedExact(15f)) },
                { ItemID.DemonBow, Do(PointBlank, DamageExact(12)) },
                { ItemID.DemonScythe, Do(DamageExact(33)) },
                { ItemID.DyeTradersScimitar, Do(UseTurn, DamageExact(24)) }, // Exotic Scimitar
                { ItemID.ElectrosphereLauncher, Do(DamageExact(44)) },
                { ItemID.ElfMelter, Do(ShootSpeedDelta(+5f)) },
                { ItemID.EmeraldStaff, Do(DamageExact(27)) },
                { ItemID.EmpressBlade, Do(DamageExact(60), UseExact(20)) }, // Terraprisma
                { ItemID.EnchantedBoomerang, Do(DamageExact(24)) },
                { ItemID.EnchantedSword, Do(UseMeleeSpeed, DamageExact(30), ShootSpeedExact(15f)) },
                { ItemID.Excalibur, Do(TrueMelee, UseRatio(0.8f), DamageExact(125), UseAnimationExact(45)) },
                { ItemID.FairyQueenMagicItem, Do(DamageExact(54)) }, // Nightglow
                { ItemID.FalconBlade, Do(UseTurn, UseExact(15)) },
                // For now, I think I will balance Celebration like a serious weapon. -CIT
                { ItemID.FireworksLauncher, Do(DamageExact(50), UseExact(25)) }, // Celebration
                { ItemID.Flairon, Do(DamageExact(73)) },
                { ItemID.Flamarang, Do(DamageExact(40)) },
                { ItemID.Flamelash, Do(DamageExact(40)) },
                { ItemID.Flamethrower, Do(DamageExact(21), ShootSpeedDelta(+3f)) },
                { ItemID.FlowerofFire, Do(ManaExact(7), UseRatio(0.88f), DamageRatio(0.65f)) }, // Uses ratios due to remix seed
                { ItemID.FlowerofFrost, Do(ManaExact(7), UseExact(22), DamageExact(70), ShootSpeedExact(14)) },
                { ItemID.FlyingKnife, Do(DamageExact(53)) },
                { ItemID.Frostbrand, Do(UseMeleeSpeed, DamageExact(88)) },
                { ItemID.FrostStaff, Do(DamageExact(160), UseExact(37), ShootSpeedExact(20f)) }, // has 1 extra update
                { ItemID.Gatligator, Do(PointBlank, UseExact(6)) },
                { ItemID.GoldenShower, Do(DamageExact(39)) },
                { ItemID.GoldShortsword, Do(TrueMelee, DamageExact(17)) },
                { ItemID.GolemFist, Do(DamageExact(150)) },
                { ItemID.Gradient, Do(DamageExact(39)) },
                { ItemID.GreenPhaseblade, Do(UseTurn, DamageExact(32)) },
                { ItemID.GreenPhasesaber, Do(DamageExact(60)) },
                { ItemID.GrenadeLauncher, Do(DamageExact(112)) },
                { ItemID.Gungnir, Do(TrueMelee, UseRatio(0.8f), DamageExact(92), ShootSpeedRatio(1.25f)) },
                { ItemID.HallowedRepeater, Do(PointBlank, UseExact(12)) },
                { ItemID.Handgun, Do(PointBlank, UseExact(22), DamageExact(36)) },
                { ItemID.HellwingBow, Do(PointBlank, DamageExact(16)) },
                { ItemID.HighVelocityBullet, Do(DamageExact(13)) },
                { ItemID.HiveFive, Do(DamageExact(26)) },
                { ItemID.HornetStaff, Do(DamageExact(18), UseExact(30)) },
                { ItemID.IceBlade, Do(UseMeleeSpeed) },
                { ItemID.IceBoomerang, Do(UseExact(25), ShootSpeedExact(9)) },
                { ItemID.IceRod, Do(UseExact(6), DamageExact(30), ShootSpeedExact(20)) },
                { ItemID.IceSickle, Do(UseMeleeSpeed, DamageExact(75), ShootSpeedExact(20f)) },
                { ItemID.IchorArrow, Do(DamageExact(13)) },
                { ItemID.IchorBullet, Do(DamageExact(11)) },
                { ItemID.ImpStaff, Do(UseExact(30), DamageExact(25)) },
                { ItemID.InfernoFork, Do(DamageExact(99), ShootSpeedExact(11)) },
                { ItemID.InfluxWaver, Do(UseMeleeSpeed, DamageExact(80), UseExact(25)) },
                { ItemID.IronShortsword, Do(TrueMelee, DamageExact(10)) },
                { ItemID.Keybrand, Do(UseTurn) },
                { ItemID.KOCannon, Do(DamageRatio(2.65f)) }, // Uses ratios due to remix seed
                { ItemID.Kraken, Do(DamageExact(85)) },
                { ItemID.LaserMachinegun, Do(DamageExact(49)) },
                { ItemID.LaserRifle, Do(DamageExact(46), UseExact(10), ManaExact(4)) },
                { ItemID.LastPrism, Do(DamageExact(57), ManaExact(10)) },
                { ItemID.LeadShortsword, Do(TrueMelee, DamageExact(11)) },
                { ItemID.LightDisc, Do(DamageExact(128), ShootSpeedExact(18)) },
                { ItemID.LunarFlareBook, Do(DamageExact(120)) },
                { ItemID.MagicalHarp, Do(DamageExact(50), ShootSpeedExact(12f)) },
                { ItemID.MagicDagger, Do(DamageRatio(1.8f), UseRatio(1.88f), ShootSpeedExact(30)) }, // Uses ratios due to remix seed
                { ItemID.MagicMissile, Do(DamageExact(23), ManaExact(10), UseAnimationExact(20), UseTimeExact(10)) },
                { ItemID.MagnetSphere, Do(DamageExact(52)) },
                { ItemID.Marrow, Do(PointBlank, DamageExact(69)) },
                { ItemID.MedusaHead, Do(ManaExact(6), DamageExact(48)) },
                { ItemID.Meowmere, Do(UseMeleeSpeed, DamageExact(240)) },
                { ItemID.MeteorStaff, Do(DamageExact(58), ManaExact(7), ShootSpeedExact(13f)) },
                { ItemID.MiniNukeI, Do(DamageExact(90)) },
                { ItemID.MiniNukeII, Do(DamageExact(90)) },
                { ItemID.Minishark, Do(PointBlank, DamageExact(4)) },
                { ItemID.MoltenFury, Do(PointBlank, UseExact(29)) },
                { ItemID.MonkStaffT1, Do(TrueMeleeNoSpeed, DamageExact(83)) }, // Sleepy Octopod
                { ItemID.MonkStaffT2, Do(TrueMelee, DamageExact(90)) }, // Ghastly Glaive
                { ItemID.MonkStaffT3, Do(DamageExact(225)) }, // Sky Dragon's Fury
                { ItemID.MoonlordBullet, Do(DamageExact(19)) }, // Luminite Bullet
                { ItemID.MoonlordTurretStaff, Do(DamageExact(50), UseExact(15)) }, //Lunar Portal Staff
                { ItemID.Muramasa, Do(CritDelta(+20)) },
                { ItemID.MushroomSpear, Do(TrueMelee, UseRatio(0.8f), DamageExact(100)) },
                { ItemID.Musket, Do(PointBlank, DamageExact(22)) },
                { ItemID.MythrilHalberd, Do(TrueMelee, UseRatio(0.8f), DamageExact(95), ShootSpeedRatio(1.25f)) },
                { ItemID.MythrilRepeater, Do(PointBlank, UseExact(16)) },
                { ItemID.MythrilSword, Do(UseTurn, DamageExact(100)) },
                { ItemID.NettleBurst, Do(ManaExact(10), DamageExact(70)) },
                { ItemID.NightsEdge, Do(TrueMelee, DamageExact(45)) },
                { ItemID.NorthPole, Do(UseMeleeSpeed) },
                { ItemID.OrangePhaseblade, Do(UseTurn, DamageExact(32)) },
                { ItemID.OrangePhasesaber, Do(DamageExact(60)) },
                { ItemID.OrichalcumHalberd, Do(TrueMelee, UseRatio(0.8f), DamageExact(98), ShootSpeedRatio(1.25f)) },
                { ItemID.OrichalcumRepeater, Do(PointBlank, DamageExact(48)) },
                { ItemID.OrichalcumSword, Do(UseTurn, DamageExact(82)) },
                { ItemID.PainterPaintballGun, Do(PointBlank, DamageExact(8)) },
                { ItemID.PaladinsHammer, Do(DamageExact(100), ShootSpeedExact(23)) },
                { ItemID.PalladiumPike, Do(TrueMelee, UseRatio(0.8f), DamageExact(96)) },
                { ItemID.PalladiumRepeater, Do(PointBlank, DamageExact(45)) },
                { ItemID.PalladiumSword, Do(DamageExact(100)) },
                { ItemID.PearlwoodBow, Do(PointBlank, DamageExact(20), UseDelta(+8), ShootSpeedDelta(+3.4f), KnockbackDelta(+1f)) },
                { ItemID.PearlwoodSword, Do(UseTurn, DamageExact(45)) },
                { ItemID.PewMaticHorn, Do(DamageExact(25), ShootSpeedExact(15)) },
                { ItemID.Phantasm, Do(PointBlank, DamageExact(48)) },
                { ItemID.PhoenixBlaster, Do(PointBlank, UseExact(18)) },
                { ItemID.PiranhaGun, Do(DamageExact(48)) },
                { ItemID.PlatinumBow, Do(PointBlank, DamageExact(12)) },
                { ItemID.PlatinumShortsword, Do(TrueMelee, DamageExact(18)) },
                { ItemID.PoisonStaff, Do(DamageExact(57)) },
                { ItemID.PossessedHatchet, Do(DamageExact(135)) },
                { ItemID.PsychoKnife, Do(UseTurn, UseExact(11), DamageExact(255)) },
                { ItemID.PurpleClubberfish, Do(UseTurn, KnockbackExact(10f)) },
                { ItemID.PurplePhaseblade, Do(UseTurn, DamageExact(32)) },
                { ItemID.PurplePhasesaber, Do(DamageExact(60)) },
                { ItemID.PygmyStaff, Do(UseExact(20), DamageExact(70)) },
                { ItemID.QuadBarrelShotgun, Do(PointBlank, DamageExact(11)) },
                { ItemID.RainbowRod, Do(DamageExact(35), ManaExact(15)) },
                { ItemID.Rally, Do(DamageExact(18)) },
                { ItemID.RainbowGun, Do(DamageExact(60), ManaExact(40)) },
                { ItemID.RavenStaff, Do(UseExact(20), DamageExact(36)) },
                { ItemID.RazorbladeTyphoon, Do(DamageExact(103)) },
                { ItemID.Razorpine, Do(DamageExact(40)) },
                { ItemID.RedPhaseblade, Do(UseTurn, DamageExact(32)) },
                { ItemID.RedPhasesaber, Do(DamageExact(60)) },
                { ItemID.RedRyder, Do(PointBlank, DamageExact(24)) },
                { ItemID.RedsYoyo, Do(DamageExact(48)) }, // Red's Throw and Valkyrie Yoyo have the same stats
                { ItemID.RocketLauncher, Do(DamageExact(70)) },
                { ItemID.Sandgun, Do(PointBlank, DamageExact(22), UseExact(20)) },
                { ItemID.SapphireStaff, Do(DamageExact(25)) },
                { ItemID.Seedler, Do(UseMeleeSpeed, DamageExact(74), ShootSpeedDelta(+4f)) },
                { ItemID.ShadowbeamStaff, Do(DamageExact(100)) },
                { ItemID.ShadowFlameBow, Do(PointBlank, DamageExact(55)) },
                { ItemID.ShadowFlameHexDoll, Do(DamageExact(40), ShootSpeedExact(30)) },
                { ItemID.ShadowFlameKnife, Do(DamageExact(70)) },
                { ItemID.SharpTears, Do(DamageExact(49)) }, // Blood Thorn
                { ItemID.Shotgun, Do(PointBlank, DamageExact(36)) },
                { ItemID.Shroomerang, Do(ShootSpeedExact(11)) },
                { ItemID.SilverBullet, Do(DamageExact(8)) },
                { ItemID.SilverShortsword, Do(TrueMelee, DamageExact(14)) },
                { ItemID.SkyFracture, Do(DamageExact(54), ShootSpeedExact(30f)) },
                { ItemID.SlapHand, Do(UseTurn, DamageExact(120)) },
                { ItemID.Smolstar, Do(DamageExact(9), UseExact(25)) }, // Blade Staff
                { ItemID.SniperRifle, Do(PointBlank, DamageExact(215)) },
                { ItemID.SolarEruption, Do(DamageExact(157)) },
                { ItemID.SoulDrain, Do(DamageExact(38)) }, // Life Drain
                { ItemID.SpaceGun, Do(DamageExact(25)) },
                { ItemID.Spear, Do(TrueMelee, DamageExact(14)) },
                { ItemID.SpectreStaff, Do(DamageExact(78)) },
                { ItemID.SpiritFlame, Do(UseExact(20), ManaExact(11), ShootSpeedExact(2f)) },
                { ItemID.StaffofEarth, Do(DamageExact(150)) },
                { ItemID.StarCannon, Do(DamageExact(25)) },
                { ItemID.StardustDragonStaff, Do(DamageExact(20), UseExact(19)) },
                { ItemID.StormTigerStaff, Do(DamageExact(49), UseExact(20)) }, // Desert Tiger Staff
                { ItemID.StylistKilLaKillScissorsIWish, Do(UseTurn, DamageExact(18)) }, // Stylish Scissors
                { ItemID.Stynger, Do(DamageExact(75)) },
                { ItemID.Swordfish, Do(TrueMelee, DamageExact(24)) },
                { ItemID.TacticalShotgun, Do(PointBlank, DamageExact(41)) },
                { ItemID.TaxCollectorsStickOfDoom, Do(UseTurn, UseRatio(0.8f), DamageExact(70)) }, // Classy Cane
                { ItemID.TendonBow, Do(PointBlank, DamageExact(17)) },
                { ItemID.TerraBlade, Do(DamageExact(122)) },
                // Vanilla damage 190. After fixing iframes so yoyo and shots can hit simultaneously,
                // Terrarian is extremely overpowered and requires a heavy nerf.
                { ItemID.Terrarian, Do(DamageExact(86)) },
                { ItemID.TheEyeOfCthulhu, Do(DamageExact(90)) },
                { ItemID.TheRottedFork, Do(TrueMelee, DamageExact(20)) },
                { ItemID.TheUndertaker, Do(PointBlank, DamageExact(15)) },
                { ItemID.ThunderSpear, Do(UseMeleeSpeed) }, // Storm Spear
                { ItemID.ThunderStaff, Do(DamageExact(18)) }, //Thunder Zapper
                { ItemID.TitaniumRepeater, Do(PointBlank, DamageExact(52)) },
                { ItemID.TitaniumSword, Do(UseTurn, DamageExact(77)) },
                { ItemID.TitaniumTrident, Do(TrueMelee, UseRatio(0.8f), DamageExact(72), ShootSpeedRatio(1.25f)) },
                { ItemID.TopazStaff, Do(ManaExact(2)) },
                { ItemID.Toxikarp, Do(UseTimeExact(7), UseAnimationExact(14)) },
                { ItemID.Trident, Do(TrueMelee, DamageExact(20)) },
                { ItemID.Trimarang, Do(DamageExact(24)) },
                { ItemID.TrueExcalibur, Do(TrueMelee, DamageExact(107)) },
                { ItemID.TrueNightsEdge, Do(DamageExact(105)) },
                { ItemID.Tsunami, Do(PointBlank, DamageExact(49)) },
                { ItemID.TungstenBullet, Do(DamageExact(8)) },
                { ItemID.TungstenShortsword, Do(TrueMelee, DamageExact(15)) },
                { ItemID.UnholyArrow, Do(DamageExact(11)) },
                { ItemID.UnholyTrident, Do(ManaRatio(0.78f), DamageRatio(0.91f)) },  // Uses ratios due to remix seed
                { ItemID.VampireKnives, Do(DamageExact(38)) },
                { ItemID.ValkyrieYoyo, Do(DamageExact(48)) }, // Red's Throw and Valkyrie Yoyo have the same stats
                { ItemID.VenomStaff, Do(DamageExact(55)) },
                { ItemID.VenusMagnum, Do(PointBlank, DamageExact(65)) },
                { ItemID.WaspGun, Do(UseExact(11), DamageExact(58)) },
                { ItemID.WaterBolt, Do(DamageExact(23)) },
                { ItemID.WhitePhaseblade, Do(UseTurn, DamageExact(32)) },
                { ItemID.WhitePhasesaber, Do(DamageExact(60)) },
                { ItemID.WoodenBoomerang, Do(DamageExact(16), Value(Item.sellPrice(copper: 20))) },
                { ItemID.Yelets, Do(DamageExact(53)) },
                { ItemID.YellowPhaseblade, Do(UseTurn, DamageExact(32)) },
                { ItemID.YellowPhasesaber, Do(DamageExact(60)) },
                { ItemID.Zenith, Do(DamageExact(210)) },
                { ItemID.ZombieArm, Do(UseTurn, KnockbackExact(12f)) },
                #endregion

                #region CATEGORY 2: Defense Balancing
                { ItemID.AncientHallowedGreaves, Do(DefenseDelta(+2)) },
                { ItemID.AncientHallowedPlateMail, Do(DefenseDelta(+3)) },
                { ItemID.AnkhShield, Do(DefenseDelta(+4)) }, // 8 total
                { ItemID.CobaltShield, Do(DefenseDelta(+3)) }, // 4 total
                { ItemID.EoCShield, Do(DefenseDelta(+1)) }, // Shield of Cthulhu
                { ItemID.FrozenShield, Do(DefenseDelta(+4)) }, // 10 total (plus the Frozen Turtle Shell DR effect)
                { ItemID.FrozenTurtleShell, Do(DefenseExact(6)) },
                { ItemID.HallowedGreaves, Do(DefenseDelta(+2)) },
                { ItemID.HallowedPlateMail, Do(DefenseDelta(+3)) },
                { ItemID.HeroShield, Do(DefenseDelta(+5)) }, // 15 total (plus increased max life)
                { ItemID.LavaSkull, Do(DefenseExact(4)) }, // Magma Skull
                { ItemID.MoltenSkullRose, Do(DefenseExact(8)) },
                { ItemID.ObsidianShield, Do(DefenseDelta(+4)) }, // 6 total
                { ItemID.ObsidianSkull, Do(DefenseDelta(+1)) }, // 2 total
                { ItemID.ObsidianSkullRose, Do(DefenseExact(4)) },
                { ItemID.OrichalcumBreastplate, Do(DefenseDelta(+3)) },
                { ItemID.OrichalcumHeadgear, Do(DefenseDelta(+2)) },
                { ItemID.OrichalcumHelmet, Do(DefenseDelta(+3)) },
                { ItemID.OrichalcumLeggings, Do(DefenseDelta(+4)) },
                { ItemID.OrichalcumMask, Do(DefenseDelta(+3)) },
                { ItemID.PaladinsShield, Do(DefenseDelta(+2)) }, // 8 total
                { ItemID.PalladiumBreastplate, Do(DefenseDelta(+3)) },
                { ItemID.PalladiumHeadgear, Do(DefenseDelta(+2)) },
                { ItemID.PalladiumHelmet, Do(DefenseDelta(+3)) },
                { ItemID.PalladiumMask, Do(DefenseDelta(+1)) },
                { ItemID.PalladiumLeggings, Do(DefenseDelta(+3)) },
                { ItemID.Shackle, Do(DefenseDelta(+2)) }, // 3 total
                #endregion

                #region CATEGORY 3: Tool Balancing
                { ItemID.AcornAxe, Do(AxePower(125)) }, // Axe of Regrowth
                { ItemID.AdamantiteChainsaw, Do(TrueMeleeNoSpeed, AxePower(90), UseTimeExact(4), TileBoostExact(+0), DamageExact(75)) },
                { ItemID.AdamantiteDrill, Do(TrueMeleeNoSpeed, PickPower(180), UseTimeExact(4), TileBoostExact(+1), DamageExact(32)) },
                { ItemID.AdamantitePickaxe, Do(PickPower(180), UseTimeExact(8), TileBoostExact(+1)) },
                { ItemID.AdamantiteWaraxe, Do(AxePower(160), UseTimeExact(10), TileBoostExact(+1)) },
                { ItemID.BloodLustCluster, Do(AxePower(100), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.BonePickaxe, Do(PickPower(55), UseTimeExact(6)) },
                { ItemID.BorealWoodHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.ButchersChainsaw, Do(TrueMeleeNoSpeed, AxePower(150), UseTimeExact(3), TileBoostExact(+0), DamageExact(177)) },
                { ItemID.CactusPickaxe, Do(PickPower(34), UseTimeExact(9)) },
                { ItemID.CnadyCanePickaxe, Do(PickPower(55), UseTimeExact(9), TileBoostExact(+1)) }, // Candy Cane Pickaxe
                { ItemID.ChlorophyteChainsaw, Do(TrueMeleeNoSpeed, AxePower(120), UseTimeExact(3), TileBoostExact(+0), DamageExact(112)) },
                { ItemID.ChlorophyteDrill, Do(TrueMeleeNoSpeed, PickPower(200), UseTimeExact(4), TileBoostExact(+2), DamageExact(43)) },
                { ItemID.ChlorophyteGreataxe, Do(AxePower(165), UseTimeExact(7), TileBoostExact(+2)) },
                { ItemID.ChlorophyteJackhammer, Do(TrueMeleeNoSpeed, HammerPower(90), UseTimeExact(5), TileBoostExact(+0)) },
                { ItemID.ChlorophytePickaxe, Do(PickPower(200), UseTimeExact(7), TileBoostExact(+2)) },
                { ItemID.ChlorophyteWarhammer, Do(HammerPower(90), UseTimeExact(8), TileBoostExact(+2)) },
                { ItemID.CobaltChainsaw, Do(TrueMeleeNoSpeed, AxePower(70), UseTimeExact(4), TileBoostExact(-1), DamageExact(51)) },
                { ItemID.CobaltDrill, Do(TrueMeleeNoSpeed, PickPower(130), UseTimeExact(5), DamageExact(21)) },
                { ItemID.CobaltPickaxe, Do(PickPower(130), UseTimeExact(9)) },
                { ItemID.CobaltWaraxe, Do(AxePower(125), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.CopperAxe, Do(AxePower(50), UseTimeExact(16), TileBoostExact(+0)) },
                { ItemID.CopperHammer, Do(HammerPower(35), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.CopperPickaxe, Do(PickPower(35), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.DeathbringerPickaxe, Do(PickPower(70), UseTimeExact(10)) },
                { ItemID.Drax, Do(TrueMeleeNoSpeed, PickPower(200), AxePower(110), UseTimeExact(4), TileBoostExact(+1), DamageExact(80)) },
                { ItemID.EbonwoodHammer, Do(HammerPower(25), UseTimeExact(9), TileBoostExact(+0)) },
                { ItemID.FleshGrinder, Do(HammerPower(70), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.GoldAxe, Do(AxePower(80), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.GoldHammer, Do(HammerPower(60), UseTimeExact(9), TileBoostExact(+0)) },
                { ItemID.GoldPickaxe, Do(PickPower(55), UseTimeExact(9)) },
                { ItemID.Hammush, Do(HammerPower(85), UseTimeExact(10), TileBoostExact(+1)) },
                { ItemID.IronAxe, Do(AxePower(60), UseTimeExact(15), TileBoostExact(+0)) },
                { ItemID.IronHammer, Do(HammerPower(45), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.IronPickaxe, Do(PickPower(40), UseTimeExact(8)) },
                { ItemID.LaserDrill, Do(PickPower(220), AxePower(120), UseTimeExact(4), DamageExact(54)) },
                { ItemID.LeadAxe, Do(AxePower(60), UseTimeExact(15), TileBoostExact(+0)) },
                { ItemID.LeadHammer, Do(HammerPower(45), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.LeadPickaxe, Do(PickPower(40), UseTimeExact(8)) },
                { ItemID.LucyTheAxe, Do(AxePower(150), UseExact(13), TileBoostExact(+1)) },
                { ItemID.LunarHamaxeNebula, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
                { ItemID.LunarHamaxeSolar, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
                { ItemID.LunarHamaxeStardust, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
                { ItemID.LunarHamaxeVortex, Do(HammerPower(100), AxePower(175), UseTimeExact(5), TileBoostExact(+4)) },
                { ItemID.MeteorHamaxe, Do(HammerPower(70), AxePower(100), UseTimeExact(16), TileBoostExact(+0)) },
                { ItemID.MoltenHamaxe, Do(HammerPower(75), AxePower(125), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.MoltenPickaxe, Do(PickPower(100), UseTimeExact(10)) },
                { ItemID.MythrilChainsaw, Do(TrueMeleeNoSpeed, AxePower(80), UseTimeExact(4), TileBoostExact(-1), DamageExact(63)) },
                { ItemID.MythrilDrill, Do(TrueMeleeNoSpeed, PickPower(160), UseTimeExact(4), DamageExact(26)) },
                { ItemID.MythrilPickaxe, Do(PickPower(160), UseTimeExact(8)) },
                { ItemID.MythrilWaraxe, Do(AxePower(140), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.NebulaDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4), DamageExact(95)) },
                { ItemID.NebulaPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.NightmarePickaxe, Do(PickPower(66), UseTimeExact(9)) },
                { ItemID.OrichalcumChainsaw, Do(TrueMeleeNoSpeed, AxePower(80), UseTimeExact(4), TileBoostExact(-1), DamageExact(63)) },
                { ItemID.OrichalcumDrill, Do(TrueMeleeNoSpeed, PickPower(160), UseTimeExact(4), DamageExact(26)) },
                { ItemID.OrichalcumPickaxe, Do(PickPower(160), UseTimeExact(8)) },
                { ItemID.OrichalcumWaraxe, Do(AxePower(140), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.PalladiumChainsaw, Do(TrueMeleeNoSpeed, AxePower(70), UseTimeExact(4), TileBoostExact(-1), DamageExact(51)) },
                { ItemID.PalladiumDrill, Do(TrueMeleeNoSpeed, PickPower(130), UseTimeExact(5), DamageExact(21)) },
                { ItemID.PalladiumPickaxe, Do(PickPower(130), UseTimeExact(9)) },
                { ItemID.PalladiumWaraxe, Do(AxePower(125), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.PalmWoodHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.PearlwoodHammer, Do(HammerPower(25), UseTimeExact(4), UseAnimationExact(20), DamageExact(36), TileBoostExact(+0)) },
                { ItemID.PickaxeAxe, Do(PickPower(200), AxePower(110), UseTimeExact(7), TileBoostExact(+1)) },
                { ItemID.Picksaw, Do(PickPower(210), AxePower(125), UseTimeExact(6), TileBoostExact(+1)) },
                { ItemID.PlatinumAxe, Do(AxePower(80), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.PlatinumHammer, Do(HammerPower(60), UseTimeExact(9), TileBoostExact(+0)) },
                { ItemID.PlatinumPickaxe, Do(PickPower(55), UseTimeExact(9)) },
                { ItemID.Pwnhammer, Do(HammerPower(80), UseTimeExact(11), TileBoostExact(+1)) },
                { ItemID.ReaverShark, Do(PickPower(100), UseTimeExact(16)) },
                { ItemID.RichMahoganyHammer, Do(HammerPower(25), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.Rockfish, Do(HammerPower(50), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.SawtoothShark, Do(TrueMeleeNoSpeed, AxePower(45), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.ShadewoodHammer, Do(HammerPower(25), UseTimeExact(9), TileBoostExact(+0)) },
                { ItemID.ShroomiteDiggingClaw, Do(PickPower(200), AxePower(125), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.SilverAxe, Do(AxePower(70), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.SilverHammer, Do(HammerPower(55), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.SilverPickaxe, Do(PickPower(50), UseTimeExact(11)) },
                { ItemID.SolarFlareDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4), DamageExact(95)) },
                { ItemID.SolarFlarePickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.SpectreHamaxe, Do(HammerPower(90), AxePower(170), TileBoostExact(+4)) },
                { ItemID.SpectrePickaxe, Do(PickPower(200), TileBoostExact(+4)) },
                { ItemID.StardustDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4), DamageExact(95)) },
                { ItemID.StardustPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.TheAxe, Do(HammerPower(100), AxePower(175), UseTimeExact(7), TileBoostExact(+1)) },
                { ItemID.TheBreaker, Do(HammerPower(70), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.TinAxe, Do(AxePower(50), UseTimeExact(16), TileBoostExact(+0)) },
                { ItemID.TinHammer, Do(HammerPower(35), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.TinPickaxe, Do(PickPower(35), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.TitaniumChainsaw, Do(TrueMeleeNoSpeed, AxePower(90), UseTimeExact(4), TileBoostExact(+0), DamageExact(75)) },
                { ItemID.TitaniumDrill, Do(TrueMeleeNoSpeed, PickPower(180), UseTimeExact(4), TileBoostExact(+1), DamageExact(32)) },
                { ItemID.TitaniumPickaxe, Do(PickPower(180), UseTimeExact(8), TileBoostExact(+1)) },
                { ItemID.TitaniumWaraxe, Do(AxePower(160), UseTimeExact(10), TileBoostExact(+1)) },
                { ItemID.TungstenAxe, Do(AxePower(70), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.TungstenHammer, Do(HammerPower(55), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.TungstenPickaxe, Do(PickPower(50), UseTimeExact(11)) },
                { ItemID.VortexDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4), DamageExact(95)) },
                { ItemID.VortexPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.WarAxeoftheNight, Do(AxePower(100), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.WoodenHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
                #endregion

                #region CATEGORY 4: True Melee support
                { ItemID.Arkhalis, trueMeleeNoSpeed },
                { ItemID.CopperShortsword, trueMelee },
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
                { ItemID.Terragrim, trueMeleeNoSpeed },
                { ItemID.TheHorsemansBlade, trueMelee },
                { ItemID.TinShortsword, trueMelee },
                { ItemID.VortexChainsaw, trueMeleeNoSpeed },
                #endregion

                #region CATEGORY 5: Point Blank support
                { ItemID.Blowpipe, pointBlank },
                { ItemID.BorealWoodBow, pointBlank },
                { ItemID.ChlorophyteShotbow, pointBlank },
                { ItemID.CopperBow, pointBlank },
                { ItemID.DartPistol, pointBlank },
                { ItemID.DD2PhoenixBow, pointBlank }, // Phantom Phoenix
                { ItemID.EbonwoodBow, pointBlank },
                { ItemID.FairyQueenRangedItem, pointBlank }, //Eventide
                { ItemID.FlintlockPistol, pointBlank },
                { ItemID.FlareGun,  Do(PointBlank, Value(Item.sellPrice(silver: 10))) },
                { ItemID.GoldBow, pointBlank },
                { ItemID.Harpoon, pointBlank },
                { ItemID.IceBow, pointBlank },
                { ItemID.IronBow, pointBlank },
                { ItemID.LeadBow, pointBlank },
                { ItemID.Megashark, pointBlank },
                { ItemID.OnyxBlaster, pointBlank },
                { ItemID.PalmWoodBow, pointBlank },
                { ItemID.PulseBow, pointBlank },
                { ItemID.Revolver, pointBlank },
                { ItemID.RichMahoganyBow, pointBlank },
                { ItemID.SDMG, pointBlank },
                { ItemID.ShadewoodBow, pointBlank },
                { ItemID.SilverBow, pointBlank },
                { ItemID.SnowballCannon, pointBlank },
                { ItemID.StakeLauncher, pointBlank },
                { ItemID.TinBow, pointBlank },
                { ItemID.TungstenBow, pointBlank },
                { ItemID.Uzi, pointBlank },
                { ItemID.VortexBeater, pointBlank },
                { ItemID.WoodenBow, pointBlank },
                #endregion

                #region CATEGORY 6: Summoner Quality of Life
                { ItemID.BabyBirdStaff, Do(UseExact(35)) }, // Finch Staff
                { ItemID.DD2BallistraTowerT2Popper, Do(UseExact(25)) }, // Ballista Tier 2
                { ItemID.DD2BallistraTowerT3Popper, Do(UseExact(20)) }, // Ballista Tier 3
                { ItemID.DD2ExplosiveTrapT2Popper, Do(UseExact(25)) }, // Explosive Trap Tier 2
                { ItemID.DD2ExplosiveTrapT3Popper, Do(UseExact(20)) }, // Explosive Trap Tier 3
                { ItemID.DD2FlameburstTowerT2Popper, Do(UseExact(25)) }, // Flameburst Tier 2
                { ItemID.DD2FlameburstTowerT3Popper, Do(UseExact(20)) }, // Flameburst Tier 3
                { ItemID.DD2LightningAuraT2Popper, Do(UseExact(25)) }, // Lightning Aura Tier 2
                { ItemID.DD2LightningAuraT3Popper, Do(UseExact(20)) }, // Lightning Aura Tier 3
                { ItemID.DeadlySphereStaff, Do(UseExact(20)) },
                { ItemID.FlinxStaff, Do(UseExact(35)) },
                { ItemID.OpticStaff, Do(UseExact(25)) },
                { ItemID.PirateStaff, Do(UseExact(25)) },
                { ItemID.QueenSpiderStaff, Do(UseExact(25)) },
                { ItemID.RainbowCrystalStaff, Do(UseExact(15)) },
                { ItemID.SanguineStaff, Do(UseExact(25)) },
                { ItemID.SlimeStaff, Do(UseExact(30)) },
                { ItemID.SpiderStaff, Do(UseExact(25)) },
                { ItemID.StaffoftheFrostHydra, Do(UseExact(20)) },
                { ItemID.StardustCellStaff, Do(UseExact(20)) },
                { ItemID.TempestStaff, Do(UseExact(20)) },
                { ItemID.VampireFrogStaff, Do(UseExact(30)) },
                { ItemID.XenoStaff, Do(UseExact(20)) },
                #endregion

                #region CATEGORY 7: UseTurn
                { ItemID.BladeofGrass, Do(UseTurn) },
                { ItemID.BloodButcherer, Do(UseTurn) },
                { ItemID.BoneSword, Do(UseTurn) },
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

                #region CATEGORY 8: Non-consumable Quality of Life
                { ItemID.Abeemination, nonConsumableBossSummon },
                { ItemID.BloodMoonStarter, nonConsumableBossSummon }, // Bloody Tear
                { ItemID.BloodySpine, nonConsumableBossSummon },
                { ItemID.CelestialSigil, nonConsumableBossSummon },
                { ItemID.DeerThing, nonConsumableBossSummon },
                { ItemID.GoblinBattleStandard, nonConsumableBossSummon },
                { ItemID.MechanicalEye, nonConsumableBossSummon },
                { ItemID.MechanicalSkull, nonConsumableBossSummon },
                { ItemID.MechanicalWorm, nonConsumableBossSummon },
                { ItemID.MechdusaSummon, nonConsumableBossSummon }, // Ocram's Razor
                { ItemID.NaughtyPresent, nonConsumableBossSummon },
                { ItemID.PirateMap, nonConsumableBossSummon },
                { ItemID.PumpkinMoonMedallion, nonConsumableBossSummon },
                { ItemID.QueenSlimeCrystal, nonConsumableBossSummon }, // Gelatin Crystal
                { ItemID.SlimeCrown, nonConsumableBossSummon },
                { ItemID.SnowGlobe, nonConsumableBossSummon },
                { ItemID.SolarTablet, nonConsumableBossSummon },
                { ItemID.SuspiciousLookingEye, nonConsumableBossSummon },
                { ItemID.WormFood, nonConsumableBossSummon },
                #endregion

                #region CATEGORY 9: Sell Prices
                { ItemID.Apple, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Apricot, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Banana, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.BlackCurrant, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.BloodOrange, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Cherry, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Coconut, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Dragonfruit, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Elderberry, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.EncumberingStone, Do(Worthless) },
                { ItemID.GlowingMushroom, Do(Worthless) },
                { ItemID.Grapefruit, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Lemon, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Mango, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Mushroom, Do(Worthless) },
                { ItemID.Peach, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Pineapple, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Plum, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Pomegranate, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Rambutan, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.SpicyPepper, Do(Value(Item.sellPrice(copper: 40))) },
                { ItemID.Starfruit, Do(Value(Item.sellPrice(copper: 40))) },
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

        #region Point Blank
        internal class PointBlankRule : IItemTweak
        {
            public bool AppliesTo(Item it) => true;
            public void ApplyTweak(Item it) => it.Calamity().canFirePointBlankShots = true;
        }
        internal static IItemTweak PointBlank => new PointBlankRule();
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

            //Fuck vanilla's stupid Giant Shelly, Crawdad, and Salamander exclusivity 
            shimmerTransmute[ModContent.ItemType<CrawCarapace>()] = ModContent.ItemType<GiantShell>();
            shimmerTransmute[ModContent.ItemType<GiantShell>()] = ModContent.ItemType<CrawCarapace>();

            //Jelly swap'n
            shimmerTransmute[ModContent.ItemType<LifeJelly>()] = ModContent.ItemType<CleansingJelly>();
            shimmerTransmute[ModContent.ItemType<CleansingJelly>()] = ModContent.ItemType<VitalJelly>();
            shimmerTransmute[ModContent.ItemType<VitalJelly>()] = ModContent.ItemType<LifeJelly>();
        }
        #endregion
    }
}
