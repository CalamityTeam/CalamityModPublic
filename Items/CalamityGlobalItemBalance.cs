using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public partial class CalamityGlobalItem : GlobalItem
	{
		#region Database and Initialization
		internal static SortedDictionary<int, IBalanceRule[]> balance = null;

		internal static void LoadBalance()
		{
			// Various shorthands for items which receive very simple changes, such as setting one flag.
			IBalanceRule[] trueMelee = Do(TrueMelee);
			IBalanceRule[] pointBlank = Do(PointBlank);
			IBalanceRule[] autoReuse = Do(AutoReuse);
			IBalanceRule[] useTurn = Do(UseTurn);


			// Please keep this strictly alphabetical. It's the only way to keep it sane. Thanks in advance.
			// - Ozzatron
			balance = new SortedDictionary<int, IBalanceRule[]>
			{
				{ ItemID.AdamantiteGlaive, Do(AutoReuse, UseRatio(0.8f)) },
				{ ItemID.AdamantitePickaxe, Do(UseTimeExact(7)) },
				{ ItemID.AdamantiteSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f)) },
				{ ItemID.AntlionClaw, Do(ScaleRatio(1.5f)) },
				// { ItemID.BabyBirdStaff, Do(AutoReuse, UseExact(35)) },
				{ ItemID.BeamSword, autoReuse },
				{ ItemID.BeeKeeper, Do(UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.BladeofGrass, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.Bladetongue, Do(UseTurn, UseRatio(0.8f)) },
				{ ItemID.BloodButcherer, Do(AutoReuse, UseTurn) },
				{ ItemID.BluePhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.BluePhasesaber, Do(ScaleRatio(1.5f)) },
				{ ItemID.BonePickaxe, Do(UseTimeExact(8)) },
				{ ItemID.BoneSword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.BorealWoodSword, Do(AutoReuse, UseTurn) },
				{ ItemID.BreakerBlade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.CactusPickaxe, Do(UseTimeExact(13)) },
				{ ItemID.CactusSword, Do(AutoReuse, UseTurn) },
				{ ItemID.CandyCaneSword, autoReuse },
				{ ItemID.ChainKnife, autoReuse },
				{ ItemID.ChlorophytePartisan, Do(AutoReuse, UseRatio(0.8f)) },
				{ ItemID.ChlorophytePickaxe, Do(UseTimeExact(6)) },
				{ ItemID.ChristmasTreeSword, Do(AutoReuse, UseTurn) },
				{ ItemID.CnadyCanePickaxe, Do(UseTimeExact(10)) },
				{ ItemID.CobaltNaginata, Do(AutoReuse, UseRatio(0.8f)) },
				{ ItemID.CobaltPickaxe, Do(UseTimeExact(9)) },
				{ ItemID.CobaltSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f)) },
				{ ItemID.CopperBroadsword, Do(AutoReuse, UseTurn) },
				{ ItemID.CopperPickaxe, Do(UseTimeExact(13), TileBoostExact(0)) },
				{ ItemID.CopperShortsword, autoReuse },
				{ ItemID.Cutlass, Do(UseRatio(0.8f)) },
				{ ItemID.DarkLance, autoReuse },
				{ ItemID.DD2BallistraTowerT1Popper, Do(UseExact(30)) },
				{ ItemID.DD2BallistraTowerT2Popper, Do(UseExact(25)) },
				{ ItemID.DD2BallistraTowerT3Popper, Do(UseExact(20)) },
				{ ItemID.DD2ExplosiveTrapT1Popper, Do(UseExact(30)) },
				{ ItemID.DD2ExplosiveTrapT2Popper, Do(UseExact(25)) },
				{ ItemID.DD2ExplosiveTrapT3Popper, Do(UseExact(20)) },
				{ ItemID.DD2FlameburstTowerT1Popper, Do(UseExact(30)) },
				{ ItemID.DD2FlameburstTowerT2Popper, Do(UseExact(25)) },
				{ ItemID.DD2FlameburstTowerT3Popper, Do(UseExact(20)) },
				{ ItemID.DD2LightningAuraT1Popper, Do(UseExact(30)) },
				{ ItemID.DD2LightningAuraT2Popper, Do(UseExact(25)) },
				{ ItemID.DD2LightningAuraT3Popper, Do(UseExact(20)) },
				{ ItemID.DeadlySphereStaff, Do(UseExact(20)) },
				{ ItemID.DeathbringerPickaxe, Do(UseTimeExact(10)) },
				{ ItemID.DyeTradersScimitar, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.EbonwoodSword, Do(AutoReuse, UseTurn) },
				// { ItemID.EmpressBlade, Do(UseExact(20)) },
				{ ItemID.EnchantedSword, Do(ScaleRatio(1.5f)) },
				{ ItemID.Excalibur, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f)) },
				{ ItemID.FalconBlade, Do(UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.FieryGreatsword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				// { ItemID.FlinxStaff, Do(AutoReuse, UseExact(35)) },
				{ ItemID.GoldBroadsword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.GoldPickaxe, Do(UseTimeExact(10)) },
				{ ItemID.GoldShortsword, autoReuse },
				{ ItemID.GreenPhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.GreenPhasesaber, Do(ScaleRatio(1.5f)) },
				{ ItemID.Gungnir, Do(AutoReuse, UseRatio(0.8f)) },
				{ ItemID.HornetStaff, Do(AutoReuse, UseExact(30)) },
				{ ItemID.ImpStaff, Do(AutoReuse, UseExact(30)) },
				{ ItemID.IronBroadsword, Do(AutoReuse, UseTurn) },
				{ ItemID.IronPickaxe, Do(UseTimeExact(12)) },
				{ ItemID.IronShortsword, autoReuse },
				{ ItemID.Keybrand, Do(UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.LeadBroadsword, Do(AutoReuse, UseTurn) },
				{ ItemID.LeadPickaxe, Do(UseTimeExact(12)) },
				{ ItemID.LeadShortsword, autoReuse },
				{ ItemID.LightsBane, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.MagicDagger, autoReuse },
				{ ItemID.MoltenPickaxe, Do(UseTimeExact(10)) },
				{ ItemID.MonkStaffT2, autoReuse },
				{ ItemID.MoonlordTurretStaff, Do(UseExact(15)) },
				{ ItemID.Muramasa, Do(ScaleRatio(1.5f)) },
				{ ItemID.MushroomSpear, Do(AutoReuse, UseRatio(0.8f)) },
				{ ItemID.MythrilHalberd, Do(AutoReuse, UseRatio(0.8f)) },
				{ ItemID.MythrilPickaxe, Do(UseTimeExact(8)) },
				{ ItemID.MythrilSword, Do(UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.NebulaPickaxe, Do(UseTimeExact(5)) },
				{ ItemID.NightsEdge, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.NightmarePickaxe, Do(UseTimeExact(10)) },
				{ ItemID.NorthPole, autoReuse },
				{ ItemID.ObsidianSwordfish, autoReuse },
				{ ItemID.OpticStaff, Do(AutoReuse, UseExact(25)) },
				{ ItemID.OrichalcumHalberd, Do(AutoReuse, UseRatio(0.8f)) },
				{ ItemID.OrichalcumPickaxe, Do(UseTimeExact(8)) },
				{ ItemID.OrichalcumSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f)) },
				{ ItemID.PaladinsHammer, autoReuse },
				{ ItemID.PalladiumPickaxe, Do(UseTimeExact(9)) },
				{ ItemID.PalladiumPike, Do(AutoReuse, UseRatio(0.8f)) },
				{ ItemID.PalladiumSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f)) },
				{ ItemID.PearlwoodBow, autoReuse },
				{ ItemID.PearlwoodSword, autoReuse },
				{ ItemID.PhoenixBlaster, autoReuse },
				{ ItemID.PickaxeAxe, Do(UseTimeExact(6)) },
				{ ItemID.Picksaw, Do(UseTimeExact(5)) },
				{ ItemID.PirateStaff, Do(AutoReuse, UseExact(25)) },
				{ ItemID.PlatinumBroadsword, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.PlatinumPickaxe, Do(UseTimeExact(10)) },
				{ ItemID.PlatinumShortsword, autoReuse },
				{ ItemID.PsychoKnife, useTurn },
				{ ItemID.PurpleClubberfish, Do(UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.PurplePhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.PurplePhasesaber, Do(ScaleRatio(1.5f)) },
				{ ItemID.PygmyStaff, Do(UseExact(20)) },
				{ ItemID.QueenSpiderStaff, Do(UseExact(25)) },
				{ ItemID.RainbowCrystalStaff, Do(UseExact(15)) },
				{ ItemID.RavenStaff, Do(UseExact(20)) },
				{ ItemID.RedPhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.RedPhasesaber, Do(ScaleRatio(1.5f)) },
				{ ItemID.RichMahoganySword, Do(AutoReuse, UseTurn) },
				// { ItemID.SanguineStaff, Do(AutoReuse, UseExact(25)) },
				{ ItemID.ShadewoodSword, Do(AutoReuse, UseTurn) },
				{ ItemID.SilverBroadsword, Do(AutoReuse, UseTurn) },
				{ ItemID.SilverPickaxe, Do(UseTimeExact(11)) },
				{ ItemID.SilverShortsword, autoReuse },
				{ ItemID.SlapHand, Do(UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.SlimeStaff, Do(AutoReuse, UseExact(30)) },
				// { ItemID.Smolstar, Do(AutoReuse, UseExact(25)) },
				{ ItemID.SolarFlarePickaxe, Do(UseTimeExact(5)) },
				{ ItemID.Spear, autoReuse },
				{ ItemID.SpectrePickaxe, Do(UseTimeExact(7)) },
				{ ItemID.SpiderStaff, Do(AutoReuse, UseExact(25)) },
				// { ItemID.StormSpear, autoReuse },
				// { ItemID.StormTigerStaff, UseExact(20) },
				{ ItemID.StaffoftheFrostHydra, Do(UseExact(20)) },
				{ ItemID.StardustCellStaff, Do(UseExact(20)) },
				{ ItemID.StardustPickaxe, Do(UseTimeExact(5)) },
				{ ItemID.StylistKilLaKillScissorsIWish, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.Swordfish, autoReuse },
				{ ItemID.TaxCollectorsStickOfDoom, Do(AutoReuse, UseTurn, ScaleRatio(1.5f), UseRatio(0.8f)) },
				{ ItemID.TempestStaff, Do(UseExact(20)) },
				{ ItemID.TinBroadsword, Do(AutoReuse, UseTurn) },
				{ ItemID.TinPickaxe, Do(UseTimeExact(13)) },
				{ ItemID.TinShortsword, autoReuse },
				{ ItemID.TheHorsemansBlade, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f)) },
				{ ItemID.TheRottedFork, autoReuse },
				{ ItemID.TitaniumPickaxe, Do(UseTimeExact(7)) },
				{ ItemID.TitaniumSword, Do(UseTurn, ScaleRatio(1.5f), UseRatio(0.8f)) },
				{ ItemID.TitaniumTrident, Do(AutoReuse, UseRatio(0.8f)) },
				{ ItemID.Trident, autoReuse },
				{ ItemID.TrueExcalibur, Do(AutoReuse, UseTurn) },
				{ ItemID.TrueNightsEdge, Do(AutoReuse, UseTurn) },
				{ ItemID.TungstenBroadsword, Do(AutoReuse, UseTurn) },
				{ ItemID.TungstenPickaxe, Do(UseTimeExact(11)) },
				{ ItemID.TungstenShortsword, autoReuse },
				// { ItemID.VampireFrogStaff, Do(AutoReuse, UseExact(30)) }
				{ ItemID.VenusMagnum, autoReuse },
				{ ItemID.VortexPickaxe, Do(UseTimeExact(5)) },
				{ ItemID.WhitePhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.WhitePhasesaber, Do(ScaleRatio(1.5f)) },
				{ ItemID.WoodenSword, Do(AutoReuse, UseTurn) },
				{ ItemID.XenoStaff, Do(UseExact(20)) },
				{ ItemID.YellowPhaseblade, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },
				{ ItemID.YellowPhasesaber, Do(ScaleRatio(1.5f)) },
				{ ItemID.ZombieArm, Do(AutoReuse, UseTurn, ScaleRatio(1.5f)) },

				{ ItemID.OrichalcumPickaxe, Do(UseTimeExact(8)) },
				{ ItemID.Phantasm, pointBlank },
				{ ItemID.Arkhalis, trueMelee },
				{ ItemID.Dynamite, new IBalanceRule[] { MaxStack(999) } },
				{ ItemID.GreenSolution, Do(Value(Item.buyPrice(silver: 5))) },
				{ ItemID.Bladetongue, Do(DamageExact(120), ScaleRatio(1.75f)) },
				{ ItemID.Gungnir, Do(TrueMelee, DamageExact(92), ShootSpeedRatio(1.25f)) },
				{ ItemID.UnholyTrident, Do(ManaExact(14)) },
			};
		}

		internal static void UnloadBalance()
		{
			balance.Clear();
			balance = null;
		}
		#endregion

		#region SetDefaults (Item Balance Applied Here)
		internal void SetDefaults_ApplyBalance(Item item)
		{
			switch (item.type)
			{
				// Point-blank shot weapons
				case ItemID.WoodenBow:
				case ItemID.BorealWoodBow:
				case ItemID.PalmWoodBow:
				case ItemID.RichMahoganyBow:
				case ItemID.CopperBow:
				case ItemID.TinBow:
				case ItemID.ShadewoodBow:
				case ItemID.EbonwoodBow:
				case ItemID.IronBow:
				case ItemID.LeadBow:
				case ItemID.SilverBow:
				case ItemID.TungstenBow:
				case ItemID.GoldBow:
				case ItemID.PlatinumBow:
				case ItemID.DemonBow:
				case ItemID.TendonBow:
				case ItemID.MoltenFury:
				case ItemID.BeesKnees:
				case ItemID.HellwingBow:
				case ItemID.FlareGun:
				case ItemID.Minishark:
				case ItemID.Blowpipe:
				case ItemID.FlintlockPistol:
				case ItemID.SnowballCannon:
				case ItemID.Boomstick:
				case ItemID.Revolver:
				case ItemID.RedRyder:
				case ItemID.Sandgun:
				case ItemID.Musket:
				case ItemID.TheUndertaker:
				case ItemID.Blowgun:
				//case ItemID.QuadBarrelShotgun:
				case ItemID.Handgun:
				case ItemID.PhoenixBlaster:
				case ItemID.PainterPaintballGun:
				case ItemID.Harpoon:
				case ItemID.IceBow:
				case ItemID.ShadowFlameBow:
				case ItemID.Marrow:
				case ItemID.PulseBow:
				case ItemID.DD2PhoenixBow:
				case ItemID.Tsunami:
				//case ItemID.Eventide:
				case ItemID.Phantasm:
				case ItemID.CobaltRepeater:
				case ItemID.PalladiumRepeater:
				case ItemID.MythrilRepeater:
				case ItemID.OrichalcumRepeater:
				case ItemID.AdamantiteRepeater:
				case ItemID.TitaniumRepeater:
				case ItemID.HallowedRepeater:
				case ItemID.ChlorophyteShotbow:
				case ItemID.StakeLauncher:
				case ItemID.ClockworkAssaultRifle:
				case ItemID.Gatligator:
				case ItemID.Shotgun:
				case ItemID.OnyxBlaster:
				case ItemID.Uzi:
				case ItemID.DartRifle:
				case ItemID.DartPistol:
				case ItemID.Megashark:
				case ItemID.VenusMagnum:
				case ItemID.TacticalShotgun:
				case ItemID.SniperRifle:
				case ItemID.CandyCornRifle:
				case ItemID.ChainGun:
				case ItemID.VortexBeater:
				case ItemID.SDMG:
					canFirePointBlankShots = true;
					break;

				// Set projectile true melee items to be true melee, this is so bosses know when the player is using a true melee projectile weapon
				case ItemID.Spear:
				case ItemID.Trident:
				case ItemID.PalladiumPike:
				case ItemID.CobaltDrill:
				case ItemID.MythrilDrill:
				case ItemID.AdamantiteDrill:
				case ItemID.PalladiumDrill:
				case ItemID.OrichalcumDrill:
				case ItemID.TitaniumDrill:
				case ItemID.ChlorophyteDrill:
				case ItemID.CobaltChainsaw:
				case ItemID.MythrilChainsaw:
				case ItemID.AdamantiteChainsaw:
				case ItemID.PalladiumChainsaw:
				case ItemID.OrichalcumChainsaw:
				case ItemID.TitaniumChainsaw:
				case ItemID.ChlorophyteChainsaw:
				case ItemID.VortexDrill:
				case ItemID.VortexChainsaw:
				case ItemID.NebulaDrill:
				case ItemID.NebulaChainsaw:
				case ItemID.SolarFlareDrill:
				case ItemID.SolarFlareChainsaw:
				case ItemID.StardustDrill:
				case ItemID.StardustChainsaw:
				case ItemID.Drax:
				case ItemID.ChlorophyteJackhammer:
				case ItemID.SawtoothShark:
				case ItemID.Arkhalis:
				case ItemID.ButchersChainsaw:
				case ItemID.MonkStaffT2:
					trueMelee = true;
					break;

				case ItemID.Dynamite:
				case ItemID.StickyDynamite:
				case ItemID.BouncyDynamite:
				case ItemID.StickyBomb:
				case ItemID.BouncyBomb:
					item.maxStack = 999;
					break;

				case ItemID.BlueSolution:
				case ItemID.DarkBlueSolution:
				case ItemID.GreenSolution:
				case ItemID.PurpleSolution:
				case ItemID.RedSolution:
					item.value = Item.buyPrice(0, 0, 5, 0);
					break;

				// Increase Pirate Map and Snow Globe stacks to 20
				case ItemID.PirateMap:
				case ItemID.SnowGlobe:
					item.maxStack = 20;
					break;

				// Set Celestial Sigil stack to 1 because it's not consumable anymore
				case ItemID.SlimeCrown:
				case ItemID.SuspiciousLookingEye:
				case ItemID.WormFood:
				case ItemID.BloodySpine:
				case ItemID.Abeemination:
				case ItemID.MechanicalEye:
				case ItemID.MechanicalWorm:
				case ItemID.MechanicalSkull:
				case ItemID.CelestialSigil:
					item.maxStack = 1;
					item.consumable = false;
					break;

				// True melee weapon adjustments
				case ItemID.BladedGlove:
					item.damage = 15;
					item.useTime = 7;
					item.useAnimation = 7;
					break;

				case ItemID.IceBlade:
					item.damage = 26;
					item.useTime = 33;
					break;

				case ItemID.EnchantedSword:
					item.damage = 42;
					item.useAnimation = 20;
					item.shootSpeed = 15f;
					break;

				case ItemID.Starfury:
					item.autoReuse = true;
					break;

				case ItemID.WoodYoyo:
				case ItemID.Chik:
				case ItemID.FormatC:
				case ItemID.HelFire:
				case ItemID.Amarok:
				case ItemID.Gradient:
				case ItemID.Code2:
				case ItemID.Yelets:
				case ItemID.RedsYoyo:
				case ItemID.ValkyrieYoyo:
				case ItemID.Kraken:
				case ItemID.TheEyeOfCthulhu:
					item.autoReuse = true;
					break;

				case ItemID.Rally:
					item.damage = 20;
					item.autoReuse = true;
					break;

				case ItemID.JungleYoyo:
					item.autoReuse = true;
					break;

				case ItemID.CrimsonYoyo:
					item.damage = 30;
					item.autoReuse = true;
					break;

				case ItemID.CorruptYoyo:
					item.damage = 27;
					item.autoReuse = true;
					break;

				case ItemID.Code1:
					item.damage = 25;
					item.autoReuse = true;
					break;

				case ItemID.Valor:
					item.damage = 32;
					item.autoReuse = true;
					break;

				case ItemID.ObsidianSwordfish:
					item.damage = 45;
					trueMelee = true;
					break;

				case ItemID.BloodyMachete:
					item.damage = 30;
					item.autoReuse = true;
					break;

				case ItemID.Cascade:
					item.damage = 39;
					item.autoReuse = true;
					break;

				case ItemID.SlapHand:
					item.damage = 120;
					break;

				case ItemID.TaxCollectorsStickOfDoom:
					item.damage = 70;
					break;

				case ItemID.Anchor:
					item.damage = 107;
					break;

				case ItemID.GolemFist:
					item.damage = 150;
					break;

				case ItemID.BreakerBlade:
					item.damage = 97;
					break;

				case ItemID.StylistKilLaKillScissorsIWish:
					item.damage = 33;
					break;

				case ItemID.BladeofGrass:
					item.damage = 65;
					break;

				case ItemID.FieryGreatsword:
					item.damage = 98;
					item.useTime = 45;
					item.useAnimation = 45;
					break;

				case ItemID.CobaltSword:
					item.damage = 80;
					break;

				case ItemID.MythrilSword:
					item.damage = 100;
					item.useTime = 25;
					item.useAnimation = 25;
					break;

				case ItemID.AdamantiteSword:
					item.damage = 77;
					break;

				case ItemID.PalladiumSword:
					item.damage = 100;
					break;

				case ItemID.OrichalcumSword:
					item.damage = 82;
					break;

				case ItemID.TitaniumSword:
					item.damage = 77;
					break;

				case ItemID.Excalibur:
					item.damage = 125;
					break;

				case ItemID.Bladetongue:
					item.damage = 120;
					item.scale = 1.75f;
					break;

				case ItemID.TheHorsemansBlade:
					item.damage = 95;
					break;

				case ItemID.Keybrand:
					item.damage = 184;
					item.useTime = 18;
					item.useAnimation = 18;
					break;

				case ItemID.AdamantiteGlaive:
					trueMelee = true;
					item.damage = 65;
					item.shootSpeed *= 1.25f;
					break;

				case ItemID.ChlorophytePartisan:
					item.damage = 100;
					break;

				case ItemID.CobaltNaginata:
					trueMelee = true;
					item.damage = 90;
					break;

				case ItemID.Gungnir:
					trueMelee = true;
					item.damage = 92;
					item.shootSpeed *= 1.25f;
					break;

				case ItemID.MythrilHalberd:
					trueMelee = true;
					item.damage = 95;
					item.shootSpeed *= 1.25f;
					break;

				case ItemID.OrichalcumHalberd:
					trueMelee = true;
					item.damage = 98;
					item.shootSpeed *= 1.25f;
					break;

				case ItemID.TitaniumTrident:
					trueMelee = true;
					item.damage = 72;
					item.shootSpeed *= 1.25f;
					break;

				case ItemID.DaoofPow:
					item.damage = 160;
					break;

				case ItemID.TheRottedFork:
					trueMelee = true;
					item.damage = 20;
					break;

				case ItemID.Swordfish:
					trueMelee = true;
					item.damage = 38;
					break;

				case ItemID.DarkLance:
					trueMelee = true;
					item.damage = 68;
					break;

				case ItemID.MushroomSpear:
					trueMelee = true;
					item.damage = 100;
					break;

				case ItemID.BluePhasesaber:
				case ItemID.RedPhasesaber:
				case ItemID.GreenPhasesaber:
				case ItemID.WhitePhasesaber:
				case ItemID.YellowPhasesaber:
				case ItemID.PurplePhasesaber:
					item.damage = 72;
					item.useTime = 20;
					item.useAnimation = 20;
					break;

				case ItemID.PaladinsHammer:
					item.damage = 100;
					break;

				case ItemID.Katana:
					item.useTime = 15;
					item.useAnimation = 15;
					break;

				case ItemID.FalconBlade:
					item.damage = 40;
					break;

				case ItemID.ChainKnife:
					item.damage = 14;
					break;

				case ItemID.DD2SquireDemonSword:
					item.damage = 110;
					break;

				case ItemID.PurpleClubberfish:
					item.damage = 45;
					item.knockBack = 10f;
					break;

				case ItemID.ChristmasTreeSword:
					item.damage = 155;
					break;

				case ItemID.MonkStaffT1:
					trueMelee = true;
					item.damage = 110;
					break;

				case ItemID.Terrarian:
					item.damage = 352;
					item.autoReuse = true;
					break;

				case ItemID.RainbowRod:
					item.damage = 130;
					break;

				case ItemID.BlizzardStaff:
					item.damage = 41;
					item.mana = 7;
					break;

				case ItemID.StardustDragonStaff:
					item.damage = 20;
					item.useTime = 19;
					item.useAnimation = 19;
					item.autoReuse = true;
					break;

				case ItemID.MonkStaffT3:
					item.damage = 225;
					break;

				case ItemID.BookStaff:
					item.mana = 10;
					break;

				case ItemID.UnholyTrident:
					item.mana = 14;
					break;

				case ItemID.FrostStaff:
					item.mana = 9;
					break;

				case ItemID.BookofSkulls:
					item.mana = 12;
					break;

				// Total defense pre-buff = 78, Total defense post-buff = 94
				case ItemID.SolarFlareHelmet:
					item.defense = 29; // 5 more defense
					break;

				case ItemID.SolarFlareBreastplate:
					item.defense = 41; // 7 more defense
					break;

				case ItemID.SolarFlareLeggings:
					item.defense = 24; // 4 more defense
					break;

				// Total defense pre-buff = 7, Total defense post-buff = 15
				case ItemID.GladiatorHelmet:
					item.defense = 3; // 1 more defense
					break;

				case ItemID.GladiatorBreastplate:
					item.defense = 5; // 2 more defense
					break;

				case ItemID.GladiatorLeggings:
					item.defense = 4; // 2 more defense
					break;

				// Total defense pre-buff = 31, 50, 35, Total defense post-buff = 36, 55, 40
				case ItemID.HallowedPlateMail:
					item.defense = 18; // 3 more defense
					break;

				case ItemID.HallowedGreaves:
					item.defense = 13; // 2 more defense
					break;

				// Not Expert because ML drops it in Normal so that it can be used with the lore item
				case ItemID.GravityGlobe:
					item.expert = false;
					item.rare = ItemRarityID.Red;
					break;

				case ItemID.SuspiciousLookingTentacle:
					item.expert = true;
					break;

				case ItemID.PearlwoodHammer:
					item.hammer += 35; // 80% hammer power
					item.useAnimation = 20;
					item.useTime = 15;
					item.damage *= 4;
					item.tileBoost += 1;
					item.rare = ItemRarityID.LightRed;
					break;

				case ItemID.PearlwoodBow:
					item.useAnimation += 8; // 35
					item.useTime += 8; // 35
					item.shootSpeed += 3.4f; // 10f
					item.knockBack += 1f; // 1f
					item.rare = ItemRarityID.LightRed;
					item.damage = (int)(item.damage * 2.1);
					canFirePointBlankShots = true;
					break;

				case ItemID.PearlwoodSword:
					item.damage *= 4;
					item.rare = ItemRarityID.LightRed;
					break;

				case ItemID.StarCannon:
					item.UseSound = null;
					break;

				case ItemID.EoCShield:
					CannotBeEnchanted = true;
					break;
			}

			if (CalamityLists.quadrupleDamageBuffList?.Contains(item.type) ?? false)
				item.damage *= 4;
			else if (CalamityLists.tripleDamageBuffList?.Contains(item.type) ?? false)
				item.damage *= 3;
			else if (CalamityLists.doubleDamageBuffList?.Contains(item.type) ?? false)
				item.damage *= 2;
			else if (CalamityLists.sixtySixDamageBuffList?.Contains(item.type) ?? false)
				item.damage = (int)(item.damage * 1.66);
			else if (CalamityLists.fiftyDamageBuffList?.Contains(item.type) ?? false)
				item.damage = (int)(item.damage * 1.5);
			else if (CalamityLists.thirtyThreeDamageBuffList?.Contains(item.type) ?? false)
				item.damage = (int)(item.damage * 1.33);
			else if (CalamityLists.twentyFiveDamageBuffList?.Contains(item.type) ?? false)
				item.damage = (int)(item.damage * 1.25);
			else if (CalamityLists.twentyDamageBuffList?.Contains(item.type) ?? false)
				item.damage = (int)(item.damage * 1.2);
			else if (CalamityLists.tenDamageBuffList?.Contains(item.type) ?? false)
				item.damage = (int)(item.damage * 1.1);
			else if (CalamityLists.tenDamageNerfList?.Contains(item.type) ?? false)
				item.damage = (int)(item.damage * 0.9);
			else if (CalamityLists.quarterDamageNerfList?.Contains(item.type) ?? false)
				item.damage = (int)(item.damage * 0.75);
		}
		#endregion

		#region Internal Structures

		// This function simply concatenates a bunch of Balance Rules into an array.
		// It looks a lot nicer than constantly typing "new IBalanceRule[]".
		internal static IBalanceRule[] Do(params IBalanceRule[] r) => r;

		#region Applicability Lambdas
		internal static bool DealsDamage(Item it) => it.damage > 0;
		internal static bool HasDefense(Item it) => it.defense > 0;
		internal static bool HasKnockback(Item it) => !it.accessory & !it.vanity; // how to check if something is wearable armor?
		internal static bool IsAxe(Item it) => it.axe > 0;
		internal static bool IsHammer(Item it) => it.hammer > 0;
		internal static bool IsMelee(Item it) => it.melee;
		internal static bool IsPickaxe(Item it) => it.pick > 0;
		internal static bool IsScalable(Item it) => it.damage > 0 && it.melee; // sanity check: only melee weapons get scaled
		internal static bool IsUsable(Item it) => it.useStyle != 0 && it.useTime > 0 && it.useAnimation > 0;
		internal static bool UsesMana(Item it) => IsUsable(it); // Only usable items cost mana, but items must be able to have their mana cost disabled or enabled at will.
		#endregion

		#region Balance Rules
		internal interface IBalanceRule
		{
			bool AppliesTo(Item it);
			void ApplyBalance(Item it);
		}

		internal class AutoReuseRule : IBalanceRule
		{
			internal readonly bool flag = true;

			public AutoReuseRule(bool ar) => flag = ar;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it) => it.autoReuse = flag;
		}
		internal static IBalanceRule AutoReuse => new AutoReuseRule(true);
		internal static IBalanceRule NoAutoReuse => new AutoReuseRule(false);

		// Uses the values shown by Terraria, which are multiplied by 5, not the internal values
		internal class AxePowerRule : IBalanceRule
		{
			internal readonly int newAxePower = 0;

			public AxePowerRule(int newDisplayedAxePower) => newAxePower = newDisplayedAxePower / 5;
			public bool AppliesTo(Item it) => IsAxe(it);
			public void ApplyBalance(Item it)
			{
				it.axe = newAxePower;
				if (it.axe < 0)
					it.axe = 0;
			}
		}
		internal static IBalanceRule AxePower(int a) => new AxePowerRule(a);

		internal class ConsumableRule : IBalanceRule
		{
			internal readonly bool flag = false;

			public ConsumableRule(bool c) => flag = c;
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it) => it.consumable = flag;
		}
		internal static IBalanceRule Consumable(bool c) => new ConsumableRule(c);

		#region Damage
		internal class DamageDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public DamageDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => DealsDamage(it);
			public void ApplyBalance(Item it)
			{
				it.damage += delta;
				if (it.damage < 0)
					it.damage = 0;
			}
		}
		internal static IBalanceRule DamageDelta(int d) => new DamageDeltaRule(d);

		internal class DamageExactRule : IBalanceRule
		{
			internal readonly int newDamage = 0;

			public DamageExactRule(int dmg) => newDamage = dmg;
			public bool AppliesTo(Item it) => DealsDamage(it);
			public void ApplyBalance(Item it)
			{
				it.damage = newDamage;
				if (it.damage < 0)
					it.damage = 0;
			}
		}
		internal static IBalanceRule DamageExact(int d) => new DamageExactRule(d);

		internal class DamageRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public DamageRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => DealsDamage(it);
			public void ApplyBalance(Item it)
			{
				it.damage = (int)(it.damage * ratio);
				if (it.damage < 0)
					it.damage = 0;
			}
		}
		internal static IBalanceRule DamageRatio(float f) => new DamageRatioRule(f);
		#endregion

		internal class DefenseDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public DefenseDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => HasDefense(it);
			public void ApplyBalance(Item it)
			{
				it.defense += delta;
				if (it.defense < 0)
					it.defense = 0;
			}
		}
		internal static IBalanceRule DefenseDelta(int d) => new DefenseDeltaRule(d);

		internal class DefenseExactRule : IBalanceRule
		{
			internal readonly int newDefense = 0;

			public DefenseExactRule(int def) => newDefense = def;
			public bool AppliesTo(Item it) => HasDefense(it);
			public void ApplyBalance(Item it)
			{
				it.defense = newDefense;
				if (it.defense < 0)
					it.defense = 0;
			}
		}
		internal static IBalanceRule DefenseExact(int d) => new DefenseExactRule(d);

		internal class HammerPowerRule : IBalanceRule
		{
			internal readonly int newHammerPower = 0;

			public HammerPowerRule(int h) => newHammerPower = h;
			public bool AppliesTo(Item it) => IsHammer(it);
			public void ApplyBalance(Item it)
			{
				it.hammer = newHammerPower;
				if (it.hammer < 0)
					it.hammer = 0;
			}
		}
		internal static IBalanceRule HammerPower(int h) => new HammerPowerRule(h);

		#region Knockback
		internal class KnockbackDeltaRule : IBalanceRule
		{
			internal readonly float delta = 0;

			public KnockbackDeltaRule(float d) => delta = d;
			public bool AppliesTo(Item it) => HasKnockback(it);
			public void ApplyBalance(Item it)
			{
				it.knockBack += delta;
				if (it.knockBack < 0f)
					it.knockBack = 0f;
			}
		}
		internal static IBalanceRule KnockbackDelta(float d) => new KnockbackDeltaRule(d);

		internal class KnockbackExactRule : IBalanceRule
		{
			internal readonly float newKnockback = 0;

			public KnockbackExactRule(float kb) => newKnockback = kb;
			public bool AppliesTo(Item it) => HasKnockback(it);
			public void ApplyBalance(Item it)
			{
				it.knockBack = newKnockback;
				if (it.knockBack < 0f)
					it.knockBack = 0f;
			}
		}
		internal static IBalanceRule KnockbackExact(float kb) => new KnockbackExactRule(kb);

		internal class KnockbackRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public KnockbackRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => HasKnockback(it);
			public void ApplyBalance(Item it)
			{
				it.knockBack *= ratio;
				if (it.knockBack < 0f)
					it.knockBack = 0f;
			}
		}
		internal static IBalanceRule KnockbackRatio(float r) => new KnockbackRatioRule(r);
		#endregion

		#region Mana Cost
		internal class ManaDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public ManaDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => UsesMana(it);
			public void ApplyBalance(Item it)
			{
				it.mana += delta;
				if (it.mana < 0)
					it.mana = 0;
			}
		}
		internal static IBalanceRule ManaDelta(int d) => new ManaDeltaRule(d);

		internal class ManaExactRule : IBalanceRule
		{
			internal readonly int newMana = 0;

			public ManaExactRule(int m) => newMana = m;
			public bool AppliesTo(Item it) => UsesMana(it);
			public void ApplyBalance(Item it)
			{
				it.mana = newMana;
				if (it.mana < 0)
					it.mana = 0;
			}
		}
		internal static IBalanceRule ManaExact(int m) => new ManaExactRule(m);

		internal class ManaRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public ManaRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => UsesMana(it);
			public void ApplyBalance(Item it)
			{
				it.mana = (int)(it.mana * ratio);
				if (it.mana < 0)
					it.mana = 0;
			}
		}
		internal static IBalanceRule ManaRatio(float f) => new ManaRatioRule(f);
		#endregion

		internal class MaxStackRule : IBalanceRule // max stack plus - calamity style
		{
			internal readonly int newMaxStack = 999;

			public MaxStackRule(int stk) => newMaxStack = stk;
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it)
			{
				it.maxStack = newMaxStack;
				if (it.maxStack < 1)
					it.maxStack = 1;
			}
		}
		internal static IBalanceRule MaxStack(int stk) => new MaxStackRule(stk);
		
		internal class PickPowerRule : IBalanceRule
		{
			internal readonly int newPickPower = 0;

			public PickPowerRule(int p) => newPickPower = p;
			public bool AppliesTo(Item it) => IsPickaxe(it);
			public void ApplyBalance(Item it)
			{
				it.pick = newPickPower;
				if (it.pick < 0)
					it.pick = 0;
			}
		}
		internal static IBalanceRule PickPower(int p) => new PickPowerRule(p);

		internal class PointBlankRule : IBalanceRule
		{
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it) => it.Calamity().canFirePointBlankShots = true;
		}
		internal static IBalanceRule PointBlank => new PointBlankRule();
		
		#region Scale (True Melee)
		internal class ScaleDeltaRule : IBalanceRule
		{
			internal readonly float delta = 0;

			public ScaleDeltaRule(float d) => delta = d;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.scale += delta;
				if (it.scale < 0f)
					it.scale = 0f;
			}
		}
		internal static IBalanceRule ScaleDelta(float d) => new ScaleDeltaRule(d);

		internal class ScaleExactRule : IBalanceRule
		{
			internal readonly float newScale = 0;

			public ScaleExactRule(float s) => newScale = s;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.scale = newScale;
				if (it.scale < 0f)
					it.scale = 0f;
			}
		}
		internal static IBalanceRule ScaleExact(float s) => new ScaleExactRule(s);

		internal class ScaleRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public ScaleRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.scale *= ratio;
				if (it.scale < 0f)
					it.scale = 0f;
			}
		}
		internal static IBalanceRule ScaleRatio(float f) => new ScaleRatioRule(f);
		#endregion

		#region Shoot Speed (Velocity)
		internal class ShootSpeedDeltaRule : IBalanceRule
		{
			internal readonly float delta = 0;

			public ShootSpeedDeltaRule(float d) => delta = d;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.shootSpeed += delta;
				if (it.shootSpeed < 0f)
					it.shootSpeed = 0f;
			}
		}
		internal static IBalanceRule ShootSpeedDelta(float d) => new ShootSpeedDeltaRule(d);

		internal class ShootSpeedExactRule : IBalanceRule
		{
			internal readonly float newShootSpeed = 0;

			public ShootSpeedExactRule(float ss) => newShootSpeed = ss;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.shootSpeed = newShootSpeed;
				if (it.shootSpeed < 0f)
					it.shootSpeed = 0f;
			}
		}
		internal static IBalanceRule ShootSpeedExact(float s) => new ShootSpeedExactRule(s);

		internal class ShootSpeedRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public ShootSpeedRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => IsScalable(it);
			public void ApplyBalance(Item it)
			{
				it.shootSpeed *= ratio;
				if (it.shootSpeed < 0f)
					it.shootSpeed = 0f;
			}
		}
		internal static IBalanceRule ShootSpeedRatio(float f) => new ShootSpeedRatioRule(f);
		#endregion

		internal class TileBoostDeltaRule : IBalanceRule
		{
			private readonly int delta = 0;

			public TileBoostDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it) => it.tileBoost += delta;
		}
		internal static IBalanceRule TileBoostDelta(int d) => new TileBoostDeltaRule(d);

		internal class TileBoostExactRule : IBalanceRule
		{
			private readonly int newTileBoost = 0;

			public TileBoostExactRule(int tb) => newTileBoost = tb;
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it) => it.tileBoost = newTileBoost;
		}
		internal static IBalanceRule TileBoostExact(int tb) => new TileBoostExactRule(tb);
		
		internal class TrueMeleeRule : IBalanceRule
		{
			public bool AppliesTo(Item it) => IsMelee(it);
			public void ApplyBalance(Item it) => it.Calamity().trueMelee = true;
		}
		internal static IBalanceRule TrueMelee => new TrueMeleeRule();

		#region Use Time and Use Animation
		internal class UseDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public UseDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation += delta;
				it.useTime += delta;
				if (it.useAnimation < 1)
					it.useAnimation = 1;
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseDelta(int d) => new UseDeltaRule(d);

		internal class UseExactRule : IBalanceRule
		{
			internal readonly int newUseTime = 0;

			public UseExactRule(int ut) => newUseTime = ut;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation = newUseTime;
				it.useTime = newUseTime;
				if (it.useAnimation < 1)
					it.useAnimation = 1;
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseExact(int ut) => new UseExactRule(ut);

		internal class UseRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public UseRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation = (int)(it.useAnimation * ratio);
				it.useTime = (int)(it.useTime * ratio);
				if (it.useAnimation < 1)
					it.useAnimation = 1;
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseRatio(float f) => new UseRatioRule(f);
		
		internal class UseAnimationDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public UseAnimationDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation += delta;
				if (it.useAnimation < 1)
					it.useAnimation = 1;
			}
		}
		internal static IBalanceRule UseAnimationDelta(int d) => new UseAnimationDeltaRule(d);

		internal class UseAnimationExactRule : IBalanceRule
		{
			internal readonly int newUseAnimation = 0;

			public UseAnimationExactRule(int ua) => newUseAnimation = ua;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation = newUseAnimation;
				if (it.useAnimation < 1)
					it.useAnimation = 1;
			}
		}
		internal static IBalanceRule UseAnimationExact(int ua) => new UseAnimationExactRule(ua);

		internal class UseAnimationRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public UseAnimationRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useAnimation = (int)(it.useAnimation * ratio);
				if (it.useAnimation < 1)
					it.useAnimation = 1;
			}
		}
		internal static IBalanceRule UseAnimationRatio(float f) => new UseAnimationRatioRule(f);

		internal class UseTimeDeltaRule : IBalanceRule
		{
			internal readonly int delta = 0;

			public UseTimeDeltaRule(int d) => delta = d;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useTime += delta;
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseTimeDelta(int d) => new UseTimeDeltaRule(d);

		internal class UseTimeExactRule : IBalanceRule
		{
			internal readonly int newUseTime = 0;

			public UseTimeExactRule(int ut) => newUseTime = ut;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useTime = newUseTime;
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseTimeExact(int ut) => new UseTimeExactRule(ut);

		internal class UseTimeRatioRule : IBalanceRule
		{
			internal readonly float ratio = 1f;

			public UseTimeRatioRule(float f) => ratio = f;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it)
			{
				it.useTime = (int)(it.useTime * ratio);
				if (it.useTime < 1)
					it.useTime = 1;
			}
		}
		internal static IBalanceRule UseTimeRatio(float f) => new UseTimeRatioRule(f);
		#endregion

		internal class UseTurnRule : IBalanceRule
		{
			internal readonly bool flag = true;

			public UseTurnRule(bool ut) => flag = ut;
			public bool AppliesTo(Item it) => IsUsable(it);
			public void ApplyBalance(Item it) => it.useTurn = flag;
		}
		internal static IBalanceRule UseTurn => new UseTurnRule(true);
		internal static IBalanceRule NoUseTurn => new UseTurnRule(false);

		internal class ValueRule : IBalanceRule
		{
			internal readonly int newValue = 0;

			public ValueRule(int v) => newValue = v;
			public bool AppliesTo(Item it) => true;
			public void ApplyBalance(Item it)
			{
				it.value = newValue;
				if (it.value < 0)
					it.value = 0;
			}
		}
		internal static IBalanceRule Value(int v) => new ValueRule(v);
		internal static IBalanceRule Worthless => new ValueRule(0);
		#endregion
		#endregion
	}
}
