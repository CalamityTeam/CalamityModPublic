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
            IItemTweak[] autoReuse = Do(AutoReuse);
            IItemTweak[] nonConsumableBossSummon = Do(MaxStack(1), NotConsumable, UseTimeExact(10));

            // SORTING NOTES:
            // 1. Sort tweaks by categories first, then sort by the internal name in alphabetical order. Navigate through categories and names using the search function.
            // 2. Higher categories hold priority over lower ones (ie. Balancing with price tweaks belong in balancing, rather than price)
            // 3. Items with different display names as opposed to internal ones should have comments for display names for ease of access.
            currentTweaks = new SortedDictionary<int, IItemTweak[]>
            {
                #region CATEGORY 1: Weapon Balancing
                { ItemID.AdamantiteGlaive, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(65), ShootSpeedRatio(1.25f)) },
                { ItemID.AdamantiteRepeater, Do(PointBlank, UseExact(18), DamageExact(61)) },
                { ItemID.AdamantiteSword, Do(UseTurn, ScaleRatio(1.45f), DamageExact(77)) },
                { ItemID.AmberStaff, Do(UseTimeExact(15), UseAnimationExact(45), ReuseDelayExact(15)) },
                { ItemID.AmethystStaff, Do(ManaExact(2)) },
                { ItemID.Anchor, Do(DamageExact(107), UseExact(30)) },
                { ItemID.AntlionClaw, Do(UseExact(14)) }, // Mandible Blade
                { ItemID.Bananarang, Do(DamageExact(98), UseExact(14)) },
                { ItemID.BatScepter, Do(DamageExact(56)) },
                { ItemID.BeamSword, Do(UseMeleeSpeed, DamageExact(200), UseAnimationExact(60), ShootSpeedExact(23f)) },
                { ItemID.BeeGun, Do(DamageExact(11)) },
                { ItemID.BeesKnees, Do(PointBlank, DamageExact(18), UseExact(38)) },
                { ItemID.Bladetongue, Do(UseTurn, UseRatio(0.8f), DamageExact(120), ScaleRatio(1.75f)) },
                { ItemID.BlizzardStaff, Do(DamageExact(41), ManaExact(7)) },
                { ItemID.BloodyMachete, Do(AutoReuse, DamageExact(24)) },
                { ItemID.Blowgun, Do(PointBlank, DamageExact(40)) },
                { ItemID.BluePhaseblade, Do(AutoReuse, UseTurn, DamageExact(32)) },
                { ItemID.BluePhasesaber, Do(ScaleRatio(1.5f), DamageExact(60)) },
                { ItemID.BookofSkulls, Do(ManaExact(12), ShootSpeedExact(5.5f)) },
                { ItemID.BookStaff, Do(ManaExact(14)) }, // Tome of Infinite Wisdom
                { ItemID.Boomstick, Do(PointBlank, DamageExact(11)) },
                { ItemID.BreakerBlade, Do(AutoReuse, UseTurn, ScaleRatio(1.4f), DamageExact(97)) },
                { ItemID.Cascade, Do(AutoReuse, DamageExact(30)) },
                { ItemID.ChainGuillotines, Do(DamageExact(100)) },
                { ItemID.ChainKnife, Do(AutoReuse, DamageRatio(1.34f)) },  // Uses ratios due to remix seed
                // Charged Blaster Cannon is now an earlier Last Prism-like, so it will probably need careful balance attention.
                // { ItemID.ChargedBlasterCannon, Do(DamageRatio(1.33f)) },
                { ItemID.ChlorophyteArrow, Do(DamageRatio(1.1f)) },
                { ItemID.ChlorophyteClaymore, Do(UseMeleeSpeed) },
                { ItemID.ChlorophytePartisan, Do(AutoReuse, UseMeleeSpeed, UseRatio(0.8f), DamageExact(70)) },
                { ItemID.ChlorophyteSaber, Do(UseMeleeSpeed, DamageExact(80), UseExact(10)) },
                { ItemID.ChlorophyteShotbow, Do(PointBlank, DamageExact(80), UseExact(50)) },
                { ItemID.ChristmasTreeSword, Do(AutoReuse, UseTurn, UseMeleeSpeed) },
                { ItemID.ClingerStaff, Do(DamageExact(118)) },
                { ItemID.ClockworkAssaultRifle, Do(PointBlank, DamageExact(21)) },
                { ItemID.CobaltNaginata, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(90)) },
                { ItemID.CobaltRepeater, Do(PointBlank, DamageExact(50), UseExact(20)) },
                { ItemID.CobaltSword, Do(UseTurn, ScaleRatio(1.4f), DamageExact(80)) },
                { ItemID.CorruptYoyo, Do(AutoReuse, DamageExact(20)) }, // Malaise
                { ItemID.CrimsonYoyo, Do(AutoReuse, DamageExact(20)) }, // Artery
                { ItemID.CrystalDart, Do(DamageExact(20)) },
                { ItemID.CrystalSerpent, Do(DamageExact(45)) },
                { ItemID.CrystalStorm, Do(DamageExact(40))},
                { ItemID.CursedDart, Do(DamageExact(25)) },
                { ItemID.Cutlass, Do(UseRatio(0.9f), DamageRatio(1.85f)) },
                // Vanilla 1.4 nerfed Daedalus Stormbow themselves. If further nerfs are needed, apply them here.
                // { ItemID.DaedalusStormbow, Do(DamageRatio(0.9f)) },
                { ItemID.DaoofPow, Do(DamageExact(160)) },
                { ItemID.DarkLance, Do(AutoReuse, TrueMelee, DamageExact(45)) },
                { ItemID.DartRifle, Do(PointBlank, DamageExact(58)) },
                { ItemID.DD2BetsyBow, Do(DamageRatio(1.1f)) }, // Aerial Bane's ridiculous multiplier is removed, so this compensates for that
                { ItemID.DD2SquireBetsySword, Do(UseMeleeSpeed) }, // Flying Dragon
                { ItemID.DD2SquireDemonSword, Do(DamageExact(110), UseExact(25)) }, // Brand of the Inferno
                { ItemID.DeathSickle, Do(UseMeleeSpeed, DamageExact(82), ShootSpeedExact(15f)) },
                { ItemID.DemonBow, Do(PointBlank, DamageExact(12), AutoReuse) },
                { ItemID.DemonScythe, Do(AutoReuse, DamageExact(33)) },
                { ItemID.DyeTradersScimitar, Do(AutoReuse, UseTurn, DamageExact(24)) }, // Exotic Scimitar
                { ItemID.ElectrosphereLauncher, Do(DamageRatio(1.1f)) },
                { ItemID.ElfMelter, Do(DamageExact(84), ShootSpeedDelta(+3f)) },
                { ItemID.EmeraldStaff, Do(DamageExact(27)) },
                { ItemID.EmpressBlade, Do(AutoReuse, DamageExact(60), UseExact(20)) }, // Terraprisma
                { ItemID.EnchantedBoomerang, Do(DamageExact(24)) },
                { ItemID.EnchantedSword, Do(UseMeleeSpeed, DamageExact(30), ShootSpeedExact(15f)) },
                { ItemID.Excalibur, Do(TrueMelee, ScaleRatio(1.2f), UseRatio(0.8f), DamageExact(125), UseAnimationExact(45)) },
                { ItemID.FairyQueenMagicItem, Do(DamageExact(54)) }, // Nightglow
                { ItemID.FalconBlade, Do(UseTurn, UseExact(15)) },
                // Unsure what to do with Celebration. Should it be treated as a serious weapon or not? Currently not changing it from vanilla.
                // { ItemID.FireworksLauncher, Do(DamageRatio(2f)) }, // Celebration
                { ItemID.Flamarang, Do(DamageExact(43)) },
                { ItemID.Flamelash, Do(DamageRatio(1.25f)) },
                { ItemID.Flamethrower, Do(DamageExact(42), ShootSpeedDelta(+3f)) },
                { ItemID.FlowerofFire, Do(AutoReuse, ManaExact(7), UseRatio(0.88f)) }, // Uses ratios due to remix seed
                { ItemID.FlowerofFrost, Do(AutoReuse, ManaExact(7), UseExact(30), DamageExact(70), ShootSpeedExact(14)) },
                { ItemID.FlyingKnife, Do(DamageExact(70)) },
                { ItemID.Frostbrand, Do(UseMeleeSpeed, DamageExact(66)) },
                { ItemID.FrostStaff, Do(DamageExact(160), UseExact(37), ShootSpeedExact(20f)) }, // has 1 extra update
                { ItemID.Gatligator, Do(PointBlank, UseExact(6)) },
                { ItemID.GoldenShower, Do(DamageExact(44)) },
                { ItemID.GoldShortsword, Do(AutoReuse, TrueMelee, DamageExact(17)) },
                { ItemID.GolemFist, Do(DamageExact(150)) },
                { ItemID.GreenPhaseblade, Do(AutoReuse, UseTurn, DamageExact(32)) },
                { ItemID.GreenPhasesaber, Do(ScaleRatio(1.5f), DamageExact(60)) },
                { ItemID.GrenadeLauncher, Do(DamageRatio(1.5f)) },
                { ItemID.Gungnir, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(92), ShootSpeedRatio(1.25f)) },
                { ItemID.HallowedRepeater, Do(PointBlank, UseExact(14), DamageExact(57)) },
                { ItemID.Handgun, Do(PointBlank, UseExact(27), DamageExact(36)) },
                { ItemID.HellwingBow, Do(PointBlank, DamageExact(16)) },
                { ItemID.HighVelocityBullet, Do(DamageExact(15)) },
                { ItemID.IceBlade, Do(UseMeleeSpeed) },
                { ItemID.IceBoomerang, Do(DamageExact(25), UseExact(25), ShootSpeedExact(9)) },
                { ItemID.IceBow, Do(PointBlank, AutoReuse, DamageRatio(3f), UseRatio(2.5f)) }, // Uses ratios due to remix seed
                { ItemID.IceRod, Do(UseExact(6), DamageExact(30), ShootSpeedExact(20)) },
                { ItemID.IceSickle, Do(AutoReuse, UseMeleeSpeed, DamageExact(75), ShootSpeedExact(20f)) },
                { ItemID.InfernoFork, Do(DamageRatio(1.2f)) },
                { ItemID.InfluxWaver, Do(UseMeleeSpeed, DamageExact(82)) },
                { ItemID.IronShortsword, Do(AutoReuse, TrueMelee, DamageExact(10)) },
                { ItemID.JestersArrow, Do(DamageExact(6)) },
                { ItemID.Keybrand, Do(UseTurn, ScaleRatio(1.5f)) }, // Uses ratios due to remix seed
                { ItemID.KOCannon, Do(DamageRatio(2.65f)) }, // Uses ratios due to remix seed
                { ItemID.LaserRifle, Do(DamageExact(46), UseExact(10), ManaExact(4)) },
                { ItemID.LastPrism, Do(DamageRatio(0.75f)) },
                { ItemID.LeadShortsword, Do(AutoReuse, TrueMelee, DamageExact(11)) },
                { ItemID.LightDisc, Do(DamageExact(128), ShootSpeedExact(18)) },
                { ItemID.LunarFlareBook, Do(DamageRatio(1.2f)) },
                { ItemID.MagicalHarp, Do(DamageExact(50), ShootSpeedExact(12f)) },
                { ItemID.MagicDagger, Do(DamageRatio(1.8f), UseRatio(1.88f), ShootSpeedExact(30)) }, // Uses ratios due to remix seed
                { ItemID.MagicMissile, Do(DamageExact(23), ManaExact(10), UseAnimationExact(20), UseTimeExact(10)) },
                { ItemID.MagnetSphere, Do(DamageRatio(1.1f)) },
                { ItemID.Marrow, Do(PointBlank, DamageExact(69)) },
                { ItemID.MedusaHead, Do(ManaExact(6), DamageRatio(1.2f)) },
                { ItemID.Meowmere, Do(UseMeleeSpeed/*, DamageRatio(1.33f) */) },
                { ItemID.MeteorStaff, Do(DamageExact(58), ManaExact(7), ShootSpeedExact(13f)) },
                { ItemID.Minishark, Do(PointBlank, DamageExact(4)) },
                { ItemID.MoltenFury, Do(PointBlank, UseExact(29), AutoReuse) },
                { ItemID.MonkStaffT1, Do(TrueMeleeNoSpeed, DamageExact(83)) }, // Sleepy Octopod
                { ItemID.MonkStaffT2, Do(AutoReuse, TrueMelee, DamageRatio(2f)) }, // Ghastly Glaive
                { ItemID.MonkStaffT3, Do(DamageExact(225)) }, // Sky Dragon's Fury
                { ItemID.Muramasa, Do(CritDelta(+20)) },
                { ItemID.MushroomSpear, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(100)) },
                { ItemID.Musket, Do(PointBlank, DamageExact(22)) },
                { ItemID.MythrilHalberd, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(95), ShootSpeedRatio(1.25f)) },
                { ItemID.MythrilRepeater, Do(PointBlank, DamageExact(58), UseExact(19)) },
                { ItemID.MythrilSword, Do(UseTurn, ScaleRatio(1.45f), DamageExact(100)) },
                { ItemID.NorthPole, Do(AutoReuse, UseMeleeSpeed) },
                { ItemID.OrangePhaseblade, Do(AutoReuse, UseTurn, DamageExact(32)) },
                { ItemID.OrangePhasesaber, Do(ScaleRatio(1.5f), DamageExact(60)) },
                { ItemID.OrichalcumHalberd, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(98), ShootSpeedRatio(1.25f)) },
                { ItemID.OrichalcumRepeater, Do(PointBlank, DamageExact(86), UseExact(27)) },
                { ItemID.OrichalcumSword, Do(UseTurn, ScaleRatio(1.45f), DamageExact(82)) },
                { ItemID.PainterPaintballGun, Do(PointBlank, DamageExact(8)) },
                { ItemID.PaladinsHammer, Do(DamageExact(100), ShootSpeedExact(23)) },
                { ItemID.PalladiumPike, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(96)) },
                { ItemID.PalladiumRepeater, Do(PointBlank, UseExact(25), DamageExact(75)) },
                { ItemID.PalladiumSword, Do(UseTurn, ScaleRatio(1.4f), DamageExact(100)) },
                { ItemID.PearlwoodBow, Do(AutoReuse, PointBlank, DamageRatio(2.31f), UseDelta(+8), ShootSpeedDelta(+3.4f), KnockbackDelta(+1f)) },
                { ItemID.PearlwoodSword, Do(UseTurn, DamageRatio(1.5f)) },
                { ItemID.PewMaticHorn, Do(DamageExact(24)) },
                { ItemID.PhoenixBlaster, Do(AutoReuse, PointBlank, UseExact(18)) },
                { ItemID.PlatinumBow, Do(PointBlank, DamageExact(12)) },
                { ItemID.PlatinumShortsword, Do(AutoReuse, TrueMelee, DamageExact(18)) },
                { ItemID.PoisonStaff, Do(DamageExact(57)) },
                { ItemID.PsychoKnife, Do(UseTurn, UseExact(11), DamageRatio(3f)) },
                { ItemID.PurpleClubberfish, Do(UseTurn, ScaleRatio(1.2f), KnockbackExact(10f)) },
                { ItemID.PurplePhaseblade, Do(AutoReuse, UseTurn, DamageExact(32)) },
                { ItemID.PurplePhasesaber, Do(ScaleRatio(1.5f), DamageExact(60)) },
                { ItemID.RainbowRod, Do(DamageExact(35), ManaExact(15)) },
                { ItemID.Rally, Do(AutoReuse, DamageExact(18)) },
                { ItemID.Razorpine, Do(DamageRatio(0.75f)) },
                { ItemID.RedPhaseblade, Do(AutoReuse, UseTurn, DamageExact(32)) },
                { ItemID.RedPhasesaber, Do(ScaleRatio(1.5f), DamageExact(60)) },
                { ItemID.RedRyder, Do(PointBlank, DamageExact(24)) },
                { ItemID.RocketIII, Do(DamageRatio(0.75f)) },
                { ItemID.RocketIV, Do(DamageRatio(0.75f)) },
                { ItemID.SapphireStaff, Do(DamageExact(25)) },
                { ItemID.Seedler, Do(UseMeleeSpeed, DamageRatio(1.5f)) },
                { ItemID.ShadowbeamStaff, Do(DamageExact(100)) },
                { ItemID.ShadowFlameBow, Do(PointBlank, DamageExact(55)) },
                { ItemID.ShadowFlameHexDoll, Do(DamageExact(40), ShootSpeedExact(30)) },
                { ItemID.ShadowFlameKnife, Do(DamageExact(70)) },
                { ItemID.Shotgun, Do(PointBlank, DamageExact(36), AutoReuse) },
                { ItemID.Shroomerang, Do(ShootSpeedExact(11)) },
                { ItemID.SilverBullet, Do(DamageExact(8)) },
                { ItemID.SilverShortsword, Do(AutoReuse, TrueMelee, DamageExact(14)) },
                { ItemID.SkyFracture, Do(DamageExact(54), ShootSpeedExact(30f)) },
                { ItemID.SlapHand, Do(UseTurn, ScaleRatio(1.5f), DamageExact(120)) },
                { ItemID.Smolstar, Do(DamageExact(9), AutoReuse, UseExact(25)) }, // Blade Staff
                { ItemID.SolarEruption, Do(DamageRatio(1.5f)) },
                // Life Drain could probably get a bigger buff
                { ItemID.SoulDrain, Do(DamageRatio(1.1f)) }, // Life Drain
                { ItemID.SpaceGun, Do(DamageExact(25)) },
                { ItemID.Spear, Do(AutoReuse, TrueMelee, DamageExact(14)) },
                { ItemID.SpectreStaff, Do(DamageRatio(1.2f)) },
                { ItemID.SpiritFlame, Do(UseExact(20), ManaExact(11), ShootSpeedExact(2f)) },
                { ItemID.StaffofEarth, Do(DamageRatio(1.2f)) },
                { ItemID.StakeLauncher, Do(PointBlank, DamageRatio(2f), UseRatio(1.5f)) },
                { ItemID.StarCannon, Do(DamageExact(25)) },
                { ItemID.StardustDragonStaff, Do(AutoReuse, DamageExact(20), UseExact(19)) },
                { ItemID.StarWrath, Do(DamageRatio(0.9f)) },
                { ItemID.StormTigerStaff, Do(AutoReuse, DamageExact(49), UseExact(20)) }, // Desert Tiger Staff
                { ItemID.StylistKilLaKillScissorsIWish, Do(AutoReuse, UseTurn, DamageExact(18)) }, // Stylish Scissors
                { ItemID.Swordfish, Do(AutoReuse, TrueMelee, DamageExact(24)) },
                { ItemID.TacticalShotgun, Do(PointBlank, DamageRatio(1.2f)) },
                { ItemID.TaxCollectorsStickOfDoom, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), UseRatio(0.8f), DamageExact(70)) },
                { ItemID.TendonBow, Do(PointBlank, DamageExact(17), AutoReuse) },
                // Vanilla damage 190. After fixing iframes so yoyo and shots can hit simultaneously,
                // Terrarian is extremely overpowered and requires a heavy nerf.
                { ItemID.Terrarian, Do(AutoReuse, DamageExact(106)) },
                { ItemID.TheRottedFork, Do(AutoReuse, TrueMelee, DamageExact(20)) },
                { ItemID.TheUndertaker, Do(PointBlank, AutoReuse, DamageExact(15)) },
                { ItemID.ThunderSpear, Do(AutoReuse, UseMeleeSpeed) }, // Storm Spear
                { ItemID.TitaniumRepeater, Do(PointBlank, UseExact(29), DamageExact(122)) },
                { ItemID.TitaniumSword, Do(UseTurn, ScaleRatio(1.45f), DamageExact(77)) },
                { ItemID.TitaniumTrident, Do(AutoReuse, TrueMelee, UseRatio(0.8f), DamageExact(72), ShootSpeedRatio(1.25f)) },
                { ItemID.TopazStaff, Do(ManaExact(2)) },
                { ItemID.Toxikarp, Do(UseTimeExact(7), UseAnimationExact(14)) },
                { ItemID.Trident, Do(AutoReuse, TrueMelee, DamageExact(20)) },
                { ItemID.Trimarang, Do(DamageExact(24)) },
                { ItemID.TrueExcalibur, Do(TrueMelee, DamageExact(82)) },
                { ItemID.TrueNightsEdge, Do(DamageExact(80), ScaleRatio(1.2f)) },
                { ItemID.Tsunami, Do(PointBlank, DamageRatio(1.25f)) },
                { ItemID.TungstenShortsword, Do(AutoReuse, TrueMelee, DamageExact(15)) },
                { ItemID.UnholyArrow, Do(DamageExact(11)) },
                { ItemID.UnholyTrident, Do(ManaRatio(0.78f), DamageRatio(1.25f)) },  // Uses ratios due to remix seed
                { ItemID.VampireKnives, Do(DamageRatio(1.33f)) },
                { ItemID.VenomStaff, Do(DamageRatio(1.5f)) },
                { ItemID.WaspGun, Do(UseExact(11)) },
                { ItemID.WaterBolt, Do(DamageExact(23)) },
                { ItemID.WhitePhaseblade, Do(AutoReuse, UseTurn, DamageExact(32)) },
                { ItemID.WhitePhasesaber, Do(ScaleRatio(1.5f), DamageExact(60)) },
                { ItemID.WoodenBoomerang, Do(DamageRatio(2f), Value(Item.sellPrice(copper: 20))) },
                { ItemID.Xenopopper, Do(DamageRatio(0.75f)) },
                { ItemID.YellowPhaseblade, Do(AutoReuse, UseTurn, DamageExact(32)) },
                { ItemID.YellowPhasesaber, Do(ScaleRatio(1.5f), DamageExact(60)) },
                { ItemID.ZombieArm, Do(AutoReuse, UseTurn, ScaleRatio(1.25f), KnockbackExact(12f)) },
                #endregion

                #region CATEGORY 2: Defense Balancing
                { ItemID.AncientHallowedGreaves, Do(DefenseDelta(+2)) },
                { ItemID.AncientHallowedPlateMail, Do(DefenseDelta(+3)) },
                { ItemID.AnkhShield, Do(DefenseDelta(+8)) },
                { ItemID.CobaltShield, Do(DefenseDelta(+3)) },
                { ItemID.EoCShield, Do(DefenseDelta(+1)) }, // Shield of Cthulhu
                { ItemID.FrozenShield, Do(DefenseDelta(+7)) },
                { ItemID.FrozenTurtleShell, Do(DefenseExact(6)) },
                { ItemID.HallowedGreaves, Do(DefenseDelta(+2)) },
                { ItemID.HallowedPlateMail, Do(DefenseDelta(+3)) },
                { ItemID.HeroShield, Do(DefenseDelta(+10)) },
                { ItemID.LavaSkull, Do(DefenseExact(4)) }, // Magma Skull
                { ItemID.MoltenSkullRose, Do(DefenseExact(8)) },
                { ItemID.ObsidianShield, Do(DefenseDelta(+5)) },
                { ItemID.ObsidianSkull, Do(DefenseDelta(+1)) },
                { ItemID.ObsidianSkullRose, Do(DefenseExact(4)) },
                { ItemID.OrichalcumBreastplate, Do(DefenseDelta(+3)) },
                { ItemID.OrichalcumHeadgear, Do(DefenseDelta(+2)) },
                { ItemID.OrichalcumHelmet, Do(DefenseDelta(+3)) },
                { ItemID.OrichalcumLeggings, Do(DefenseDelta(+4)) },
                { ItemID.OrichalcumMask, Do(DefenseDelta(+3)) },
                { ItemID.PaladinsShield, Do(DefenseDelta(+3)) },
                { ItemID.PalladiumBreastplate, Do(DefenseDelta(+3)) },
                { ItemID.PalladiumHeadgear, Do(DefenseDelta(+2)) },
                { ItemID.PalladiumHelmet, Do(DefenseDelta(+3)) },
                { ItemID.PalladiumMask, Do(DefenseDelta(+1)) },
                { ItemID.PalladiumLeggings, Do(DefenseDelta(+3)) },
                { ItemID.Shackle, Do(DefenseDelta(+2)) },
                #endregion

                #region CATEGORY 3: Tool Balancing
                { ItemID.AcornAxe, Do(AxePower(125)) }, // Axe of Regrowth
                { ItemID.AdamantiteChainsaw, Do(TrueMeleeNoSpeed, AxePower(90), UseTimeExact(4), TileBoostExact(+0)) },
                { ItemID.AdamantiteDrill, Do(TrueMeleeNoSpeed, PickPower(180), UseTimeExact(4), TileBoostExact(+1)) },
                { ItemID.AdamantitePickaxe, Do(PickPower(180), UseTimeExact(8), TileBoostExact(+1)) },
                { ItemID.AdamantiteWaraxe, Do(AxePower(160), UseTimeExact(10), TileBoostExact(+1)) },
                { ItemID.BloodLustCluster, Do(AxePower(100), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.BonePickaxe, Do(PickPower(55), UseTimeExact(6)) },
                { ItemID.BorealWoodHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.ButchersChainsaw, Do(TrueMeleeNoSpeed, AxePower(150), UseTimeExact(3), TileBoostExact(+0)) },
                { ItemID.CactusPickaxe, Do(PickPower(34), UseTimeExact(9)) },
                { ItemID.CnadyCanePickaxe, Do(PickPower(55), UseTimeExact(9), TileBoostExact(+1)) }, // Candy Cane Pickaxe
                { ItemID.ChlorophyteChainsaw, Do(TrueMeleeNoSpeed, AxePower(120), UseTimeExact(3), TileBoostExact(+0)) },
                { ItemID.ChlorophyteDrill, Do(TrueMeleeNoSpeed, PickPower(200), UseTimeExact(4), TileBoostExact(+2)) },
                { ItemID.ChlorophyteGreataxe, Do(AxePower(165), UseTimeExact(7), TileBoostExact(+2)) },
                { ItemID.ChlorophyteJackhammer, Do(TrueMeleeNoSpeed, HammerPower(90), UseTimeExact(5), TileBoostExact(+0)) },
                { ItemID.ChlorophytePickaxe, Do(PickPower(200), UseTimeExact(7), TileBoostExact(+2)) },
                { ItemID.ChlorophyteWarhammer, Do(HammerPower(90), UseTimeExact(8), TileBoostExact(+2)) },
                { ItemID.CobaltChainsaw, Do(TrueMeleeNoSpeed, AxePower(70), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.CobaltDrill, Do(TrueMeleeNoSpeed, PickPower(130), UseTimeExact(5)) },
                { ItemID.CobaltPickaxe, Do(PickPower(130), UseTimeExact(9)) },
                { ItemID.CobaltWaraxe, Do(AxePower(125), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.CopperAxe, Do(AxePower(50), UseTimeExact(16), TileBoostExact(+0)) },
                { ItemID.CopperHammer, Do(HammerPower(35), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.CopperPickaxe, Do(PickPower(35), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.DeathbringerPickaxe, Do(PickPower(70), UseTimeExact(10)) },
                { ItemID.Drax, Do(TrueMeleeNoSpeed, PickPower(200), AxePower(110), UseTimeExact(4), TileBoostExact(+1)) },
                { ItemID.EbonwoodHammer, Do(HammerPower(25), UseTimeExact(9), TileBoostExact(+0)) },
                { ItemID.FleshGrinder, Do(HammerPower(70), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.GoldAxe, Do(AxePower(80), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.GoldHammer, Do(HammerPower(60), UseTimeExact(9), TileBoostExact(+0)) },
                { ItemID.GoldPickaxe, Do(PickPower(55), UseTimeExact(9)) },
                { ItemID.Hammush, Do(HammerPower(85), UseTimeExact(10), TileBoostExact(+1)) },
                { ItemID.IronAxe, Do(AxePower(60), UseTimeExact(15), TileBoostExact(+0)) },
                { ItemID.IronHammer, Do(HammerPower(45), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.IronPickaxe, Do(PickPower(40), UseTimeExact(8)) },
                { ItemID.LaserDrill, Do(PickPower(220), AxePower(120), UseTimeExact(4)) },
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
                { ItemID.MythrilChainsaw, Do(TrueMeleeNoSpeed, AxePower(80), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.MythrilDrill, Do(TrueMeleeNoSpeed, PickPower(160), UseTimeExact(4)) },
                { ItemID.MythrilPickaxe, Do(PickPower(160), UseTimeExact(8)) },
                { ItemID.MythrilWaraxe, Do(AxePower(140), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.NebulaDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
                { ItemID.NebulaPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.NettleBurst, Do(ManaExact(10), DamageExact(43)) },
                { ItemID.NightmarePickaxe, Do(PickPower(66), UseTimeExact(9)) },
                { ItemID.OrichalcumChainsaw, Do(TrueMeleeNoSpeed, AxePower(80), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.OrichalcumDrill, Do(TrueMeleeNoSpeed, PickPower(160), UseTimeExact(4)) },
                { ItemID.OrichalcumPickaxe, Do(PickPower(160), UseTimeExact(8)) },
                { ItemID.OrichalcumWaraxe, Do(AxePower(140), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.PalladiumChainsaw, Do(TrueMeleeNoSpeed, AxePower(70), UseTimeExact(4), TileBoostExact(-1)) },
                { ItemID.PalladiumDrill, Do(TrueMeleeNoSpeed, PickPower(130), UseTimeExact(5)) },
                { ItemID.PalladiumPickaxe, Do(PickPower(130), UseTimeExact(9)) },
                { ItemID.PalladiumWaraxe, Do(AxePower(125), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.PalmWoodHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
                { ItemID.PearlwoodHammer, Do(HammerPower(25), UseTimeExact(4), UseAnimationExact(20), DamageRatio(4f), TileBoostExact(+0)) },
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
                { ItemID.SolarFlareDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
                { ItemID.SolarFlarePickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.SpectreHamaxe, Do(HammerPower(90), AxePower(170), TileBoostExact(+4)) },
                { ItemID.SpectrePickaxe, Do(PickPower(200), TileBoostExact(+4)) },
                { ItemID.StardustDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
                { ItemID.StardustPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.TheAxe, Do(HammerPower(100), AxePower(175), UseTimeExact(7), TileBoostExact(+1)) },
                { ItemID.TheBreaker, Do(HammerPower(70), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.TinAxe, Do(AxePower(50), UseTimeExact(16), TileBoostExact(+0)) },
                { ItemID.TinHammer, Do(HammerPower(35), UseTimeExact(12), TileBoostExact(+0)) },
                { ItemID.TinPickaxe, Do(PickPower(35), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.TitaniumChainsaw, Do(TrueMeleeNoSpeed, AxePower(90), UseTimeExact(4), TileBoostExact(+0)) },
                { ItemID.TitaniumDrill, Do(TrueMeleeNoSpeed, PickPower(180), UseTimeExact(4), TileBoostExact(+1)) },
                { ItemID.TitaniumPickaxe, Do(PickPower(180), UseTimeExact(8), TileBoostExact(+1)) },
                { ItemID.TitaniumWaraxe, Do(AxePower(160), UseTimeExact(10), TileBoostExact(+1)) },
                { ItemID.TungstenAxe, Do(AxePower(70), UseTimeExact(14), TileBoostExact(+0)) },
                { ItemID.TungstenHammer, Do(HammerPower(55), UseTimeExact(10), TileBoostExact(+0)) },
                { ItemID.TungstenPickaxe, Do(PickPower(50), UseTimeExact(11)) },
                { ItemID.VortexDrill, Do(TrueMeleeNoSpeed, PickPower(225), UseTimeExact(3), TileBoostExact(+4)) },
                { ItemID.VortexPickaxe, Do(PickPower(225), UseTimeExact(6), TileBoostExact(+4)) },
                { ItemID.WarAxeoftheNight, Do(AxePower(100), UseTimeExact(13), TileBoostExact(+0)) },
                { ItemID.WoodenHammer, Do(HammerPower(25), UseTimeExact(11), TileBoostExact(+0)) },
                #endregion

                #region CATEGORY 4: True Melee support
                { ItemID.Arkhalis, trueMeleeNoSpeed },
                { ItemID.CopperShortsword, Do(AutoReuse, TrueMelee) },
                { ItemID.Gladius, Do(AutoReuse, TrueMelee) },
                { ItemID.HallowJoustingLance, trueMelee },
                { ItemID.JoustingLance, trueMelee },
                { ItemID.NebulaChainsaw, trueMeleeNoSpeed },
                { ItemID.NightsEdge, trueMelee },
                { ItemID.ObsidianSwordfish, Do(AutoReuse, TrueMelee) },
                { ItemID.PiercingStarlight, trueMelee }, // Starlight
                { ItemID.Ruler, trueMelee },
                { ItemID.ShadowJoustingLance, trueMelee },
                { ItemID.SolarFlareChainsaw, trueMeleeNoSpeed },
                { ItemID.StardustChainsaw, trueMeleeNoSpeed },
                { ItemID.Terragrim, trueMeleeNoSpeed },
                { ItemID.TheHorsemansBlade, trueMelee },
                { ItemID.TinShortsword, Do(AutoReuse, TrueMelee) },
                { ItemID.VortexChainsaw, trueMeleeNoSpeed },
                #endregion

                #region CATEGORY 5: Point Blank support
                { ItemID.Blowpipe, pointBlank },
                { ItemID.BorealWoodBow, pointBlank },
                { ItemID.CandyCornRifle, pointBlank },
                { ItemID.ChainGun, pointBlank },
                { ItemID.CopperBow, pointBlank },
                { ItemID.DartPistol, pointBlank },
                { ItemID.DD2PhoenixBow, pointBlank }, // Phantom Phoenix
                { ItemID.EbonwoodBow, pointBlank },
                { ItemID.FairyQueenRangedItem, pointBlank }, // Eventide
                { ItemID.FlintlockPistol, pointBlank },
                { ItemID.FlareGun,  Do(PointBlank, Value(Item.sellPrice(silver: 10))) },
                { ItemID.GoldBow, pointBlank },
                { ItemID.Harpoon, pointBlank },
                { ItemID.IronBow, pointBlank },
                { ItemID.LeadBow, pointBlank },
                { ItemID.Megashark, pointBlank },
                { ItemID.OnyxBlaster, pointBlank },
                { ItemID.PalmWoodBow, pointBlank },
                { ItemID.Phantasm, pointBlank },
                { ItemID.PulseBow, pointBlank },
                { ItemID.QuadBarrelShotgun, pointBlank },
                { ItemID.Revolver, Do(PointBlank, AutoReuse) },
                { ItemID.RichMahoganyBow, pointBlank },
                { ItemID.Sandgun, pointBlank },
                { ItemID.SDMG, pointBlank },
                { ItemID.ShadewoodBow, pointBlank },
                { ItemID.SilverBow, pointBlank },
                { ItemID.SniperRifle, pointBlank },
                { ItemID.SnowballCannon, pointBlank },
                { ItemID.TinBow, pointBlank },
                { ItemID.TungstenBow, pointBlank },
                { ItemID.Uzi, pointBlank },
                { ItemID.VenusMagnum, pointBlank },
                { ItemID.VortexBeater, pointBlank },
                { ItemID.WoodenBow, pointBlank },
                #endregion

                #region CATEGORY 6: Summoner Quality of Life
                { ItemID.AbigailsFlower, autoReuse },
                { ItemID.BabyBirdStaff, Do(AutoReuse, UseExact(35)) }, // Finch Staff
                { ItemID.BlandWhip, autoReuse }, // Leather Whip
                { ItemID.BoneWhip, autoReuse }, // Spinal Tap
                { ItemID.CoolWhip, autoReuse },
                { ItemID.DD2BallistraTowerT1Popper, autoReuse }, // Ballista Tier 1
                { ItemID.DD2BallistraTowerT2Popper, Do(AutoReuse, UseExact(25)) }, // Ballista Tier 2
                { ItemID.DD2BallistraTowerT3Popper, Do(AutoReuse, UseExact(20)) }, // Ballista Tier 3
                { ItemID.DD2ExplosiveTrapT1Popper, autoReuse }, // Explosive Trap Tier 1
                { ItemID.DD2ExplosiveTrapT2Popper, Do(AutoReuse, UseExact(25)) }, // Explosive Trap Tier 2
                { ItemID.DD2ExplosiveTrapT3Popper, Do(AutoReuse, UseExact(20)) }, // Explosive Trap Tier 3
                { ItemID.DD2FlameburstTowerT1Popper, autoReuse }, // Flameburst Tier 1
                { ItemID.DD2FlameburstTowerT2Popper, Do(AutoReuse, UseExact(25)) }, // Flameburst Tier 2
                { ItemID.DD2FlameburstTowerT3Popper, Do(AutoReuse, UseExact(20)) }, // Flameburst Tier 3
                { ItemID.DD2LightningAuraT1Popper, autoReuse }, // Lightning Aura Tier 1
                { ItemID.DD2LightningAuraT2Popper, Do(AutoReuse, UseExact(25)) }, // Lightning Aura Tier 2
                { ItemID.DD2LightningAuraT3Popper, Do(AutoReuse, UseExact(20)) }, // Lightning Aura Tier 3
                { ItemID.DeadlySphereStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.FireWhip, autoReuse }, // Firecracker
                { ItemID.FlinxStaff, Do(AutoReuse, UseExact(35)) },
                { ItemID.HornetStaff, Do(AutoReuse, UseExact(30)) },
                { ItemID.ImpStaff, Do(AutoReuse, UseExact(30)) },
                { ItemID.MaceWhip, autoReuse }, // Morning Star
                { ItemID.MoonlordTurretStaff, Do(UseExact(15)) }, // Lunar Portal Staff
                { ItemID.OpticStaff, Do(AutoReuse, UseExact(25)) },
                { ItemID.PirateStaff, Do(AutoReuse, UseExact(25)) },
                { ItemID.PygmyStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.QueenSpiderStaff, Do(UseExact(25)) },
                { ItemID.RainbowCrystalStaff, Do(UseExact(15)) },
                { ItemID.RainbowWhip, autoReuse }, // Kaleidoscope
                { ItemID.RavenStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.SanguineStaff, Do(AutoReuse, UseExact(25)) },
                { ItemID.ScytheWhip, autoReuse }, // Dark Harvest
                { ItemID.SlimeStaff, Do(AutoReuse, UseExact(30)) },
                { ItemID.SpiderStaff, Do(AutoReuse, UseExact(25)) },
                { ItemID.StaffoftheFrostHydra, Do(UseExact(20)) },
                { ItemID.StardustCellStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.SwordWhip, autoReuse }, // Durendal
                { ItemID.TempestStaff, Do(AutoReuse, UseExact(20)) },
                { ItemID.ThornWhip, autoReuse }, // Snapthorn
                { ItemID.VampireFrogStaff, Do(AutoReuse, UseExact(30)) },                
                { ItemID.XenoStaff, Do(AutoReuse, UseExact(20)) },
                #endregion

                #region CATEGORY 7: Other Quality of Life (AutoReuse / UseTurn)
                { ItemID.Amarok, autoReuse },
                { ItemID.BatBat, autoReuse },
                { ItemID.BeeKeeper, Do(UseTurn) },
                { ItemID.BladeofGrass, Do(AutoReuse, UseTurn) },
                { ItemID.BloodButcherer, Do(AutoReuse, UseTurn) },
                { ItemID.BoneSword, Do(AutoReuse, UseTurn) },
                { ItemID.BorealWoodSword, Do(AutoReuse, UseTurn) },
                { ItemID.CactusSword, Do(AutoReuse, UseTurn) },
                { ItemID.CandyCaneSword, Do(AutoReuse, UseTurn) },
                { ItemID.Chik, autoReuse },
                { ItemID.Code1, autoReuse },
                { ItemID.Code2, autoReuse },
                { ItemID.CopperBroadsword, Do(AutoReuse, UseTurn) },
                { ItemID.EbonwoodSword, Do(AutoReuse, UseTurn) },
                { ItemID.FieryGreatsword, Do(AutoReuse, UseTurn) }, // Volcano
                { ItemID.FormatC, autoReuse },
                { ItemID.GoldBroadsword, Do(AutoReuse, UseTurn) },
                { ItemID.Gradient, autoReuse },
                { ItemID.HelFire, autoReuse },
                { ItemID.HiveFive, autoReuse },
                { ItemID.IronBroadsword, Do(AutoReuse, UseTurn) },
                { ItemID.JungleYoyo, autoReuse }, // Amazon
                { ItemID.Kraken, autoReuse },
                { ItemID.LifeCrystal, autoReuse },
                { ItemID.LeadBroadsword, Do(AutoReuse, UseTurn) },
                { ItemID.LifeFruit, autoReuse },
                { ItemID.LightsBane, Do(AutoReuse, UseTurn) },
                { ItemID.ManaCrystal, autoReuse },
                { ItemID.PalmWoodSword, Do(AutoReuse, UseTurn) },
                { ItemID.PaperAirplaneA, autoReuse },
                { ItemID.PaperAirplaneB, autoReuse }, // White Paper Airplane
                { ItemID.PlatinumBroadsword, Do(AutoReuse, UseTurn) },
                { ItemID.RedsYoyo, autoReuse },
                { ItemID.RichMahoganySword, Do(AutoReuse, UseTurn) },
                { ItemID.ShadewoodSword, Do(AutoReuse, UseTurn) },
                { ItemID.SilverBroadsword, Do(AutoReuse, UseTurn) },
                { ItemID.Starfury, autoReuse },
                { ItemID.TentacleSpike, autoReuse },
                { ItemID.TinBroadsword, Do(AutoReuse, UseTurn) },
                { ItemID.TheEyeOfCthulhu, autoReuse },
                { ItemID.TragicUmbrella, autoReuse },
                { ItemID.TungstenBroadsword, Do(AutoReuse, UseTurn) },
                { ItemID.Umbrella, autoReuse },
                { ItemID.ValkyrieYoyo, autoReuse },
                { ItemID.Valor, autoReuse },
                { ItemID.WandofFrosting, autoReuse },
                { ItemID.WandofSparking, autoReuse },
                { ItemID.WeatherPain, autoReuse },
                { ItemID.WoodenSword, Do(AutoReuse, UseTurn) },
                { ItemID.WoodYoyo, autoReuse },
                { ItemID.Yelets, autoReuse },
                { ItemID.ZapinatorGray, autoReuse },
                { ItemID.ZapinatorOrange, autoReuse },
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

        #region Auto Reuse
        internal class AutoReuseRule : IItemTweak
        {
            internal readonly bool flag = true;

            public AutoReuseRule(bool ar) => flag = ar;
            public bool AppliesTo(Item it) => IsUsable(it);
            public void ApplyTweak(Item it) => it.autoReuse = flag;
        }
        internal static IItemTweak AutoReuse => new AutoReuseRule(true);
        internal static IItemTweak NoAutoReuse => new AutoReuseRule(false);
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
