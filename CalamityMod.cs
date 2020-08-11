using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.ILEditing;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Dyes.HairDye;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Tools;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Localization;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Projectiles.Hybrid;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Melee.Spears;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Schematics;
using CalamityMod.Skies;
using CalamityMod.TileEntities;
using CalamityMod.Tiles;
using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.LivingFire;
using CalamityMod.UI;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Dyes;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace CalamityMod
{
    public class CalamityMod : Mod
    {
        // CONSIDER -- I have been advised by Jopo that Mods should never contain static variables

        // Hotkeys
        public static ModHotKey NormalityRelocatorHotKey;
        public static ModHotKey AegisHotKey;
        public static ModHotKey TarraHotKey;
        public static ModHotKey RageHotKey;
        public static ModHotKey AdrenalineHotKey;
        public static ModHotKey AstralTeleportHotKey;
        public static ModHotKey AstralArcanumUIHotkey;
        public static ModHotKey MomentumCapacitatorHotkey;
        public static ModHotKey SandCloakHotkey;
        public static ModHotKey SpectralVeilHotKey;
        public static ModHotKey PlaguePackHotKey;

        // Boss Spawners
        public static int ghostKillCount = 0;
        public static int sharkKillCount = 0;

		// Textures & Shaders
        public static Texture2D heartOriginal2;
		public static Texture2D heartOriginal;
		public static Texture2D rainOriginal;
		public static Texture2D manaOriginal;
		public static Texture2D carpetOriginal;
		public static Texture2D AstralCactusTexture;
        public static Texture2D AstralCactusGlowTexture;
        public static Texture2D AstralSky;
        public static Effect CustomShader;
        public static Effect LightShader;
        public static Effect TentacleShader;
        public static Effect LightDistortionShader;

        // DR data structure
        public static SortedDictionary<int, float> DRValues;

		// Boss Kill Time data structure
		public static SortedDictionary<int, int> bossKillTimes;

		// Boss velocity scaling data structure
		public static SortedDictionary<int, float> bossVelocityDamageScaleValues;
		public const float velocityScaleMin = 0.5f;
		public const float bitingEnemeyVelocityScale = 0.8f;

		// Lists
		public static IList<string> donatorList;
		public static List<int> trueMeleeProjectileList; // DO NOT, EVER, DELETE THIS LIST, OR I WILL COME FOR YOU :D
        public static List<int> rangedProjectileExceptionList;
        public static List<int> projectileDestroyExceptionList;
        public static List<int> projectileMinionList;
        public static List<int> enemyImmunityList;
        public static List<int> dungeonEnemyBuffList;
        public static List<int> dungeonProjectileBuffList;
        public static List<int> bossScaleList;
        public static List<int> bossHPScaleList;
        public static List<int> beeEnemyList;
        public static List<int> beeProjectileList;
        public static List<int> friendlyBeeList;
        public static List<int> hardModeNerfList;
        public static List<int> debuffList;
        public static List<int> fireWeaponList;
        public static List<int> iceWeaponList;
        public static List<int> natureWeaponList;
        public static List<int> alcoholList;
        public static List<int> doubleDamageBuffList; //100% buff
        public static List<int> sixtySixDamageBuffList; //66% buff
        public static List<int> fiftyDamageBuffList; //50% buff
        public static List<int> thirtyThreeDamageBuffList; //33% buff
        public static List<int> twentyFiveDamageBuffList; //25% buff
        public static List<int> twentyDamageBuffList; //20% buff
        public static List<int> tenDamageBuffList; //10% buff
        public static List<int> weaponAutoreuseList;
        public static List<int> tenDamageNerfList; //10% nerf
        public static List<int> quarterDamageNerfList; //25% nerf
        public static List<int> pumpkinMoonBuffList;
        public static List<int> frostMoonBuffList;
        public static List<int> eclipseBuffList;
        public static List<int> eventProjectileBuffList;
        public static List<int> revengeanceEnemyBuffList25Percent;
		public static List<int> revengeanceEnemyBuffList20Percent;
		public static List<int> revengeanceEnemyBuffList15Percent;
		public static List<int> revengeanceEnemyBuffList10Percent;
		public static List<int> revengeanceEnemyBuffList5Percent;
		public static List<int> revengeanceProjectileBuffList25Percent;
		public static List<int> revengeanceProjectileBuffList20Percent;
		public static List<int> revengeanceProjectileBuffList15Percent;
		public static List<int> revengeanceProjectileBuffList10Percent;
		public static List<int> revengeanceProjectileBuffList5Percent;
		public static List<int> revengeanceLifeStealExceptionList;
        public static List<int> movementImpairImmuneList;
        public static List<int> needsDebuffIconDisplayList;
        public static List<int> trapProjectileList;
        public static List<int> scopedWeaponList;
        public static List<int> boomerangList;
        public static List<int> javelinList;
        public static List<int> daggerList;
        public static List<int> flaskBombList;
        public static List<int> spikyBallList;
        public static List<int> boomerangProjList;
        public static List<int> javelinProjList;
        public static List<int> daggerProjList;
        public static List<int> flaskBombProjList;
        public static List<int> spikyBallProjList;
        public static List<int> noGravityList;
        public static List<int> lavaFishList;
        public static List<int> highTestFishList;
        public static List<int> flamethrowerList;
        public static List<int> forceItemList;
        public static List<int> livingFireBlockList;

        public static List<int> zombieList;
        public static List<int> demonEyeList;
        public static List<int> skeletonList;
        public static List<int> angryBonesList;
        public static List<int> hornetList;
        public static List<int> mossHornetList;
        public static List<int> bossMinionList;

        public static List<int> legOverrideList;

        // TODO -- Calamity should check for other mods existing in exactly one place
        internal Mod thorium = null;
        public bool fargosMutant = false;

		internal static CalamityMod Instance;

        #region Load
        public override void Load()
        {
			Instance = this;
			heartOriginal2 = Main.heartTexture;
			heartOriginal = Main.heart2Texture;
			rainOriginal = Main.rainTexture;
			manaOriginal = Main.manaTexture;
			carpetOriginal = Main.flyingCarpetTexture;

			NormalityRelocatorHotKey = RegisterHotKey("Normality Relocator", "Z");
            RageHotKey = RegisterHotKey("Rage Mode", "V");
            AdrenalineHotKey = RegisterHotKey("Adrenaline Mode", "B");
            AegisHotKey = RegisterHotKey("Elysian Guard", "N");
            TarraHotKey = RegisterHotKey("Armor Set Bonus", "Y");
            AstralTeleportHotKey = RegisterHotKey("Astral Teleport", "P");
            AstralArcanumUIHotkey = RegisterHotKey("Astral Arcanum UI Toggle", "O");
            MomentumCapacitatorHotkey = RegisterHotKey("Momentom Capacitater Effect", "U");
            SandCloakHotkey = RegisterHotKey("Sand Cloak Effect", "C");
            SpectralVeilHotKey = RegisterHotKey("Spectral Veil Teleport", "Z");
            PlaguePackHotKey = RegisterHotKey("Booster Dash", "Q");

            if (!Main.dedServ)
            {
                LoadClient();
            }

            ILChanges.Initialize();
            thorium = ModLoader.GetMod("ThoriumMod");

            BossHealthBarManager.Load(this);

            SetupLists();
            SetupVanillaDR();
			SetupBossKillTimes();
			SetupBossVelocityScalingValues();
            SetupThoriumBossDR(thorium);

            CalamityLocalization.AddLocalizations();

			On.Terraria.Player.TileInteractionsUse += Player_TileInteractionsUse;
        }

		private static void Player_TileInteractionsUse(On.Terraria.Player.orig_TileInteractionsUse orig, Player player, int i, int j)
		{
			Tile tile = Main.tile[i, j];
			if (tile.type == ModContent.TileType<AgedLaboratoryDoorOpen>())
			{
				DoorSwap(ModContent.TileType<AgedLaboratoryDoorClosed>(), ModContent.TileType<AgedLaboratoryDoorOpen>(), i, j);
			}
			else if (tile.type == ModContent.TileType<AgedLaboratoryDoorClosed>())
			{
				DoorSwap(ModContent.TileType<AgedLaboratoryDoorOpen>(), ModContent.TileType<AgedLaboratoryDoorClosed>(), i, j);
			}
			else if (tile.type == ModContent.TileType<LaboratoryDoorOpen>())
			{
				DoorSwap(ModContent.TileType<LaboratoryDoorClosed>(), ModContent.TileType<LaboratoryDoorOpen>(), i, j);
			}
			else if (tile.type == ModContent.TileType<LaboratoryDoorClosed>())
			{
				DoorSwap(ModContent.TileType<LaboratoryDoorOpen>(), ModContent.TileType<LaboratoryDoorClosed>(), i, j);
			}
			else
			{
				orig(player, i, j);
			}
		}

		private static void DoorSwap(int type1, int type2, int i, int j)
		{
			if (PlayerInput.Triggers.JustPressed.MouseRight)
			{
				ushort type = (ushort)type1;
				short frameY = 0;
				for (int dy = -4; dy < 4; dy++)
				{
					if (Main.tile[i, j + dy].frameY > 0 && frameY == 0)
						continue;
					if (Main.tile[i, j + dy].type == type2)
					{
						if (Main.tile[i, j + dy] is null)
						{
							Main.tile[i, j + dy] = new Tile();
						}
						Main.tile[i, j + dy].type = type;
						Main.tile[i, j + dy].frameY = frameY;
						frameY += 16;
						if ((int)frameY / 16 >= 4)
							break;
					}
				}

				Main.PlaySound(SoundID.DoorClosed, i * 16, j * 16, 1, 1f, 0f);
			}
		}

        private void LoadClient()
        {
            AddEquipTexture(new AbyssalDivingSuitHead(), null, EquipType.Head, "AbyssalDivingSuitHead", "CalamityMod/Items/Accessories/AbyssalDivingSuit_Head");
            AddEquipTexture(new AbyssalDivingSuitBody(), null, EquipType.Body, "AbyssalDivingSuitBody", "CalamityMod/Items/Accessories/AbyssalDivingSuit_Body", "CalamityMod/Items/Accessories/AbyssalDivingSuit_Arms");
            AddEquipTexture(new AbyssalDivingSuitLegs(), null, EquipType.Legs, "AbyssalDivingSuitLeg", "CalamityMod/Items/Accessories/AbyssalDivingSuit_Legs");

            AddEquipTexture(new SirenHead(), null, EquipType.Head, "SirenHead", "CalamityMod/Items/Accessories/SirenTrans_Head");
            AddEquipTexture(new SirenBody(), null, EquipType.Body, "SirenBody", "CalamityMod/Items/Accessories/SirenTrans_Body", "CalamityMod/Items/Accessories/SirenTrans_Arms");
            AddEquipTexture(new SirenLegs(), null, EquipType.Legs, "SirenLeg", "CalamityMod/Items/Accessories/SirenTrans_Legs");

            AddEquipTexture(new AndromedaHead(), null, EquipType.Head, "NoHead", "CalamityMod/ExtraTextures/AndromedaWithout_Head");

            AddEquipTexture(new PopoHead(), null, EquipType.Head, "PopoHead", "CalamityMod/Items/Accessories/Vanity/Popo_Head");
            AddEquipTexture(new PopoNoselessHead(), null, EquipType.Head, "PopoNoselessHead", "CalamityMod/Items/Accessories/Vanity/PopoNoseless_Head");
            AddEquipTexture(new PopoBody(), null, EquipType.Body, "PopoBody", "CalamityMod/Items/Accessories/Vanity/Popo_Body", "CalamityMod/Items/Accessories/Vanity/Popo_Arms");
            AddEquipTexture(new PopoLegs(), null, EquipType.Legs, "PopoLeg", "CalamityMod/Items/Accessories/Vanity/Popo_Legs");

            AddEquipTexture(new ProfanedCrystalHead(), null, EquipType.Head, "ProviHead", "CalamityMod/Items/Accessories/ProfanedSoulTransHead");
            AddEquipTexture(new ProfanedCrystalBody(), null, EquipType.Body, "ProviBody", "CalamityMod/Items/Accessories/ProfanedSoulTransBody", "CalamityMod/Items/Accessories/ProfanedSoulTransArms");
            AddEquipTexture(new ProfanedCrystalLegs(), null, EquipType.Legs, "ProviLegs", "CalamityMod/Items/Accessories/ProfanedSoulTransLegs");
            AddEquipTexture(new ProfanedCrystalWings(), null, EquipType.Wings, "ProviWings", "CalamityMod/Items/Accessories/Wings/ProfanedSoulTransWings");

            AddEquipTexture(new SnowRuffianWings(), null, EquipType.Wings, "SnowRuffWings", "CalamityMod/Items/Armor/SnowRuffianWings");

            AddEquipTexture(new MeldTransformationHead(), null, EquipType.Head, "MeldTransformationHead", "CalamityMod/Items/Armor/MeldTransformation_Head");
            AddEquipTexture(new MeldTransformationBody(), null, EquipType.Body, "MeldTransformationBody", "CalamityMod/Items/Armor/MeldTransformation_Body", "CalamityMod/Items/Armor/MeldTransformation_Arms");
            AddEquipTexture(new MeldTransformationLegs(), null, EquipType.Legs, "MeldTransformationLegs", "CalamityMod/Items/Armor/MeldTransformation_Legs");

            AddEquipTexture(new AbyssDivingGearHair(), null, EquipType.Head, "AbyssDivingGearHead", "CalamityMod/Items/Accessories/AbyssalDivingGear_Face");

            AstralCactusTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Tiles/AstralCactus");
            AstralCactusGlowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Tiles/AstralCactusGlow");
            AstralSky = ModContent.GetTexture("CalamityMod/ExtraTextures/AstralSky");
            CustomShader = GetEffect("Effects/CustomShader");
            LightShader = GetEffect("Effects/LightBurstShader");
            TentacleShader = GetEffect("Effects/TentacleShader");
            LightDistortionShader = GetEffect("Effects/DistortionShader");

            Filters.Scene["CalamityMod:DevourerofGodsHead"] = new Filter(new DoGScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.1f, 1.0f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:DevourerofGodsHead"] = new DoGSky();

            Filters.Scene["CalamityMod:DevourerofGodsHeadS"] = new Filter(new DoGScreenShaderDataS("FilterMiniTower").UseColor(0.4f, 0.1f, 1.0f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:DevourerofGodsHeadS"] = new DoGSkyS();

            Filters.Scene["CalamityMod:CalamitasRun3"] = new Filter(new CalScreenShaderData("FilterMiniTower").UseColor(1.1f, 0.3f, 0.3f).UseOpacity(0.6f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:CalamitasRun3"] = new CalSky();

            Filters.Scene["CalamityMod:PlaguebringerGoliath"] = new Filter(new PbGScreenShaderData("FilterMiniTower").UseColor(0.2f, 0.6f, 0.2f).UseOpacity(0.35f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:PlaguebringerGoliath"] = new PbGSky();

            Filters.Scene["CalamityMod:Yharon"] = new Filter(new YScreenShaderData("FilterMiniTower").UseColor(1f, 0.4f, 0f).UseOpacity(0.75f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:Yharon"] = new YSky();

            Filters.Scene["CalamityMod:Leviathan"] = new Filter(new LevScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0.5f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:Leviathan"] = new LevSky();

            Filters.Scene["CalamityMod:Providence"] = new Filter(new ProvScreenShaderData("FilterMiniTower").UseColor(0.45f, 0.4f, 0.2f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:Providence"] = new ProvSky();

            Filters.Scene["CalamityMod:SupremeCalamitas"] = new Filter(new SCalScreenShaderData("FilterMiniTower").UseColor(1.1f, 0.3f, 0.3f).UseOpacity(0.65f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:SupremeCalamitas"] = new SCalSky();

			Filters.Scene["CalamityMod:Signus"] = new Filter(new SignusScreenShaderData("FilterMiniTower").UseColor(0.35f, 0.1f, 0.55f).UseOpacity(0.35f), EffectPriority.VeryHigh);
			SkyManager.Instance["CalamityMod:Signus"] = new SignusSky();

            Filters.Scene["CalamityMod:Astral"] = new Filter(new AstralScreenShaderData(new Ref<Effect>(CustomShader), "AstralPass").UseColor(0.18f, 0.08f, 0.24f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:Astral"] = new AstralSky();

            Filters.Scene["CalamityMod:LightBurst"] = new Filter(new ScreenShaderData(new Ref<Effect>(LightShader), "BurstPass"), EffectPriority.VeryHigh);
            Filters.Scene["CalamityMod:LightBurst"].Load();

            GameShaders.Misc["CalamityMod:SubsumingTentacle"] = new MiscShaderData(new Ref<Effect>(TentacleShader), "BurstPass");
            GameShaders.Misc["CalamityMod:LightDistortion"] = new MiscShaderData(new Ref<Effect>(LightDistortionShader), "DistortionPass");

            SkyManager.Instance["CalamityMod:Cryogen"] = new CryogenSky();

            RipperUI.Reset();
            AstralArcanumUI.Load(this);

			GameShaders.Hair.BindShader(ModContent.ItemType<AdrenalineHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(0, 255, 171), ((float)player.Calamity().adrenaline / (float)player.Calamity().adrenalineMax))));
			GameShaders.Hair.BindShader(ModContent.ItemType<RageHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(255, 83, 48), ((float)player.Calamity().rage / (float)player.Calamity().rageMax))));
			GameShaders.Hair.BindShader(ModContent.ItemType<WingTimeHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(139, 205, 255), ((float)player.wingTime / (float)player.wingTimeMax))));
			GameShaders.Hair.BindShader(ModContent.ItemType<StealthHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(186, 85, 211), (player.Calamity().rogueStealth / player.Calamity().rogueStealthMax))));

            SchematicLoader.LoadEverything();

            PopupGUIManager.LoadGUIs();
        }
        #endregion

        #region Unload
        public override void Unload()
        {
            NormalityRelocatorHotKey = null;
            RageHotKey = null;
            AdrenalineHotKey = null;
            AegisHotKey = null;
            TarraHotKey = null;
            AstralTeleportHotKey = null;
            AstralArcanumUIHotkey = null;
            MomentumCapacitatorHotkey = null;
            SandCloakHotkey = null;
            SpectralVeilHotKey = null;
            PlaguePackHotKey = null;

            AstralCactusTexture = null;
            AstralCactusGlowTexture = null;
            AstralSky = null;

            DRValues?.Clear();
            DRValues = null;
			bossKillTimes?.Clear();
			bossKillTimes = null;
			bossVelocityDamageScaleValues?.Clear();
			bossVelocityDamageScaleValues = null;

            donatorList = null;
			trueMeleeProjectileList = null;
            rangedProjectileExceptionList = null;
            projectileDestroyExceptionList = null;
            projectileMinionList = null;
            enemyImmunityList = null;
            dungeonEnemyBuffList = null;
            dungeonProjectileBuffList = null;
            bossScaleList = null;
            bossHPScaleList = null;
            beeEnemyList = null;
            friendlyBeeList = null;
            beeProjectileList = null;
            hardModeNerfList = null;
            debuffList = null;
            fireWeaponList = null;
            iceWeaponList = null;
            natureWeaponList = null;
            alcoholList = null;
            doubleDamageBuffList = null;
            sixtySixDamageBuffList = null;
            fiftyDamageBuffList = null;
            thirtyThreeDamageBuffList = null;
            twentyFiveDamageBuffList = null;
            tenDamageBuffList = null;
            weaponAutoreuseList = null;
            tenDamageNerfList = null;
            quarterDamageNerfList = null;
            pumpkinMoonBuffList = null;
            frostMoonBuffList = null;
            eclipseBuffList = null;
            eventProjectileBuffList = null;
			revengeanceEnemyBuffList25Percent = null;
			revengeanceEnemyBuffList20Percent = null;
			revengeanceEnemyBuffList15Percent = null;
			revengeanceEnemyBuffList10Percent = null;
			revengeanceEnemyBuffList5Percent = null;
			revengeanceProjectileBuffList25Percent = null;
			revengeanceProjectileBuffList20Percent = null;
			revengeanceProjectileBuffList15Percent = null;
			revengeanceProjectileBuffList10Percent = null;
			revengeanceProjectileBuffList5Percent = null;
			revengeanceLifeStealExceptionList = null;
            movementImpairImmuneList = null;
            needsDebuffIconDisplayList = null;
            trapProjectileList = null;
            scopedWeaponList = null;
            boomerangList = null;
            javelinList = null;
            daggerList = null;
            flaskBombList = null;
            spikyBallList = null;
            boomerangProjList = null;
            javelinProjList = null;
            daggerProjList = null;
            flaskBombProjList = null;
            spikyBallProjList = null;
            noGravityList = null;
            lavaFishList = null;
            highTestFishList = null;
            flamethrowerList = null;
            forceItemList = null;
            livingFireBlockList = null;

            zombieList = null;
            demonEyeList = null;
            skeletonList = null;
            angryBonesList = null;
            hornetList = null;
            mossHornetList = null;
            bossMinionList = null;

            legOverrideList = null;

            thorium = null;
            fargosMutant = false;

			Instance = null;

            PopupGUIManager.UnloadGUIs();
            SchematicLoader.UnloadEverything();
            BossHealthBarManager.Unload();
            base.Unload();

            TileFraming.Unload();

            RipperUI.Reset();
            AstralArcanumUI.Unload();
            base.Unload();

			if (!Main.dedServ)
			{
				Main.heartTexture = heartOriginal2;
				Main.heart2Texture = heartOriginal;
				Main.rainTexture = rainOriginal;
				Main.manaTexture = manaOriginal;
				Main.flyingCarpetTexture = carpetOriginal;
			}

			heartOriginal2 = null;
			heartOriginal = null;
			rainOriginal = null;
			manaOriginal = null;
			carpetOriginal = null;
        }
        #endregion

        #region Late Loading
        public override void PostAddRecipes()
        {
            // This is placed here so that all tiles from all mods are guaranteed to be loaded at this point.
            TileFraming.Load();
        }
        #endregion

        #region ConfigCrap
        internal static void SaveConfig(CalamityConfig cfg)
		{
			// in-game ModConfig saving from mod code is not supported yet in tmodloader, and subject to change, so we need to be extra careful.
			// This code only supports client configs, and doesn't call onchanged. It also doesn't support ReloadRequired or anything else.
			MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
			if (saveMethodInfo != null)
				saveMethodInfo.Invoke(null, new object[] { cfg });
			else
				Instance.Logger.Warn("In-game SaveConfig failed, code update required");
		}
		#endregion

        #region SetupLists
        public static void SetupLists()
        {
            donatorList = new List<string>()
            {
                "Vorbis",
                "SoloMael",
                "Chaotic Reks",
                "The Buildmonger",
                "Yuh",
                "Littlepiggy",
                "LompL",
                "Lilith Saintclaire",
                "Ben Shapiro",
                "Frederik Henschel",
                "Faye",
                "Gibb50",
                "Braden Hajer",
                "Hannes Holmlund",
                "profoundmango69",
                "Jack M Sargent",
                "Hans Volter",
                "Krankwagon",
                "MishiroUsui",
                "pixlgray",
                "Arkhine",
                "Lodude",
                "DevAesthetic",
                "Mister Winchester",
                "Zacky",
                "Veine",
                "Javyz",
                "Shifter",
                "Crysthamyr",
                "Elfinlocks",
                "Ein",
                "2Larry2",
                "Jenonen",
                "Dodu",
                "Arti",
                "Tervastator",
                "Luis Arguello",
                "Alexander Davis",
                "BakaQing",
                "Laura Coonrod",
                "Xaphlactus",
                "MajinBagel",
                "Bendy",
                "Rando Calrissian",
                "Tails the Fox 92",
                "Bread",
                "Minty Candy",
                "Preston Card",
                "MovingTarget_086",
                "Shiro",
                "Chip",
                "Taylor Riverpaw",
                "ShotgunAngel",
                "Sandblast",
                "ThomasThePencil",
                "Aero (Aero#4599)",
                "GlitchOut",
                "Daawnz",
                "CrabBar",
                "Yatagarasu",
                "Jarod Isaac Gordon",
                "Zombieh",
                "MingWhy",
                "Random Weeb",
                "Ahmed Fahad Zamel Al Sharif",
                "Eragon3942",
                "TheBlackHand",
                "william",
                "Samuel Foreman",
                "Christopher Pham",
                "DemoN K!ng",
                "Malik Ciaramella",
                "Ryan Baker-Ortiz",
                "Aleksanders Denisovs",
                "TheSilverGhost",
                "Lucazii",
                "Shay",
                "Prism",
                "BobIsNotMyRealName",
                "Guwahavel",
                "Azura",
                "Joshua Miranda",
                "Doveda",
                "William Chang",
                "Arche",
                "DevilSunrise",
                "Yanmei",
                "Chaos",
                "Ryan Tucker",
                "Fish Repairs",
                "Melvin Brouwers",
                "Vroomy Has -3,000 IQ",
                "The Goliath",
                "DaPyRo",
                "Termi",
				"Circuit-Jay",
				"Commmander Frostbite",
				"cytokat",
				"Cameron Fowlks",
				"Orudeon",
				"BumbleDoge",
				"John Ballard",
				"Naglfar",
				"Helixas",
				"Vetus Dea",
				"High Charity",
				"Devonte Plati",
				"Cerberus",
				"Brendan Kendall",
				"Victor Pittman",
				"KAT-G307",
				"Tombarry Expresserino",
				"Drip Veezy",
				"Glaid",
				"Apotheosis",
				"Bladesaber",
				"Devon Leigh",
				"Ruthoranium",
				"cocodezi_",
				"Mendzey",
				"GameRDheAsianSandwich",
				"Tobias",
				"Streakist .",
				"Eisaya A Cook",
				"Xenocrona",
				"RKMoon"
			};

			trueMeleeProjectileList = new List<int>()
			{
				// Vanilla shit
				ProjectileID.Spear,
                ProjectileID.Trident,
                ProjectileID.TheRottedFork,
                ProjectileID.Swordfish,
                ProjectileID.Arkhalis,
                ProjectileID.DarkLance,
                ProjectileID.CobaltNaginata,
                ProjectileID.PalladiumPike,
                ProjectileID.MythrilHalberd,
                ProjectileID.OrichalcumHalberd,
                ProjectileID.AdamantiteGlaive,
                ProjectileID.TitaniumTrident,
                ProjectileID.MushroomSpear,
                ProjectileID.Gungnir,
                ProjectileID.ObsidianSwordfish,
                ProjectileID.ChlorophytePartisan,
                ProjectileID.MonkStaffT1,
                ProjectileID.MonkStaffT2,
                ProjectileID.MonkStaffT3,
                ProjectileID.NorthPoleWeapon,

				// Tools
                ProjectileID.CobaltDrill,
                ProjectileID.MythrilDrill,
                ProjectileID.AdamantiteDrill,
                ProjectileID.PalladiumDrill,
                ProjectileID.OrichalcumDrill,
                ProjectileID.TitaniumDrill,
                ProjectileID.ChlorophyteDrill,
                ProjectileID.CobaltChainsaw,
                ProjectileID.MythrilChainsaw,
                ProjectileID.AdamantiteChainsaw,
                ProjectileID.PalladiumChainsaw,
                ProjectileID.OrichalcumChainsaw,
                ProjectileID.TitaniumChainsaw,
                ProjectileID.ChlorophyteChainsaw,
                ProjectileID.VortexDrill,
                ProjectileID.VortexChainsaw,
                ProjectileID.NebulaDrill,
                ProjectileID.NebulaChainsaw,
                ProjectileID.SolarFlareDrill,
                ProjectileID.SolarFlareChainsaw,
                ProjectileID.StardustDrill,
                ProjectileID.StardustChainsaw,
                ProjectileID.Hamdrax,
                ProjectileID.ChlorophyteJackhammer,
                ProjectileID.SawtoothShark,
                ProjectileID.ButchersChainsaw,

				// Calamity shit
				ModContent.ProjectileType<DevilsSunriseProj>(),
				ModContent.ProjectileType<MarniteObliteratorProj>(),
				ModContent.ProjectileType<MurasamaSlash>(),
				ModContent.ProjectileType<AmidiasTridentProj>(),
				ModContent.ProjectileType<AstralPikeProj>(),
				ModContent.ProjectileType<BansheeHookProj>(),
				ModContent.ProjectileType<BrimlanceProj>(),
				ModContent.ProjectileType<DiseasedPikeSpear>(),
				ModContent.ProjectileType<EarthenPikeSpear>(),
				ModContent.ProjectileType<ExsanguinationLanceProjectile>(),
				ModContent.ProjectileType<FulgurationHalberdProj>(),
				ModContent.ProjectileType<GildedProboscisProj>(),
				ModContent.ProjectileType<GoldplumeSpearProjectile>(),
				ModContent.ProjectileType<HellionFlowerSpearProjectile>(),
				ModContent.ProjectileType<InsidiousImpalerProj>(),
				ModContent.ProjectileType<MarniteSpearProjectile>(),
				ModContent.ProjectileType<NadirSpear>(),
				ModContent.ProjectileType<SausageMakerSpear>(),
				ModContent.ProjectileType<SpatialLanceProjectile>(),
				ModContent.ProjectileType<StarnightLanceProjectile>(),
				ModContent.ProjectileType<StreamGougeProj>(),
				ModContent.ProjectileType<TenebreusTidesProjectile>(),
				ModContent.ProjectileType<TerraLanceProjectile>(),
				ModContent.ProjectileType<TyphonsGreedStaff>(),
				ModContent.ProjectileType<UrchinSpearProjectile>(),
				ModContent.ProjectileType<YateveoBloomSpear>()
			};

			rangedProjectileExceptionList = new List<int>()
            {
                ProjectileID.Phantasm,
                ProjectileID.VortexBeater,
                ProjectileID.DD2PhoenixBow,
                ProjectileID.IchorDart,
                ProjectileID.PhantasmArrow,
                ProjectileID.RainbowBack,
                ModContent.ProjectileType<PhangasmBow>(),
                ModContent.ProjectileType<ContagionBow>(),
                ModContent.ProjectileType<DaemonsFlameBow>(),
                ModContent.ProjectileType<ExoTornado>(),
                ModContent.ProjectileType<DrataliornusBow>(),
                ModContent.ProjectileType<FlakKrakenGun>(),
                ModContent.ProjectileType<ButcherGun>(),
                ModContent.ProjectileType<StarfleetMK2Gun>(),
                ModContent.ProjectileType<TerraBulletSplit>(),
                ModContent.ProjectileType<TerraArrowSplit>(),
                ModContent.ProjectileType<HyperiusSplit>(),
                ModContent.ProjectileType<NorfleetCannon>(),
                ModContent.ProjectileType<NorfleetComet>(),
                ModContent.ProjectileType<NorfleetExplosion>(),
                ModContent.ProjectileType<AetherBeam>(),
                ModContent.ProjectileType<FlurrystormCannonShooting>(),
                ModContent.ProjectileType<MagnomalyBeam>(),
                ModContent.ProjectileType<MagnomalyAura>(),
                ModContent.ProjectileType<RainbowTrail>(),
                ModContent.ProjectileType<PrismaticBeam>(),
                ModContent.ProjectileType<ExoLight>(),
                ModContent.ProjectileType<ExoLightBomb>(),
                ModContent.ProjectileType<UltimaBowProjectile>(),
                ModContent.ProjectileType<UltimaSpark>(), // Because of potential dust lag.
                ModContent.ProjectileType<UltimaRay>()
            };

            projectileDestroyExceptionList = new List<int>()
            {
				//holdout projectiles
                ProjectileID.Phantasm,
                ProjectileID.VortexBeater,
                ProjectileID.DD2PhoenixBow,
                ProjectileID.LastPrism,
                ProjectileID.LaserMachinegun,
                ProjectileID.ChargedBlasterCannon,
				ProjectileID.MedusaHead,
                ModContent.ProjectileType<PhangasmBow>(),
                ModContent.ProjectileType<ContagionBow>(),
                ModContent.ProjectileType<DaemonsFlameBow>(),
                ModContent.ProjectileType<DrataliornusBow>(),
                ModContent.ProjectileType<FlakKrakenGun>(),
                ModContent.ProjectileType<ButcherGun>(),
                ModContent.ProjectileType<StarfleetMK2Gun>(),
                ModContent.ProjectileType<NorfleetCannon>(),
                ModContent.ProjectileType<FlurrystormCannonShooting>(),
                ModContent.ProjectileType<PurgeProj>(),
                ModContent.ProjectileType<T1000Proj>(),
                ModContent.ProjectileType<YharimsCrystalPrism>(),
                ModContent.ProjectileType<DarkSparkPrism>(),
                ModContent.ProjectileType<GhastlyVisageProj>(),

                ModContent.ProjectileType<FlakKrakenProj>(),
                ModContent.ProjectileType<SylvanSlashAttack>(),
                ModContent.ProjectileType<InfernadoFriendly>(),
				ModContent.ProjectileType<MurasamaSlash>(),
                ModContent.ProjectileType<PhaseslayerProjectile>(),

				//Some hostile boss projectiles
                ModContent.ProjectileType<BrimstoneMonster>(),
                ModContent.ProjectileType<InfernadoRevenge>(),
				ModContent.ProjectileType<OverlyDramaticDukeSummoner>(),
				ModContent.ProjectileType<ProvidenceHolyRay>(),
				ModContent.ProjectileType<OldDukeVortex>(),
				ModContent.ProjectileType<BrimstoneRay>(),
				ModContent.ProjectileType<BrimstoneTargetRay>()
            };

            projectileMinionList = new List<int>()
            {
                ProjectileID.PygmySpear,
                ProjectileID.UFOMinion,
                ProjectileID.UFOLaser,
                ProjectileID.StardustCellMinionShot,
                ProjectileID.MiniSharkron,
                ProjectileID.MiniRetinaLaser,
                ProjectileID.ImpFireball,
                ProjectileID.HornetStinger,
                ProjectileID.DD2FlameBurstTowerT1Shot,
                ProjectileID.DD2FlameBurstTowerT2Shot,
                ProjectileID.DD2FlameBurstTowerT3Shot,
                ProjectileID.DD2BallistraProj,
                ProjectileID.DD2ExplosiveTrapT1Explosion,
                ProjectileID.DD2ExplosiveTrapT2Explosion,
                ProjectileID.DD2ExplosiveTrapT3Explosion,
                ProjectileID.SpiderEgg,
                ProjectileID.BabySpider,
                ProjectileID.FrostBlastFriendly,
                ProjectileID.MoonlordTurretLaser,
                ProjectileID.RainbowCrystalExplosion
            };

            enemyImmunityList = new List<int>()
            {
                NPCID.KingSlime,
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.BrainofCthulhu,
                NPCID.Creeper,
                NPCID.EyeofCthulhu,
                NPCID.QueenBee,
                NPCID.SkeletronHead,
                NPCID.SkeletronHand,
                NPCID.WallofFlesh,
                NPCID.WallofFleshEye,
                NPCID.Retinazer,
                NPCID.Spazmatism,
                NPCID.SkeletronPrime,
                NPCID.PrimeCannon,
                NPCID.PrimeSaw,
                NPCID.PrimeLaser,
                NPCID.PrimeVice,
                NPCID.Plantera,
                NPCID.IceQueen,
                NPCID.Pumpking,
                NPCID.Mothron,
                NPCID.Golem,
                NPCID.GolemHead,
				NPCID.GolemHeadFree,
				NPCID.GolemFistRight,
                NPCID.GolemFistLeft,
                NPCID.GolemHeadFree,
                NPCID.DukeFishron,
                NPCID.Sharkron,
                NPCID.Sharkron2,
                NPCID.CultistBoss,
                NPCID.MoonLordHead,
                NPCID.MoonLordHand,
                NPCID.MoonLordCore,
                NPCID.MoonLordFreeEye,
                NPCID.DD2WyvernT1,
                NPCID.DD2WyvernT2,
                NPCID.DD2WyvernT3,
                NPCID.DD2DarkMageT1,
                NPCID.DD2DarkMageT3,
                NPCID.DD2SkeletonT1,
                NPCID.DD2SkeletonT3,
                NPCID.DD2WitherBeastT2,
                NPCID.DD2WitherBeastT3,
                NPCID.DD2DrakinT2,
                NPCID.DD2DrakinT3,
                NPCID.DD2KoboldWalkerT2,
                NPCID.DD2KoboldWalkerT3,
                NPCID.DD2KoboldFlyerT2,
                NPCID.DD2KoboldFlyerT3,
                NPCID.DD2OgreT2,
                NPCID.DD2OgreT3,
                NPCID.DD2Betsy,
				ModContent.NPCType<DesertScourgeHeadSmall>(),
				ModContent.NPCType<DesertScourgeBodySmall>(),
				ModContent.NPCType<DesertScourgeTailSmall>(),
				ModContent.NPCType<GiantClam>(),
				ModContent.NPCType<PerforatorHeadLarge>(),
				ModContent.NPCType<PerforatorHeadMedium>(),
				ModContent.NPCType<PerforatorHeadSmall>(),
				ModContent.NPCType<PerforatorBodyLarge>(),
				ModContent.NPCType<PerforatorBodyMedium>(),
				ModContent.NPCType<PerforatorBodySmall>(),
				ModContent.NPCType<PerforatorTailLarge>(),
				ModContent.NPCType<PerforatorTailMedium>(),
				ModContent.NPCType<PerforatorTailSmall>(),
				ModContent.NPCType<SlimeGod>(),
				ModContent.NPCType<SlimeGodRun>(),
				ModContent.NPCType<SlimeGodSplit>(),
				ModContent.NPCType<SlimeGodRunSplit>(),
                ModContent.NPCType<CalamitasRun>(),
				ModContent.NPCType<CalamitasRun2>() //brothers
            };

            dungeonEnemyBuffList = new List<int>()
            {
                NPCID.SkeletonSniper,
                NPCID.TacticalSkeleton,
                NPCID.SkeletonCommando,
                NPCID.Paladin,
                NPCID.GiantCursedSkull,
                NPCID.BoneLee,
                NPCID.DiabolistWhite,
                NPCID.DiabolistRed,
                NPCID.NecromancerArmored,
                NPCID.Necromancer,
                NPCID.RaggedCasterOpenCoat,
                NPCID.RaggedCaster,
                NPCID.HellArmoredBonesSword,
                NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBones,
                NPCID.BlueArmoredBonesSword,
                NPCID.BlueArmoredBonesNoPants,
                NPCID.BlueArmoredBonesMace,
                NPCID.BlueArmoredBones,
                NPCID.RustyArmoredBonesSwordNoArmor,
                NPCID.RustyArmoredBonesSword,
                NPCID.RustyArmoredBonesFlail,
                NPCID.RustyArmoredBonesAxe
            };

            dungeonProjectileBuffList = new List<int>()
            {
                ProjectileID.PaladinsHammerHostile,
                ProjectileID.ShadowBeamHostile,
                ProjectileID.InfernoHostileBolt,
                ProjectileID.InfernoHostileBlast,
                ProjectileID.LostSoulHostile,
                ProjectileID.SniperBullet,
                ProjectileID.RocketSkeleton,
                ProjectileID.BulletDeadeye,
                ProjectileID.Shadowflames
            };

            bossScaleList = new List<int>()
            {
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.Creeper,
                NPCID.SkeletronHand,
                NPCID.WallofFleshEye,
                NPCID.TheHungry,
                NPCID.TheHungryII,
                NPCID.TheDestroyerBody,
                NPCID.TheDestroyerTail,
                NPCID.PrimeCannon,
                NPCID.PrimeVice,
                NPCID.PrimeSaw,
                NPCID.PrimeLaser,
                NPCID.PlanterasTentacle,
                NPCID.Pumpking,
                NPCID.IceQueen,
                NPCID.Mothron,
                NPCID.GolemHead
            };

            bossHPScaleList = new List<int>()
            {
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.SkeletronHand,
                NPCID.WallofFleshEye,
                NPCID.TheDestroyerBody,
                NPCID.TheDestroyerTail,
                NPCID.PrimeCannon,
                NPCID.PrimeLaser,
                NPCID.PrimeVice,
                NPCID.PrimeSaw,
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.GolemFistRight,
                NPCID.GolemFistLeft,
                NPCID.MoonLordHead,
                NPCID.MoonLordHand
            };

            beeEnemyList = new List<int>()
            {
                NPCID.GiantMossHornet,
                NPCID.BigMossHornet,
                NPCID.LittleMossHornet,
                NPCID.TinyMossHornet,
                NPCID.MossHornet,
                NPCID.VortexHornetQueen,
                NPCID.VortexHornet,
                NPCID.Bee,
                NPCID.BeeSmall,
                NPCID.QueenBee,
                ModContent.NPCType<PlaguebringerGoliath>(),
                ModContent.NPCType<PlaguebringerShade>(),
                ModContent.NPCType<PlagueBeeLargeG>(),
                ModContent.NPCType<PlagueBeeLarge>(),
                ModContent.NPCType<PlagueBeeG>(),
                ModContent.NPCType<PlagueBee>()
            };

            beeProjectileList = new List<int>()
            {
                ProjectileID.Stinger,
                ProjectileID.HornetStinger,
                ModContent.ProjectileType<PlagueStingerGoliath>(),
                ModContent.ProjectileType<PlagueStingerGoliathV2>(),
                ModContent.ProjectileType<PlagueExplosion>()
            };

			friendlyBeeList = new List<int>()
			{
				ProjectileID.GiantBee,
				ProjectileID.Bee,
				ProjectileID.Wasp,
				ModContent.ProjectileType<PlaguenadeBee>(),
				ModContent.ProjectileType<PlaguePrincess>(),
				ModContent.ProjectileType<BabyPlaguebringer>(),
				ModContent.ProjectileType<PlagueBeeSmall>()
			};

            hardModeNerfList = new List<int>()
            {
                ProjectileID.WebSpit,
                ProjectileID.PinkLaser,
                ProjectileID.FrostBlastHostile,
                ProjectileID.RuneBlast,
                ProjectileID.GoldenShowerHostile,
                ProjectileID.RainNimbus,
                ProjectileID.Stinger,
                ProjectileID.FlamingArrow,
                ProjectileID.BulletDeadeye,
                ProjectileID.CannonballHostile
            };

            debuffList = new List<int>()
            {
                BuffID.Poisoned,
                BuffID.Darkness,
                BuffID.Cursed,
                BuffID.OnFire,
                BuffID.Bleeding,
                BuffID.Confused,
                BuffID.Slow,
                BuffID.Weak,
                BuffID.Silenced,
                BuffID.BrokenArmor,
                BuffID.CursedInferno,
                BuffID.Frostburn,
                BuffID.Chilled,
                BuffID.Frozen,
                BuffID.Burning,
                BuffID.Suffocation,
                BuffID.Ichor,
                BuffID.Venom,
                BuffID.Blackout,
                BuffID.Electrified,
                BuffID.Rabies,
                BuffID.Webbed,
                BuffID.Stoned,
                BuffID.Dazed,
                BuffID.VortexDebuff,
                BuffID.WitheredArmor,
                BuffID.WitheredWeapon,
                BuffID.OgreSpit,
                BuffID.BetsysCurse,
                ModContent.BuffType<SulphuricPoisoning>(),
                ModContent.BuffType<Shadowflame>(),
                ModContent.BuffType<BrimstoneFlames>(),
                ModContent.BuffType<BurningBlood>(),
                ModContent.BuffType<GlacialState>(),
                ModContent.BuffType<GodSlayerInferno>(),
                ModContent.BuffType<AstralInfectionDebuff>(),
                ModContent.BuffType<HolyFlames>(),
                ModContent.BuffType<Irradiated>(),
                ModContent.BuffType<Plague>(),
                ModContent.BuffType<AbyssalFlames>(),
                ModContent.BuffType<CrushDepth>(),
                ModContent.BuffType<MarkedforDeath>(),
                ModContent.BuffType<WarCleave>(),
                ModContent.BuffType<ArmorCrunch>(),
                ModContent.BuffType<Vaporfied>(),
                ModContent.BuffType<Eutrophication>(),
				ModContent.BuffType<LethalLavaBurn>(),
				ModContent.BuffType<Nightwither>()
			};

            fireWeaponList = new List<int>()
            {
                ItemID.FieryGreatsword,
                ItemID.DD2SquireDemonSword,
                ItemID.TheHorsemansBlade,
                ItemID.Cascade,
                ItemID.HelFire,
                ItemID.Flamarang,
                ItemID.MoltenFury,
                ItemID.Sunfury,
                ItemID.PhoenixBlaster,
                ItemID.Flamelash,
                ItemID.SolarEruption,
                ItemID.DayBreak,
                ItemID.HellwingBow,
                ItemID.DD2PhoenixBow,
                ItemID.DD2BetsyBow,
                ItemID.FlareGun,
                ItemID.Flamethrower,
                ItemID.EldMelter,
                ItemID.FlowerofFire,
                ItemID.MeteorStaff,
                ItemID.ApprenticeStaffT3,
                ItemID.InfernoFork,
                ItemID.HeatRay,
                ItemID.BookofSkulls,
                ItemID.ImpStaff,
                ItemID.DD2FlameburstTowerT1Popper,
                ItemID.DD2FlameburstTowerT2Popper,
                ItemID.DD2FlameburstTowerT3Popper,
                ItemID.MolotovCocktail,
                ItemID.WandofSparking,
                ModContent.ItemType<AegisBlade>(),
                ModContent.ItemType<BalefulHarvester>(),
                ModContent.ItemType<Chaotrix>(),
                ModContent.ItemType<CometQuasher>(),
                ModContent.ItemType<DivineRetribution>(),
                ModContent.ItemType<DraconicDestruction>(),
                ModContent.ItemType<Drataliornus>(),
                ModContent.ItemType<EnergyStaff>(),
                ModContent.ItemType<ExsanguinationLance>(),
                ModContent.ItemType<FirestormCannon>(),
                ModContent.ItemType<FlameburstShortsword>(),
                ModContent.ItemType<FlameScythe>(),
                ModContent.ItemType<FlameScytheMelee>(),
                ModContent.ItemType<FlareBolt>(),
                ModContent.ItemType<FlarewingBow>(),
                ModContent.ItemType<ForbiddenSun>(),
                ModContent.ItemType<GreatbowofTurmoil>(),
                ModContent.ItemType<HarvestStaff>(),
                ModContent.ItemType<Hellborn>(),
                ModContent.ItemType<HellBurst>(),
                ModContent.ItemType<HellfireFlamberge>(),
                ModContent.ItemType<Hellkite>(),
                ModContent.ItemType<HellwingStaff>(),
                ModContent.ItemType<Helstorm>(),
                ModContent.ItemType<HellsSun>(),
                ModContent.ItemType<InfernaCutter>(),
                ModContent.ItemType<Lazhar>(),
                ModContent.ItemType<MeteorFist>(),
                ModContent.ItemType<Mourningstar>(),
                ModContent.ItemType<PhoenixBlade>(),
                ModContent.ItemType<RedSun>(),
                ModContent.ItemType<SparkSpreader>(),
                ModContent.ItemType<SpectralstormCannon>(),
                ModContent.ItemType<SunGodStaff>(),
                ModContent.ItemType<SunSpiritStaff>(),
                ModContent.ItemType<TerraFlameburster>(),
                ModContent.ItemType<TheLastMourning>(),
                ModContent.ItemType<TheWand>(),
                ModContent.ItemType<VenusianTrident>(),
                ModContent.ItemType<Vesuvius>(),
                ModContent.ItemType<BlissfulBombardier>(),
                ModContent.ItemType<HolyCollider>(),
                ModContent.ItemType<MoltenAmputator>(),
                ModContent.ItemType<PurgeGuzzler>(),
                ModContent.ItemType<SolarFlare>(),
                ModContent.ItemType<TelluricGlare>(),
                ModContent.ItemType<AngryChickenStaff>(),
                ModContent.ItemType<ChickenCannon>(),
                ModContent.ItemType<DragonRage>(),
                ModContent.ItemType<DragonsBreath>(),
                ModContent.ItemType<PhoenixFlameBarrage>(),
                ModContent.ItemType<ProfanedTrident>(),
                ModContent.ItemType<TheBurningSky>(),
                ModContent.ItemType<TotalityBreakers>(),
                ModContent.ItemType<ProfanedPartisan>(),
                ModContent.ItemType<BlastBarrel>(),
                ModContent.ItemType<LatcherMine>(),
                ModContent.ItemType<BouncingBetty>(),
                ModContent.ItemType<HeliumFlash>(),
                ModContent.ItemType<ShatteredSun>(),
                ModContent.ItemType<DivineHatchet>(),
                ModContent.ItemType<DazzlingStabberStaff>(),
                ModContent.ItemType<PristineFury>(),
                ModContent.ItemType<SarosPossession>(),
                ModContent.ItemType<CinderBlossomStaff>(),
                ModContent.ItemType<FinalDawn>()
            };

            iceWeaponList = new List<int>()
            {
                ItemID.IceBlade,
                ItemID.IceSickle,
                ItemID.Frostbrand,
                ItemID.Amarok,
                ItemID.NorthPole,
                ItemID.IceBoomerang,
                ItemID.IceBow,
                ItemID.SnowmanCannon,
                ItemID.SnowballCannon,
                ItemID.IceRod,
                ItemID.FlowerofFrost,
                ItemID.FrostStaff,
                ItemID.BlizzardStaff,
                ItemID.StaffoftheFrostHydra,
                ItemID.Snowball,
                ModContent.ItemType<AbsoluteZero>(),
                ModContent.ItemType<Avalanche>(),
                ModContent.ItemType<GlacialCrusher>(),
                ModContent.ItemType<TemporalFloeSword>(),
                ModContent.ItemType<ColdheartIcicle>(),
                ModContent.ItemType<KelvinCatalystMelee>(),
                ModContent.ItemType<CosmicDischarge>(),
                ModContent.ItemType<EffluviumBow>(),
                ModContent.ItemType<EternalBlizzard>(),
                ModContent.ItemType<FrostbiteBlaster>(),
                ModContent.ItemType<IcicleStaff>(),
                ModContent.ItemType<BittercoldStaff>(),
                ModContent.ItemType<CrystalFlareStaff>(),
                ModContent.ItemType<IcicleTrident>(),
                ModContent.ItemType<SnowstormStaff>(),
                ModContent.ItemType<Cryophobia>(),
                ModContent.ItemType<FrostBolt>(),
                ModContent.ItemType<WintersFury>(),
                ModContent.ItemType<ArcticBearPaw>(),
                ModContent.ItemType<AncientIceChunk>(),
                ModContent.ItemType<CryogenicStaff>(),
                ModContent.ItemType<FrostyFlare>(),
                ModContent.ItemType<IceStar>(),
                ModContent.ItemType<Icebreaker>(),
                ModContent.ItemType<KelvinCatalyst>(),
                ModContent.ItemType<FrostcrushValari>(),
                ModContent.ItemType<Endogenesis>(),
                ModContent.ItemType<FlurrystormCannon>(),
                ModContent.ItemType<Hypothermia>(),
                ModContent.ItemType<IceBarrage>(),
                ModContent.ItemType<FrostBlossomStaff>(),
                ModContent.ItemType<EndoHydraStaff>(),
				//Cryonic Bar set stuff, could potentially be removed
                ModContent.ItemType<Trinity>(),
                ModContent.ItemType<Shimmerspark>(),
                ModContent.ItemType<StarnightLance>(),
                ModContent.ItemType<DarkechoGreatbow>(),
                ModContent.ItemType<ShadecrystalTome>(),
                ModContent.ItemType<CrystalPiercer>()
            };

            natureWeaponList = new List<int>()
            {
                ItemID.BladeofGrass,
                ItemID.ChlorophyteClaymore,
                ItemID.ChlorophyteSaber,
                ItemID.ChlorophytePartisan,
                ItemID.ChlorophyteShotbow,
                ItemID.Seedler,
                ItemID.ChristmasTreeSword,
                ItemID.TerraBlade,
                ItemID.JungleYoyo,
                ItemID.Yelets,
                ItemID.MushroomSpear,
                ItemID.ThornChakram,
                ItemID.Bananarang,
                ItemID.FlowerPow,
                ItemID.BeesKnees,
                ItemID.Toxikarp,
                ItemID.Bladetongue,
                ItemID.PoisonStaff,
                ItemID.VenomStaff,
                ItemID.StaffofEarth,
                ItemID.BeeGun,
                ItemID.LeafBlower,
                ItemID.WaspGun,
                ItemID.CrystalSerpent,
                ItemID.Razorpine,
                ItemID.HornetStaff,
                ItemID.QueenSpiderStaff,
                ItemID.SlimeStaff,
                ItemID.PygmyStaff,
                ItemID.RavenStaff,
                ItemID.BatScepter,
                ItemID.SpiderStaff,
                ItemID.Beenade,
                ItemID.FrostDaggerfish,
                ModContent.ItemType<DepthBlade>(),
                ModContent.ItemType<AbyssBlade>(),
                ModContent.ItemType<NeptunesBounty>(),
                ModContent.ItemType<AquaticDissolution>(),
                ModContent.ItemType<ArchAmaryllis>(),
                ModContent.ItemType<ThornBlossom>(),
                ModContent.ItemType<BiomeBlade>(),
                ModContent.ItemType<TrueBiomeBlade>(),
                ModContent.ItemType<OmegaBiomeBlade>(),
                ModContent.ItemType<BladedgeGreatbow>(),
                ModContent.ItemType<BlossomFlux>(),
                ModContent.ItemType<EvergladeSpray>(),
                ModContent.ItemType<FeralthornClaymore>(),
                ModContent.ItemType<Floodtide>(),
                ModContent.ItemType<FourSeasonsGalaxia>(),
                ModContent.ItemType<GammaFusillade>(),
                ModContent.ItemType<GleamingMagnolia>(),
                ModContent.ItemType<HarvestStaff>(),
                ModContent.ItemType<HellionFlowerSpear>(),
                ModContent.ItemType<Lazhar>(),
                ModContent.ItemType<LifefruitScythe>(),
                ModContent.ItemType<ManaRose>(),
                ModContent.ItemType<MangroveChakram>(),
                ModContent.ItemType<MangroveChakramMelee>(),
                ModContent.ItemType<MantisClaws>(),
                ModContent.ItemType<Mariana>(),
                ModContent.ItemType<Mistlestorm>(),
                ModContent.ItemType<Monsoon>(),
                ModContent.ItemType<Alluvion>(),
                ModContent.ItemType<Needler>(),
                ModContent.ItemType<NettlelineGreatbow>(),
                ModContent.ItemType<Quagmire>(),
                ModContent.ItemType<Shroomer>(),
                ModContent.ItemType<SolsticeClaymore>(),
                ModContent.ItemType<SporeKnife>(),
                ModContent.ItemType<Spyker>(),
                ModContent.ItemType<StormSaber>(),
                ModContent.ItemType<StormRuler>(),
                ModContent.ItemType<StormSurge>(),
                ModContent.ItemType<TarragonThrowingDart>(),
                ModContent.ItemType<TerraEdge>(),
                ModContent.ItemType<TerraLance>(),
                ModContent.ItemType<TerraRay>(),
                ModContent.ItemType<TerraShiv>(),
                ModContent.ItemType<Terratomere>(),
                ModContent.ItemType<TerraFlameburster>(),
                ModContent.ItemType<TheSwarmer>(),
                ModContent.ItemType<Verdant>(),
                ModContent.ItemType<Barinautical>(),
                ModContent.ItemType<DeepseaStaff>(),
                ModContent.ItemType<Downpour>(),
                ModContent.ItemType<SubmarineShocker>(),
                ModContent.ItemType<ScourgeoftheSeas>(),
                ModContent.ItemType<Archerfish>(),
                ModContent.ItemType<BallOFugu>(),
                ModContent.ItemType<BlackAnurian>(),
                ModContent.ItemType<CalamarisLament>(),
                ModContent.ItemType<HerringStaff>(),
                ModContent.ItemType<Lionfish>(),
                ModContent.ItemType<ShellfishStaff>(),
                ModContent.ItemType<ClamCrusher>(),
                ModContent.ItemType<ClamorRifle>(),
                ModContent.ItemType<Serpentine>(),
                ModContent.ItemType<UrchinFlail>(),
                ModContent.ItemType<CoralCannon>(),
                ModContent.ItemType<Shellshooter>(),
                ModContent.ItemType<SandDollar>(),
                ModContent.ItemType<MagicalConch>(),
                ModContent.ItemType<SnapClam>(),
                ModContent.ItemType<GacruxianMollusk>(),
                ModContent.ItemType<PolarisParrotfish>(),
                ModContent.ItemType<SparklingEmpress>(),
                ModContent.ItemType<NastyCholla>(),
                ModContent.ItemType<PoisonPack>(),
                ModContent.ItemType<PlantationStaff>(),
                ModContent.ItemType<SeasSearing>(),
                ModContent.ItemType<YateveoBloom>(),
                ModContent.ItemType<TerraDisk>(),
                ModContent.ItemType<TerraDiskMelee>(),
                ModContent.ItemType<BelladonnaSpiritStaff>(),
                ModContent.ItemType<TenebreusTides>(),
                ModContent.ItemType<Greentide>(),
                ModContent.ItemType<Leviatitan>(),
                ModContent.ItemType<BrackishFlask>(),
                ModContent.ItemType<LeviathanTeeth>(),
                ModContent.ItemType<GastricBelcherStaff>()
            };

            alcoholList = new List<int>()
            {
                ModContent.BuffType<BloodyMaryBuff>(),
                ModContent.BuffType<CaribbeanRumBuff>(),
                ModContent.BuffType<CinnamonRollBuff>(),
                ModContent.BuffType<EverclearBuff>(),
                ModContent.BuffType<EvergreenGinBuff>(),
                ModContent.BuffType<FireballBuff>(),
                ModContent.BuffType<GrapeBeerBuff>(),
                ModContent.BuffType<MargaritaBuff>(),
                ModContent.BuffType<MoonshineBuff>(),
                ModContent.BuffType<MoscowMuleBuff>(),
                ModContent.BuffType<RedWineBuff>(),
                ModContent.BuffType<RumBuff>(),
                ModContent.BuffType<ScrewdriverBuff>(),
                ModContent.BuffType<StarBeamRyeBuff>(),
                ModContent.BuffType<TequilaBuff>(),
                ModContent.BuffType<TequilaSunriseBuff>(),
                ModContent.BuffType<VodkaBuff>(),
                ModContent.BuffType<WhiskeyBuff>(),
                ModContent.BuffType<WhiteWineBuff>()
            };

            doubleDamageBuffList = new List<int>()
            {
                ItemID.BallOHurt,
                ItemID.TheMeatball,
                ItemID.BlueMoon,
                ItemID.Sunfury,
                ItemID.DaoofPow,
                ItemID.FlowerPow,
                ItemID.Anchor,
                ItemID.KOCannon,
                ItemID.GolemFist,
                ItemID.BreakerBlade,
                ItemID.MonkStaffT2,
                ItemID.ProximityMineLauncher,
                ItemID.FireworksLauncher,
                ItemID.ShadowbeamStaff
            };

            sixtySixDamageBuffList = new List<int>()
            {
                ItemID.TrueNightsEdge,
                ItemID.MedusaHead,
                ItemID.StaffofEarth,
                ItemID.ChristmasTreeSword,
                ItemID.MonkStaffT1,
                ItemID.InfernoFork,
                ItemID.VenomStaff,
                ItemID.Frostbrand
            };

            fiftyDamageBuffList = new List<int>()
            {
                ItemID.NightsEdge,
                ItemID.EldMelter,
                ItemID.Flamethrower,
                ItemID.MoonlordTurretStaff,
                ItemID.WaspGun,
                ItemID.Keybrand,
                ItemID.PulseBow,
                ItemID.PaladinsHammer,
                ItemID.SolarEruption,
                ItemID.DayBreak
            };

            thirtyThreeDamageBuffList = new List<int>()
            {
                ItemID.WandofSparking,
				ItemID.IceBow,
				ItemID.Marrow,
                ItemID.CrystalVileShard,
                ItemID.SoulDrain,
                ItemID.ClingerStaff,
                ItemID.ChargedBlasterCannon,
                ItemID.NettleBurst,
                ItemID.Excalibur,
                ItemID.AmberStaff,
                ItemID.BluePhasesaber,
                ItemID.RedPhasesaber,
                ItemID.GreenPhasesaber,
                ItemID.WhitePhasesaber,
                ItemID.YellowPhasesaber,
                ItemID.PurplePhasesaber,
                ItemID.TheRottedFork,
                ItemID.VampireKnives,
                ItemID.Cascade,
                ItemID.TrueExcalibur
            };

            twentyFiveDamageBuffList = new List<int>()
            {
                ItemID.Muramasa,
                ItemID.StakeLauncher,
                ItemID.BookStaff
            };

            twentyDamageBuffList = new List<int>()
            {
                ItemID.ChainGuillotines,
                ItemID.FlowerofFrost,
                ItemID.PoisonStaff,
                ItemID.Gungnir,
                ItemID.TacticalShotgun
            };

            tenDamageBuffList = new List<int>()
            {
                ItemID.MagnetSphere,
                ItemID.BatScepter
            };

            weaponAutoreuseList = new List<int>()
            {
                ItemID.NightsEdge,
                ItemID.TrueNightsEdge,
                ItemID.TrueExcalibur,
                ItemID.PhoenixBlaster,
                ItemID.VenusMagnum,
                ItemID.MagicDagger,
                ItemID.BeamSword,
                ItemID.MonkStaffT2,
                ItemID.PaladinsHammer,
                ItemID.PearlwoodSword,
                ItemID.PearlwoodBow,
                ItemID.TaxCollectorsStickOfDoom,
                ItemID.StylistKilLaKillScissorsIWish
            };

            tenDamageNerfList = new List<int>()
            {
                ItemID.Phantasm
            };

            quarterDamageNerfList = new List<int>()
            {
                ItemID.Razorpine,
                ItemID.DaedalusStormbow,
                ItemID.PhoenixBlaster,
                ItemID.DD2BetsyBow,
                ItemID.InfluxWaver,
                ItemID.Xenopopper,
                ItemID.ElectrosphereLauncher,
                ItemID.OpticStaff //Note: got local i frames so it should be better
            };

            pumpkinMoonBuffList = new List<int>()
            {
                NPCID.Scarecrow1,
                NPCID.Scarecrow2,
                NPCID.Scarecrow3,
                NPCID.Scarecrow4,
                NPCID.Scarecrow5,
                NPCID.Scarecrow6,
                NPCID.Scarecrow7,
                NPCID.Scarecrow8,
                NPCID.Scarecrow9,
                NPCID.Scarecrow10,
                NPCID.HeadlessHorseman,
                NPCID.MourningWood,
                NPCID.Splinterling,
                NPCID.Pumpking,
                NPCID.PumpkingBlade,
                NPCID.Hellhound,
                NPCID.Poltergeist
            };

            frostMoonBuffList = new List<int>()
            {
                NPCID.ZombieElf,
                NPCID.ZombieElfBeard,
                NPCID.ZombieElfGirl,
                NPCID.PresentMimic,
                NPCID.GingerbreadMan,
                NPCID.Yeti,
                NPCID.Everscream,
                NPCID.IceQueen,
                NPCID.SantaNK1,
                NPCID.ElfCopter,
                NPCID.Nutcracker,
                NPCID.NutcrackerSpinning,
                NPCID.ElfArcher,
                NPCID.Krampus,
                NPCID.Flocko
            };

            eclipseBuffList = new List<int>()
            {
                NPCID.Eyezor,
                NPCID.Reaper,
                NPCID.Frankenstein,
                NPCID.SwampThing,
                NPCID.Vampire,
                NPCID.VampireBat,
                NPCID.Butcher,
                NPCID.CreatureFromTheDeep,
                NPCID.Fritz,
                NPCID.Nailhead,
                NPCID.Psycho,
                NPCID.DeadlySphere,
                NPCID.DrManFly,
                NPCID.ThePossessed,
                NPCID.Mothron,
                NPCID.MothronEgg,
                NPCID.MothronSpawn
            };

            eventProjectileBuffList = new List<int>()
            {
                ProjectileID.FlamingWood,
                ProjectileID.GreekFire1,
                ProjectileID.GreekFire2,
                ProjectileID.GreekFire3,
                ProjectileID.FlamingScythe,
                ProjectileID.FlamingArrow,
                ProjectileID.PineNeedleHostile,
                ProjectileID.OrnamentHostile,
                ProjectileID.OrnamentHostileShrapnel,
                ProjectileID.FrostWave,
                ProjectileID.FrostShard,
                ProjectileID.Missile,
                ProjectileID.Present,
                ProjectileID.Spike,
                ProjectileID.BulletDeadeye,
                ProjectileID.EyeLaser,
                ProjectileID.Nail,
                ProjectileID.DrManFlyFlask
            };

			// Enemies that inflict an average of 1 to 50 damage in Expert Mode
			revengeanceEnemyBuffList25Percent = new List<int>()
            {
                NPCID.ServantofCthulhu,
                NPCID.EyeofCthulhu,
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.Creeper,
                NPCID.Bee,
                NPCID.BeeSmall,
                NPCID.SkeletronHand,
				NPCID.GiantWormHead,
				NPCID.BlazingWheel,
				ModContent.NPCType<DesertScourgeBody>(),
				ModContent.NPCType<DesertScourgeTail>(),
				ModContent.NPCType<DesertScourgeHeadSmall>(),
				ModContent.NPCType<DesertScourgeBodySmall>(),
				ModContent.NPCType<DesertScourgeTailSmall>(),
				ModContent.NPCType<DriedSeekerHead>(),
                ModContent.NPCType<CrabShroom>(),
                ModContent.NPCType<HiveMind>(),
                ModContent.NPCType<DankCreeper>(),
                ModContent.NPCType<PerforatorHive>(),
				ModContent.NPCType<AquaticSeekerHead>(),
				ModContent.NPCType<Cnidrion>(),
                ModContent.NPCType<PrismTurtle>(),
                ModContent.NPCType<GhostBell>()
            };

			// Enemies that inflict an average of 51 to 100 damage in Expert Mode
			revengeanceEnemyBuffList20Percent = new List<int>()
			{
				NPCID.KingSlime,
				NPCID.BrainofCthulhu,
				NPCID.QueenBee,
				NPCID.SkeletronHead,
				NPCID.TheHungryII,
				NPCID.LeechHead,
				NPCID.TheDestroyerBody,
				NPCID.TheDestroyerTail,
				NPCID.Probe,
				NPCID.PrimeSaw,
				NPCID.PrimeVice,
				NPCID.PrimeLaser,
				NPCID.PrimeCannon,
				NPCID.DevourerHead,
				NPCID.MeteorHead,
				NPCID.BoneSerpentHead,
				NPCID.ManEater,
				NPCID.Snatcher,
				NPCID.Piranha,
				NPCID.Shark,
				NPCID.SpikeBall,
				NPCID.DiggerHead,
				NPCID.WallCreeper,
				NPCID.WallCreeperWall,
				NPCID.Lihzahrd,
				NPCID.Pumpking,
				NPCID.SlimeSpiked,
				ModContent.NPCType<DesertScourgeHead>(),
				ModContent.NPCType<CrabulonIdle>(),
				ModContent.NPCType<HiveMindP2>(),
				ModContent.NPCType<PerforatorHeadLarge>(),
				ModContent.NPCType<PerforatorHeadMedium>(),
				ModContent.NPCType<PerforatorHeadSmall>(),
				ModContent.NPCType<SlimeGod>(),
				ModContent.NPCType<SlimeGodRun>(),
				ModContent.NPCType<SlimeGodCore>(),
				ModContent.NPCType<SlimeGodSplit>(),
				ModContent.NPCType<SlimeGodRunSplit>(),
				ModContent.NPCType<SlimeSpawnCorrupt>(),
				ModContent.NPCType<SlimeSpawnCrimson2>(),
				ModContent.NPCType<Cryocore>(),
				ModContent.NPCType<Cryocore2>(),
				ModContent.NPCType<IceMass>(),
				ModContent.NPCType<AquaticScourgeTail>(),
				ModContent.NPCType<BrimstoneElemental>(),
				ModContent.NPCType<LifeSeeker>(),
				ModContent.NPCType<SoulSeeker>(),
				ModContent.NPCType<EutrophicRay>(),
				ModContent.NPCType<Clam>(),
				ModContent.NPCType<SeaSerpent1>(),
				ModContent.NPCType<GiantClam>(),
				ModContent.NPCType<FearlessGoldfishWarrior>()
			};

			// Enemies that inflict an average of 101 to 200 damage in Expert Mode
			revengeanceEnemyBuffList15Percent = new List<int>()
			{
				NPCID.WallofFlesh,
				NPCID.Spazmatism,
				NPCID.Retinazer,
				NPCID.SkeletronPrime,
				NPCID.Plantera,
				NPCID.PlanterasTentacle,
				NPCID.Spore,
				NPCID.DetonatingBubble,
				NPCID.Golem,
				NPCID.GolemHead,
				NPCID.GolemHeadFree,
				NPCID.GolemFistLeft,
				NPCID.GolemFistRight,
				NPCID.DukeFishron,
				NPCID.Sharkron,
				NPCID.Sharkron2,
				NPCID.CultistDragonHead,
				NPCID.AncientCultistSquidhead,
				NPCID.AncientLight,
				NPCID.DD2Betsy,
				NPCID.Mimic,
				NPCID.WyvernHead,
				NPCID.SeekerHead,
				NPCID.AnglerFish,
				NPCID.Werewolf,
				NPCID.Wraith,
				NPCID.Arapaima,
				NPCID.BlackRecluse,
				NPCID.BlackRecluseWall,
				NPCID.AngryTrapper,
				NPCID.LihzahrdCrawler,
				NPCID.PirateCaptain,
				NPCID.FlyingSnake,
				NPCID.Reaper,
				NPCID.Paladin,
				NPCID.BoneLee,
				NPCID.MourningWood,
				NPCID.PumpkingBlade,
				NPCID.PresentMimic,
				NPCID.Everscream,
				NPCID.IceQueen,
				NPCID.SantaNK1,
				NPCID.StardustWormHead,
				NPCID.Butcher,
				NPCID.Psycho,
				NPCID.DeadlySphere,
				NPCID.BigMimicCorruption,
				NPCID.BigMimicCrimson,
				NPCID.BigMimicHallow,
				NPCID.Mothron,
				NPCID.DuneSplicerHead,
				NPCID.SandShark,
				NPCID.SandsharkCorrupt,
				NPCID.SandsharkCrimson,
				NPCID.SandsharkHallow,
				ModContent.NPCType<Cryogen>(),
				ModContent.NPCType<CryogenIce>(),
				ModContent.NPCType<AquaticScourgeHead>(),
				ModContent.NPCType<AquaticScourgeBody>(),
				ModContent.NPCType<AquaticScourgeBodyAlt>(),
				ModContent.NPCType<Calamitas>(),
				ModContent.NPCType<CalamitasRun3>(),
				ModContent.NPCType<CalamitasRun>(),
				ModContent.NPCType<CalamitasRun2>(),
				ModContent.NPCType<Parasea>(),
				ModContent.NPCType<AquaticAberration>(),
				ModContent.NPCType<Leviathan>(),
				ModContent.NPCType<SirenIce>(),
				ModContent.NPCType<Siren>(),
				ModContent.NPCType<AstrumAureus>(),
				ModContent.NPCType<PlaguebringerGoliath>(),
				ModContent.NPCType<PlaguebringerShade>(),
				ModContent.NPCType<PlagueHomingMissile>(),
				ModContent.NPCType<PlagueBeeG>(),
				ModContent.NPCType<PlagueBeeLargeG>(),
				ModContent.NPCType<RavagerBody>(),
				ModContent.NPCType<RavagerClawRight>(),
				ModContent.NPCType<RavagerClawLeft>(),
				ModContent.NPCType<ProfanedGuardianBoss2>(),
				ModContent.NPCType<ProfanedGuardianBoss3>(),
				ModContent.NPCType<Atlas>(),
				ModContent.NPCType<ArmoredDiggerHead>(),
				ModContent.NPCType<GreatSandShark>(),
				ModContent.NPCType<Horse>(),
				ModContent.NPCType<ScornEater>(),
				ModContent.NPCType<BlindedAngler>()
			};

			// Enemies that inflict an average of 201 to 400 damage in Expert Mode
			revengeanceEnemyBuffList10Percent = new List<int>()
			{
				NPCID.TheDestroyer,
				NPCID.SolarCrawltipedeHead,
				ModContent.NPCType<AstrumDeusHeadSpectral>(),
				ModContent.NPCType<ProfanedGuardianBoss>(),
				ModContent.NPCType<Bumblefuck>(),
				ModContent.NPCType<Bumblefuck2>(),

				ModContent.NPCType<ProvSpawnDefense>(),
				ModContent.NPCType<ProvSpawnOffense>(),
				ModContent.NPCType<CeaselessVoid>(),
				ModContent.NPCType<StormWeaverHead>(),
				ModContent.NPCType<StormWeaverHeadNaked>(),
				ModContent.NPCType<Signus>(),
				ModContent.NPCType<CosmicLantern>(),
				ModContent.NPCType<Polterghast>(),
				ModContent.NPCType<PolterPhantom>(),
				ModContent.NPCType<OldDuke>(),
				ModContent.NPCType<OldDukeToothBall>(),
				ModContent.NPCType<OldDukeSharkron>(),
				ModContent.NPCType<DevourerofGodsBody>(),
				ModContent.NPCType<DevourerofGodsTail>(),
				ModContent.NPCType<DevourerofGodsBodyS>(),
				ModContent.NPCType<DevourerofGodsTailS>(),
				ModContent.NPCType<DevourerofGodsHead2>(),
				ModContent.NPCType<DetonatingFlare>(),
				ModContent.NPCType<BobbitWormHead>(),
				ModContent.NPCType<ColossalSquid>(),
				ModContent.NPCType<EidolonWyrmHead>(),
				ModContent.NPCType<GulperEelHead>(),
				ModContent.NPCType<Mauler>(),
				ModContent.NPCType<Reaper>()
			};

			// Enemies that inflict an average of 401+ damage in Expert Mode
			revengeanceEnemyBuffList5Percent = new List<int>()
			{
				ModContent.NPCType<DevourerofGodsHead>(),
				ModContent.NPCType<DevourerofGodsHeadS>(),
				ModContent.NPCType<Yharon>(),
				ModContent.NPCType<DetonatingFlare2>(),
				ModContent.NPCType<SupremeCalamitas>()
			};

			revengeanceProjectileBuffList25Percent = new List<int>()
            {
                ProjectileID.SandBallFalling,
                ProjectileID.AshBallFalling,
                ProjectileID.EbonsandBallFalling,
                ProjectileID.PearlSandBallFalling,
                ProjectileID.CrimsandBallFalling,
                ProjectileID.GeyserTrap,
                ModContent.ProjectileType<SandBlast>(),
                ModContent.ProjectileType<MushBomb>(),
                ModContent.ProjectileType<MushBombFall>(),
				ModContent.ProjectileType<BloodGeyser>(),
                ModContent.ProjectileType<IchorShot>()
            };

			revengeanceProjectileBuffList20Percent = new List<int>()
			{
				ProjectileID.PoisonDartTrap,
				ProjectileID.DemonSickle,
				ProjectileID.SandnadoHostile,
				ProjectileID.DD2BetsyFireball,
				ProjectileID.DD2BetsyFlameBreath,
				ModContent.ProjectileType<Mushmash>(),
				ModContent.ProjectileType<ShaderainHostile>(),
				ModContent.ProjectileType<AbyssMine>(),
				ModContent.ProjectileType<AbyssMine2>(),
				ModContent.ProjectileType<AbyssBallVolley>(),
				ModContent.ProjectileType<AbyssBallVolley2>(),
				ModContent.ProjectileType<IceBlast>(),
				ModContent.ProjectileType<IceBomb>(),
				ModContent.ProjectileType<IceRain>(),
				ModContent.ProjectileType<SandTooth>()
			};

			revengeanceProjectileBuffList15Percent = new List<int>()
			{
				ProjectileID.SpikyBallTrap,
				ProjectileID.SpearTrap,
				ProjectileID.FlamethrowerTrap,
				ProjectileID.FlamesTrap,
				ProjectileID.Fireball,
				ProjectileID.PaladinsHammerHostile,
				ProjectileID.FlamingWood,
				ProjectileID.FlamingScythe,
				ProjectileID.FrostWave,
				ProjectileID.Present,
				ProjectileID.Spike,
				ProjectileID.SaucerDeathray,
				ProjectileID.NebulaBolt,
				ProjectileID.NebulaSphere,
				ProjectileID.NebulaLaser,
				ProjectileID.StardustSoldierLaser,
				ProjectileID.VortexLaser,
				ProjectileID.VortexVortexLightning,
				ProjectileID.VortexLightning,
				ProjectileID.VortexAcid,
				ProjectileID.Sharknado,
				ProjectileID.Cthulunado,
				ModContent.ProjectileType<SandPoisonCloud>(),
				ModContent.ProjectileType<BrimstoneHellfireball>(),
				ModContent.ProjectileType<HellfireExplosion>(),
				ModContent.ProjectileType<BrimstoneRay>(),
				ModContent.ProjectileType<BrimstoneLaser>(),
				ModContent.ProjectileType<BrimstoneLaserSplit>(),
				ModContent.ProjectileType<BrimstoneBall>(),
				ModContent.ProjectileType<BrimstoneFire>(),
				ModContent.ProjectileType<LeviathanBomb>(),
				ModContent.ProjectileType<WaterSpear>(),
				ModContent.ProjectileType<FrostMist>(),
				ModContent.ProjectileType<SirenSong>(),
				ModContent.ProjectileType<AstralFlame>(),
				ModContent.ProjectileType<AstralLaser>(),
				ModContent.ProjectileType<PlagueExplosion>(),
				ModContent.ProjectileType<PlagueStingerGoliath>(),
				ModContent.ProjectileType<PlagueStingerGoliathV2>(),
				ModContent.ProjectileType<HiveBombGoliath>(),
				ModContent.ProjectileType<ScavengerNuke>(),
				ModContent.ProjectileType<AstralShot2>(),
				ModContent.ProjectileType<DeusMine>(),
				ModContent.ProjectileType<HolyBomb>(),
				ModContent.ProjectileType<HolyFire>(),
				ModContent.ProjectileType<HolyFire2>(),
				ModContent.ProjectileType<HolyFlare>(),
				ModContent.ProjectileType<MoltenBlast>(),
				ModContent.ProjectileType<MoltenBlob>(),
				ModContent.ProjectileType<DarkEnergyBall>(),
				ModContent.ProjectileType<DoGBeam>(),
				ModContent.ProjectileType<GreatSandBlast>(),
				ModContent.ProjectileType<PearlBurst>(),
				ModContent.ProjectileType<PearlRain>()
			};

			revengeanceProjectileBuffList10Percent = new List<int>()
			{
				ProjectileID.Boulder,
				ModContent.ProjectileType<ProfanedSpear>(),
				ModContent.ProjectileType<FlareDust>(),
				ModContent.ProjectileType<RedLightningFeather>(),
				ModContent.ProjectileType<RedLightning>(),
				ModContent.ProjectileType<HolyBlast>(),
				ModContent.ProjectileType<HolySpear>(),
				ModContent.ProjectileType<ProvidenceCrystalShard>(),
				ModContent.ProjectileType<ProvidenceHolyRay>(),
				ModContent.ProjectileType<SignusScythe>(),
				ModContent.ProjectileType<EssenceDust>(),
				ModContent.ProjectileType<PhantomBlast>(),
				ModContent.ProjectileType<PhantomBlast2>(),
				ModContent.ProjectileType<PhantomGhostShot>(),
				ModContent.ProjectileType<PhantomHookShot>(),
				ModContent.ProjectileType<PhantomMine>(),
				ModContent.ProjectileType<PhantomShot>(),
				ModContent.ProjectileType<PhantomShot2>(),
				ModContent.ProjectileType<OldDukeGore>(),
				ModContent.ProjectileType<OldDukeVortex>(),
				ModContent.ProjectileType<DoGDeath>(),
				ModContent.ProjectileType<DoGFire>(),
				ModContent.ProjectileType<DoGNebulaShot>(),
				ModContent.ProjectileType<FlareBomb>(),
				ModContent.ProjectileType<FlareDust2>(),
				ModContent.ProjectileType<Flarenado>()
			};

			revengeanceProjectileBuffList5Percent = new List<int>()
			{
				ModContent.ProjectileType<Infernado>(),
				ModContent.ProjectileType<Infernado2>(),
				ModContent.ProjectileType<YharonFireball>(),
				ModContent.ProjectileType<YharonFireball2>(),
				ModContent.ProjectileType<BrimstoneBarrage>(),
				ModContent.ProjectileType<BrimstoneHellblast>(),
				ModContent.ProjectileType<BrimstoneFireblast>(),
				ModContent.ProjectileType<BrimstoneGigaBlast>(),
				ModContent.ProjectileType<BrimstoneHellblast2>(),
				ModContent.ProjectileType<BrimstoneMonster>(),
				ModContent.ProjectileType<BrimstoneWave>()
			};

			revengeanceLifeStealExceptionList = new List<int>()
            {
                NPCID.Probe,
                NPCID.CultistDragonHead,
                NPCID.CultistDragonBody1,
                NPCID.CultistDragonBody2,
                NPCID.CultistDragonBody3,
                NPCID.CultistDragonBody4,
                NPCID.CultistDragonTail,
                NPCID.AncientCultistSquidhead,
                NPCID.AncientLight,
                NPCID.Sharkron,
                NPCID.Sharkron2,
                NPCID.PlanterasTentacle,
                NPCID.Spore,
                NPCID.TheHungryII,
                NPCID.LeechHead,
                NPCID.LeechBody,
                NPCID.LeechTail,
                NPCID.TheDestroyerBody,
                NPCID.TheDestroyerTail,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.GolemFistRight,
                NPCID.GolemFistLeft,
                NPCID.MoonLordCore
            };

            movementImpairImmuneList = new List<int>()
            {
                NPCID.QueenBee,
            };

            needsDebuffIconDisplayList = new List<int>()
            {
                NPCID.WallofFleshEye
            };

            trapProjectileList = new List<int>()
            {
                ProjectileID.PoisonDartTrap,
                ProjectileID.SpikyBallTrap,
                ProjectileID.SpearTrap,
                ProjectileID.FlamethrowerTrap,
                ProjectileID.FlamesTrap,
                ProjectileID.PoisonDart,
                ProjectileID.GeyserTrap
            };

            scopedWeaponList = new List<int>()
            {
                ModContent.ItemType<AMR>(),
                ModContent.ItemType<HalleysInferno>(),
                ModContent.ItemType<Shroomer>(),
                ModContent.ItemType<SpectreRifle>(),
                ModContent.ItemType<Svantechnical>(),
                ModContent.ItemType<Skullmasher>(),
                ModContent.ItemType<TyrannysEnd>()
            };

            boomerangList = new List<int>()
            {
                ModContent.ItemType<Brimblade>(),
                ModContent.ItemType<BlazingStar>(),
                ModContent.ItemType<Celestus>(),
                ModContent.ItemType<AccretionDisk>(),
                ModContent.ItemType<EnchantedAxe>(),
                ModContent.ItemType<EpidemicShredder>(),
                ModContent.ItemType<Equanimity>(),
                ModContent.ItemType<Eradicator>(),
                ModContent.ItemType<TruePaladinsHammer>(),
                ModContent.ItemType<FlameScythe>(),
                ModContent.ItemType<GalaxySmasherRogue>(),
                ModContent.ItemType<Glaive>(),
                ModContent.ItemType<GhoulishGouger>(),
                ModContent.ItemType<Icebreaker>(),
                ModContent.ItemType<KelvinCatalyst>(),
                ModContent.ItemType<Kylie>(),
                ModContent.ItemType<MangroveChakram>(),
                ModContent.ItemType<MoltenAmputator>(),
                ModContent.ItemType<NanoblackReaperRogue>(),
                ModContent.ItemType<Pwnagehammer>(),
                ModContent.ItemType<SandDollar>(),
                ModContent.ItemType<SeashellBoomerang>(),
                ModContent.ItemType<Shroomerang>(),
                ModContent.ItemType<StellarContemptRogue>(),
                ModContent.ItemType<TriactisTruePaladinianMageHammerofMight>(),
                ModContent.ItemType<Valediction>(),
                ModContent.ItemType<FrostcrushValari>(),
                ModContent.ItemType<DefectiveSphere>(),
                ModContent.ItemType<TerraDisk>(),
                ModContent.ItemType<ToxicantTwister>()
            };

            boomerangProjList = new List<int>()
            {
                ModContent.ProjectileType<AccretionDiskProj>(),
                ModContent.ProjectileType<AccretionDisk2>(),
                ModContent.ProjectileType<BlazingStarProj>(),
                ModContent.ProjectileType<CelestusBoomerang>(),
                ModContent.ProjectileType<BrimbladeProj>(),
                ModContent.ProjectileType<Brimblade2>(),
                ModContent.ProjectileType<EnchantedAxeProj>(),
                ModContent.ProjectileType<EpidemicShredderProjectile>(),
                ModContent.ProjectileType<EquanimityProj>(),
                ModContent.ProjectileType<EradicatorProjectile>(),
                ModContent.ProjectileType<FlameScytheProjectile>(),
                ModContent.ProjectileType<GhoulishGougerBoomerang>(),
                ModContent.ProjectileType<GlaiveProj>(),
                ModContent.ProjectileType<KylieBoomerang>(),
                ModContent.ProjectileType<MangroveChakramProjectile>(),
                ModContent.ProjectileType<MoltenAmputatorProj>(),
                ModContent.ProjectileType<OPHammer>(),
                ModContent.ProjectileType<SandDollarProj>(),
                ModContent.ProjectileType<SandDollarStealth>(),
                ModContent.ProjectileType<SeashellBoomerangProjectile>(),
                ModContent.ProjectileType<ShroomerangProj>(),
                ModContent.ProjectileType<TriactisOPHammer>(),
                ModContent.ProjectileType<ValedictionBoomerang>(),
                ModContent.ProjectileType<GalaxySmasherHammer>(),
                ModContent.ProjectileType<KelvinCatalystBoomerang>(),
                ModContent.ProjectileType<NanoblackMain>(),
                ModContent.ProjectileType<StellarContemptHammer>(),
                ModContent.ProjectileType<IcebreakerHammer>(),
                ModContent.ProjectileType<PwnagehammerProj>(),
                ModContent.ProjectileType<ValariBoomerang>(),
                ModContent.ProjectileType<SphereSpiked>(),
                ModContent.ProjectileType<SphereBladed>(),
                ModContent.ProjectileType<SphereYellow>(),
                ModContent.ProjectileType<ButcherKnife>(),
                ModContent.ProjectileType<TerraDiskProjectile>(),
                ModContent.ProjectileType<TerraDiskProjectile2>(),
                ModContent.ProjectileType<ToxicantTwisterProjectile>(),
                ModContent.ProjectileType<ToxicantTwisterTwoPointZero>()
            };

            javelinList = new List<int>()
            {
                ModContent.ItemType<CrystalPiercer>(),
                ModContent.ItemType<PalladiumJavelin>(),
                ModContent.ItemType<DuneHopper>(),
                ModContent.ItemType<EclipsesFall>(),
                ModContent.ItemType<IchorSpear>(),
                ModContent.ItemType<ProfanedTrident>(),
                ModContent.ItemType<LuminousStriker>(),
                ModContent.ItemType<ScarletDevil>(),
                ModContent.ItemType<ScourgeoftheCosmosThrown>(),
                ModContent.ItemType<ScourgeoftheDesert>(),
                ModContent.ItemType<ScourgeoftheSeas>(),
                ModContent.ItemType<SpearofDestiny>(),
                ModContent.ItemType<SpearofPaleolith>(),
                ModContent.ItemType<XerocPitchfork>(),
                ModContent.ItemType<PhantasmalRuin>(),
                ModContent.ItemType<PhantomLance>(),
                ModContent.ItemType<ProfanedPartisan>(),
                ModContent.ItemType<Turbulance>(),
                ModContent.ItemType<NightsGaze>()
            };

            javelinProjList = new List<int>()
            {
                ModContent.ProjectileType<CrystalPiercerProjectile>(),
                ModContent.ProjectileType<DuneHopperProjectile>(),
                ModContent.ProjectileType<EclipsesFallMain>(),
                ModContent.ProjectileType<EclipsesStealth>(),
                ModContent.ProjectileType<IchorSpearProj>(),
                ModContent.ProjectileType<InfernalSpearProjectile>(),
                ModContent.ProjectileType<LuminousStrikerProj>(),
                ModContent.ProjectileType<PalladiumJavelinProjectile>(),
                ModContent.ProjectileType<PhantasmalRuinProj>(),
                ModContent.ProjectileType<PhantomLanceProj>(),
                ModContent.ProjectileType<ProfanedPartisanproj>(),
                ModContent.ProjectileType<ScarletDevilProjectile>(),
                ModContent.ProjectileType<ScourgeoftheDesertProj>(),
                ModContent.ProjectileType<ScourgeoftheSeasProjectile>(),
                ModContent.ProjectileType<ScourgeoftheCosmosProj>(),
                ModContent.ProjectileType<SpearofDestinyProjectile>(),
                ModContent.ProjectileType<SpearofPaleolithProj>(),
                ModContent.ProjectileType<AntumbraShardProjectile>(),
                ModContent.ProjectileType<TurbulanceProjectile>(),
                ModContent.ProjectileType<NightsGazeProjectile>()
            };

            daggerList = new List<int>()
            {
                ModContent.ItemType<AshenStalactite>(),
                ModContent.ItemType<CobaltKunai>(),
                ModContent.ItemType<FeatherKnife>(),
                ModContent.ItemType<GelDart>(),
                ModContent.ItemType<MonkeyDarts>(),
                ModContent.ItemType<MythrilKnife>(),
                ModContent.ItemType<OrichalcumSpikedGemstone>(),
                ModContent.ItemType<TarragonThrowingDart>(),
                ModContent.ItemType<WulfrumKnife>(),
                ModContent.ItemType<Cinquedea>(),
                ModContent.ItemType<CosmicKunai>(),
                ModContent.ItemType<CorpusAvertor>(),
                ModContent.ItemType<Crystalline>(),
                ModContent.ItemType<CursedDagger>(),
                ModContent.ItemType<Malachite>(),
                ModContent.ItemType<Mycoroot>(),
                ModContent.ItemType<Prismalline>(),
                ModContent.ItemType<Quasar>(),
                ModContent.ItemType<RadiantStar>(),
                ModContent.ItemType<ShatteredSun>(),
                ModContent.ItemType<StellarKnife>(),
                ModContent.ItemType<StormfrontRazor>(),
                ModContent.ItemType<TimeBolt>(),
                ModContent.ItemType<LunarKunai>(),
                ModContent.ItemType<GildedDagger>(),
                ModContent.ItemType<GleamingDagger>(),
                ModContent.ItemType<EmpyreanKnives>(),
                ModContent.ItemType<RoyalKnives>(),
                ModContent.ItemType<InfernalKris>(),
                ModContent.ItemType<UtensilPoker>(),
                ModContent.ItemType<ShinobiBlade>(),
                ModContent.ItemType<JawsOfOblivion>(),
                ModContent.ItemType<LeviathanTeeth>(),
                ModContent.ItemType<DeificThunderbolt>()
            };

            daggerProjList = new List<int>()
            {
                ModContent.ProjectileType<AshenStalactiteProj>(),
                ModContent.ProjectileType<AshenStalagmiteProj>(),
                ModContent.ProjectileType<CinquedeaProj>(),
                ModContent.ProjectileType<CobaltKunaiProjectile>(),
                ModContent.ProjectileType<CosmicKunaiProj>(),
                ModContent.ProjectileType<CrystallineProj>(),
                ModContent.ProjectileType<Crystalline2>(),
                ModContent.ProjectileType<CursedDaggerProj>(),
                ModContent.ProjectileType<EmpyreanKnife>(),
                ModContent.ProjectileType<FeatherKnifeProjectile>(),
                ModContent.ProjectileType<GelDartProjectile>(),
                ModContent.ProjectileType<GildedDaggerProj>(),
                ModContent.ProjectileType<GleamingDaggerProj>(),
                ModContent.ProjectileType<IllustriousKnife>(),
                ModContent.ProjectileType<LunarKunaiProj>(),
                ModContent.ProjectileType<MalachiteProj>(),
                ModContent.ProjectileType<MalachiteBolt>(),
                ModContent.ProjectileType<MalachiteStealth>(),
                ModContent.ProjectileType<MonkeyDart>(),
                ModContent.ProjectileType<MycorootProj>(),
                ModContent.ProjectileType<MythrilKnifeProjectile>(),
                ModContent.ProjectileType<OrichalcumSpikedGemstoneProjectile>(),
                ModContent.ProjectileType<PrismallineProj>(),
                ModContent.ProjectileType<Prismalline2>(),
                ModContent.ProjectileType<Prismalline3>(),
                ModContent.ProjectileType<QuasarKnife>(),
                ModContent.ProjectileType<Quasar2>(),
                ModContent.ProjectileType<RadiantStarKnife>(),
                ModContent.ProjectileType<RadiantStar2>(),
                ModContent.ProjectileType<ShatteredSunKnife>(),
                ModContent.ProjectileType<StellarKnifeProj>(),
                ModContent.ProjectileType<StormfrontRazorProjectile>(),
                ModContent.ProjectileType<TarragonThrowingDartProjectile>(),
                ModContent.ProjectileType<TimeBoltKnife>(),
                ModContent.ProjectileType<WulfrumKnifeProj>(),
                ModContent.ProjectileType<Fork>(),
                ModContent.ProjectileType<Knife>(),
                ModContent.ProjectileType<CarvingFork>(),
                ModContent.ProjectileType<InfernalKrisProjectile>(),
                ModContent.ProjectileType<ShinobiBladeProjectile>(),
                ModContent.ProjectileType<JawsProjectile>(),
                ModContent.ProjectileType<LeviathanTooth>(),
                ModContent.ProjectileType<DeificThunderboltProj>()
            };

            flaskBombList = new List<int>()
            {
                ModContent.ItemType<Plaguenade>(),
                ModContent.ItemType<BallisticPoisonBomb>(),
                ModContent.ItemType<BrackishFlask>(),
                ModContent.ItemType<DuststormInABottle>(),
                ModContent.ItemType<SeafoamBomb>(),
                ModContent.ItemType<ConsecratedWater>(),
                ModContent.ItemType<DesecratedWater>(),
                ModContent.ItemType<BouncingBetty>(),
                ModContent.ItemType<TotalityBreakers>(),
                ModContent.ItemType<BlastBarrel>(),
                ModContent.ItemType<Penumbra>(),
                ModContent.ItemType<LatcherMine>(),
                ModContent.ItemType<Supernova>(),
                ModContent.ItemType<ShockGrenade>(),
                ModContent.ItemType<Exorcism>(),
                ModContent.ItemType<MeteorFist>(),
                ModContent.ItemType<StarofDestruction>(),
                ModContent.ItemType<CraniumSmasher>(),
                ModContent.ItemType<ContaminatedBile>(),
                ModContent.ItemType<AcidicRainBarrel>(),
                ModContent.ItemType<SkyfinBombers>(),
                ModContent.ItemType<SpentFuelContainer>(),
                ModContent.ItemType<SealedSingularity>(),
                ModContent.ItemType<PlasmaGrenade>()
            };

            flaskBombProjList = new List<int>()
            {
                ModContent.ProjectileType<BallisticPoisonBombProj>(),
                ModContent.ProjectileType<BlastBarrelProjectile>(),
                ModContent.ProjectileType<BouncingBettyProjectile>(),
                ModContent.ProjectileType<BrackishFlaskProj>(),
                ModContent.ProjectileType<DuststormInABottleProj>(),
                ModContent.ProjectileType<PlaguenadeProj>(),
                ModContent.ProjectileType<SeafoamBombProj>(),
                ModContent.ProjectileType<TotalityFlask>(),
                ModContent.ProjectileType<ConsecratedWaterProjectile>(),
                ModContent.ProjectileType<DesecratedWaterProj>(),
                ModContent.ProjectileType<PenumbraBomb>(),
                ModContent.ProjectileType<LatcherMineProjectile>(),
                ModContent.ProjectileType<SupernovaBomb>(),
                ModContent.ProjectileType<ShockGrenadeProjectile>(),
                ModContent.ProjectileType<ExorcismProj>(),
                ModContent.ProjectileType<MeteorFistProj>(),
                ModContent.ProjectileType<CraniumSmasherProj>(),
                ModContent.ProjectileType<CraniumSmasherExplosive>(),
                ModContent.ProjectileType<CraniumSmasherStealth>(),
                ModContent.ProjectileType<DestructionStar>(),
                ModContent.ProjectileType<DestructionBolt>(),
                ModContent.ProjectileType<ContaminatedBileFlask>(),
                ModContent.ProjectileType<GreenDonkeyKongReference>(),
                ModContent.ProjectileType<SkyfinNuke>(),
                ModContent.ProjectileType<SpentFuelContainerProjectile>(),
                ModContent.ProjectileType<SealedSingularityProj>(),
                ModContent.ProjectileType<PlasmaGrenadeProjectile>()
            };

            spikyBallList = new List<int>()
            {
                ModContent.ItemType<BouncySpikyBall>(),
                ModContent.ItemType<GodsParanoia>(),
                ModContent.ItemType<NastyCholla>(),
                ModContent.ItemType<HellsSun>(),
                ModContent.ItemType<SkyStabber>(),
                ModContent.ItemType<StickySpikyBall>(),
                ModContent.ItemType<WebBall>(),
                ModContent.ItemType<PoisonPack>(),
                ModContent.ItemType<Nychthemeron>(),
                ModContent.ItemType<MetalMonstrosity>(),
                ModContent.ItemType<BurningStrife>()
            };

            spikyBallProjList = new List<int>()
            {
                ModContent.ProjectileType<BouncyBol>(),
                ModContent.ProjectileType<GodsParanoiaProj>(),
                ModContent.ProjectileType<HellsSunProj>(),
                ModContent.ProjectileType<NastyChollaBol>(),
                ModContent.ProjectileType<StickyBol>(),
                ModContent.ProjectileType<SkyStabberProj>(),
                ModContent.ProjectileType<WebBallBol>(),
                ModContent.ProjectileType<PoisonBol>(),
                ModContent.ProjectileType<NychthemeronProjectile>(),
                ModContent.ProjectileType<MetalChunk>(),
                ModContent.ProjectileType<BurningStrifeProj>()
            };

            noGravityList = new List<int>()
            {
                ModContent.ItemType<AuricBar>(),
                ModContent.ItemType<EssenceofChaos>(),
                ModContent.ItemType<EssenceofCinder>(),
                ModContent.ItemType<EssenceofEleum>(),
                ModContent.ItemType<CoreofChaos>(),
                ModContent.ItemType<CoreofCinder>(),
                ModContent.ItemType<CoreofEleum>(),
                ModContent.ItemType<CoreofCalamity>(),
                ModContent.ItemType<CalamitousEssence>(),
                ModContent.ItemType<HellcasterFragment>(),
                ModContent.ItemType<TwistingNether>(),
                ModContent.ItemType<DarkPlasma>(),
                ModContent.ItemType<DarksunFragment>(),
                ModContent.ItemType<UnholyEssence>(),
                ModContent.ItemType<GalacticaSingularity>(),
                ModContent.ItemType<NightmareFuel>(),
                ModContent.ItemType<EndothermicEnergy>(),
                ModContent.ItemType<SoulofCryogen>(),

                ModContent.ItemType<KnowledgeAquaticScourge>(),
                ModContent.ItemType<KnowledgeAstralInfection>(),
                ModContent.ItemType<KnowledgeAstrumAureus>(),
                ModContent.ItemType<KnowledgeAstrumDeus>(),
                ModContent.ItemType<KnowledgeBloodMoon>(),
                ModContent.ItemType<KnowledgeBrainofCthulhu>(),
                ModContent.ItemType<KnowledgeBrimstoneCrag>(),
                ModContent.ItemType<KnowledgeBrimstoneElemental>(),
                ModContent.ItemType<KnowledgeBumblebirb>(),
                ModContent.ItemType<KnowledgeCalamitas>(),
                ModContent.ItemType<KnowledgeCalamitasClone>(),
                ModContent.ItemType<KnowledgeCorruption>(),
                ModContent.ItemType<KnowledgeCrabulon>(),
                ModContent.ItemType<KnowledgeCrimson>(),
                ModContent.ItemType<KnowledgeCryogen>(),
                ModContent.ItemType<KnowledgeDesertScourge>(),
                ModContent.ItemType<KnowledgeDestroyer>(),
                ModContent.ItemType<KnowledgeDevourerofGods>(),
                ModContent.ItemType<KnowledgeDukeFishron>(),
                ModContent.ItemType<KnowledgeEaterofWorlds>(),
                ModContent.ItemType<KnowledgeEyeofCthulhu>(),
                ModContent.ItemType<KnowledgeGolem>(),
                ModContent.ItemType<KnowledgeHiveMind>(),
                ModContent.ItemType<KnowledgeKingSlime>(),
                ModContent.ItemType<KnowledgeLeviathanandSiren>(),
                ModContent.ItemType<KnowledgeLunaticCultist>(),
                ModContent.ItemType<KnowledgeMechs>(),
                ModContent.ItemType<KnowledgeMoonLord>(),
                ModContent.ItemType<KnowledgeOcean>(),
                ModContent.ItemType<KnowledgeOldDuke>(),
                ModContent.ItemType<KnowledgePerforators>(),
                ModContent.ItemType<KnowledgePlaguebringerGoliath>(),
                ModContent.ItemType<KnowledgePlantera>(),
                ModContent.ItemType<KnowledgePolterghast>(),
                ModContent.ItemType<KnowledgeProfanedGuardians>(),
                ModContent.ItemType<KnowledgeProvidence>(),
                ModContent.ItemType<KnowledgeQueenBee>(),
                ModContent.ItemType<KnowledgeRavager>(),
                ModContent.ItemType<KnowledgeSentinels>(),
                ModContent.ItemType<KnowledgeSkeletron>(),
                ModContent.ItemType<KnowledgeSkeletronPrime>(),
                ModContent.ItemType<KnowledgeSlimeGod>(),
                ModContent.ItemType<KnowledgeSulphurSea>(),
                ModContent.ItemType<KnowledgeTwins>(),
                ModContent.ItemType<KnowledgeUnderworld>(),
                ModContent.ItemType<KnowledgeWallofFlesh>(),
                ModContent.ItemType<KnowledgeYharon>(),
            };

            lavaFishList = new List<int>()
            {
                ModContent.ItemType<SlurperPole>(),
                ModContent.ItemType<ChaoticSpreadRod>(),
                ModContent.ItemType<TheDevourerofCods>()
            };

            highTestFishList = new List<int>()
            {
                ItemID.GoldenFishingRod,
                ModContent.ItemType<EarlyBloomRod>(),
                ModContent.ItemType<TheDevourerofCods>()
            };

            flamethrowerList = new List<int>()
            {
                ModContent.ItemType<DragoonDrizzlefish>(),
                ModContent.ItemType<BloodBoiler>()
            };

            forceItemList = new List<int>()
            {
                ModContent.ItemType<SubmarineShocker>(),
                ModContent.ItemType<Barinautical>(),
                ModContent.ItemType<Downpour>(),
                ModContent.ItemType<DeepseaStaff>(),
                ModContent.ItemType<ScourgeoftheSeas>(),
                ModContent.ItemType<InsidiousImpaler>(),
                ModContent.ItemType<SepticSkewer>(),
                ModContent.ItemType<FetidEmesis>(),
                ModContent.ItemType<VitriolicViper>(),
                ModContent.ItemType<CadaverousCarrion>(),
                ModContent.ItemType<ToxicantTwister>(),
                ModContent.ItemType<DukeScales>(),
                ModContent.ItemType<Greentide>(),
                ModContent.ItemType<Leviatitan>(),
                ModContent.ItemType<Atlantis>(),
                ModContent.ItemType<SirensSong>(),
                ModContent.ItemType<BrackishFlask>(),
                ModContent.ItemType<LeviathanTeeth>(),
                ModContent.ItemType<GastricBelcherStaff>(),
                ModContent.ItemType<LureofEnthrallment>(),
                ModContent.ItemType<AquaticScourgeBag>(),
                ModContent.ItemType<OldDukeBag>(),
                ModContent.ItemType<LeviathanBag>(),
                ModContent.ItemType<OldDukeMask>(),
                ModContent.ItemType<LeviathanMask>(),
                ModContent.ItemType<AquaticScourgeMask>(),
                ModContent.ItemType<OldDukeTrophy>(),
                ModContent.ItemType<LeviathanTrophy>(),
                ModContent.ItemType<AquaticScourgeTrophy>(),
                ModContent.ItemType<KnowledgeAquaticScourge>(),
                ModContent.ItemType<KnowledgeLeviathanandSiren>(),
                ModContent.ItemType<KnowledgeSulphurSea>(),
                ModContent.ItemType<KnowledgeOcean>(),
                ModContent.ItemType<KnowledgeOldDuke>(),
                ModContent.ItemType<VictoryShard>(),
                ModContent.ItemType<AeroStone>(),
                ModContent.ItemType<DukesDecapitator>(),
				ModContent.ItemType<SulphurousSand>(),
				ModContent.ItemType<MagnumRounds>(),
				ModContent.ItemType<GrenadeRounds>(),
				ModContent.ItemType<ExplosiveShells>(),
				ItemID.HotlineFishingHook,
				ItemID.BottomlessBucket,
				ItemID.SuperAbsorbantSponge,
				ItemID.FishingPotion,
				ItemID.SonarPotion,
				ItemID.CratePotion,
				ItemID.AnglerTackleBag,
				ItemID.HighTestFishingLine,
				ItemID.TackleBox,
				ItemID.AnglerEarring,
				ItemID.FishermansGuide,
				ItemID.WeatherRadio,
				ItemID.Sextant,
				ItemID.AnglerHat,
				ItemID.AnglerVest,
				ItemID.AnglerPants,
				ItemID.GoldenBugNet,
				ItemID.FishronWings,
				ItemID.Flairon,
				ItemID.Tsunami,
				ItemID.BubbleGun,
				ItemID.RazorbladeTyphoon,
				ItemID.TempestStaff,
				ItemID.FishronBossBag,
				ItemID.Coral,
				ItemID.Seashell,
				ItemID.Starfish,
				ItemID.SoulofSight,
				ItemID.GreaterHealingPotion,
				ItemID.SuperHealingPotion
            };

			livingFireBlockList = new List<int>()
			{
				ModContent.TileType<LivingGodSlayerFireBlockTile>(),
				ModContent.TileType<LivingHolyFireBlockTile>(),
				ModContent.TileType<LivingBrimstoneFireBlockTile>(),
				TileID.LivingFire,
				TileID.LivingCursedFire,
				TileID.LivingDemonFire,
				TileID.LivingFrostFire,
				TileID.LivingIchor,
				TileID.LivingUltrabrightFire
			};

			zombieList = new List<int>()
			{
				NPCID.Zombie,
				NPCID.ArmedZombie,
				NPCID.BaldZombie,
				NPCID.PincushionZombie,
				NPCID.ArmedZombiePincussion, // what is this spelling
				NPCID.SlimedZombie,
				NPCID.ArmedZombieSlimed,
				NPCID.SwampZombie,
				NPCID.ArmedZombieSwamp,
				NPCID.TwiggyZombie,
				NPCID.ArmedZombieTwiggy,
				NPCID.FemaleZombie,
				NPCID.ArmedZombieCenx,
				NPCID.ZombieRaincoat,
				NPCID.ZombieEskimo,
				NPCID.ArmedZombieEskimo,
				NPCID.BigRainZombie,
				NPCID.SmallRainZombie,
				NPCID.BigFemaleZombie,
				NPCID.SmallFemaleZombie,
				NPCID.BigTwiggyZombie,
				NPCID.SmallTwiggyZombie,
				NPCID.BigSwampZombie,
				NPCID.SmallSwampZombie,
				NPCID.BigSlimedZombie,
				NPCID.SmallSlimedZombie,
				NPCID.BigPincushionZombie,
				NPCID.SmallPincushionZombie,
				NPCID.BigBaldZombie,
				NPCID.SmallBaldZombie,
				NPCID.BigZombie,
				NPCID.SmallZombie
				// halloween zombies not included because they don't drop shackles or zombie arms
			};

			demonEyeList = new List<int>()
			{
				NPCID.DemonEye,
				NPCID.CataractEye,
				NPCID.SleepyEye,
				NPCID.DialatedEye, // it is spelled "dilated"
				NPCID.GreenEye,
				NPCID.PurpleEye,
				NPCID.DemonEyeOwl,
				NPCID.DemonEyeSpaceship,
				NPCID.DemonEye2,
				NPCID.PurpleEye2,
				NPCID.GreenEye2,
				NPCID.DialatedEye2,
				NPCID.SleepyEye2,
				NPCID.CataractEye2
			};

			skeletonList = new List<int>()
			{
				NPCID.Skeleton,
				NPCID.HeadacheSkeleton,
				NPCID.MisassembledSkeleton,
				NPCID.PantlessSkeleton,
				NPCID.BoneThrowingSkeleton,
				NPCID.BoneThrowingSkeleton2,
				NPCID.BoneThrowingSkeleton3,
				NPCID.BoneThrowingSkeleton4,
				NPCID.BigPantlessSkeleton,
				NPCID.SmallPantlessSkeleton,
				NPCID.BigMisassembledSkeleton,
				NPCID.SmallMisassembledSkeleton,
				NPCID.BigHeadacheSkeleton,
				NPCID.SmallHeadacheSkeleton,
				NPCID.BigSkeleton,
				NPCID.SmallSkeleton,

				//Note: These skeletons don't count for Skeleton Banner for some god forsaken reason
				NPCID.SkeletonTopHat,
				NPCID.SkeletonAstonaut,
				NPCID.SkeletonAlien,

				//Other skeleton types
				NPCID.ArmoredSkeleton,
				NPCID.HeavySkeleton,
				NPCID.SkeletonArcher,
				NPCID.GreekSkeleton
			};

			angryBonesList = new List<int>()
			{
				NPCID.AngryBones,
				NPCID.AngryBonesBig,
				NPCID.AngryBonesBigMuscle,
				NPCID.AngryBonesBigHelmet,
				NPCID.BigBoned,
				NPCID.ShortBones
			};

			hornetList = new List<int>()
			{
				NPCID.BigHornetStingy,
				NPCID.LittleHornetStingy,
				NPCID.BigHornetSpikey,
				NPCID.LittleHornetSpikey,
				NPCID.BigHornetLeafy,
				NPCID.LittleHornetLeafy,
				NPCID.BigHornetHoney,
				NPCID.LittleHornetHoney,
				NPCID.BigHornetFatty,
				NPCID.LittleHornetFatty,
				NPCID.BigStinger,
				NPCID.LittleStinger,
				NPCID.Hornet,
				NPCID.HornetFatty,
				NPCID.HornetHoney,
				NPCID.HornetLeafy,
				NPCID.HornetSpikey,
				NPCID.HornetStingy
			};

			mossHornetList = new List<int>()
			{
				NPCID.MossHornet,
				NPCID.TinyMossHornet,
				NPCID.LittleMossHornet,
				NPCID.BigMossHornet,
				NPCID.GiantMossHornet
			};

            bossMinionList = new List<int>()
            {
                ModContent.NPCType<DesertScourgeHeadSmall>(),
                ModContent.NPCType<DesertScourgeBodySmall>(),
                ModContent.NPCType<DesertScourgeTailSmall>(),
                NPCID.SlimeSpiked,
                NPCID.ServantofCthulhu,
                ModContent.NPCType<CrabShroom>(),
                NPCID.EaterofWorldsHead,
                NPCID.EaterofWorldsBody,
                NPCID.EaterofWorldsTail,
                NPCID.Creeper,
                ModContent.NPCType<PerforatorHeadSmall>(),
                ModContent.NPCType<PerforatorBodySmall>(),
                ModContent.NPCType<PerforatorTailSmall>(),
                ModContent.NPCType<PerforatorHeadMedium>(),
                ModContent.NPCType<PerforatorBodyMedium>(),
                ModContent.NPCType<PerforatorTailMedium>(),
                ModContent.NPCType<PerforatorHeadLarge>(),
                ModContent.NPCType<PerforatorBodyLarge>(),
                ModContent.NPCType<PerforatorTailLarge>(),
                ModContent.NPCType<HiveBlob>(),
                ModContent.NPCType<HiveBlob2>(),
                ModContent.NPCType<DankCreeper>(),
                NPCID.SkeletronHand,
                ModContent.NPCType<SlimeGod>(),
                ModContent.NPCType<SlimeGodSplit>(),
                ModContent.NPCType<SlimeGodRun>(),
                ModContent.NPCType<SlimeGodRunSplit>(),
                ModContent.NPCType<SlimeSpawnCorrupt>(),
                ModContent.NPCType<SlimeSpawnCorrupt2>(),
                ModContent.NPCType<SlimeSpawnCrimson>(),
                ModContent.NPCType<SlimeSpawnCrimson2>(),
                NPCID.LeechHead,
                NPCID.LeechBody,
                NPCID.LeechTail,
                NPCID.WallofFleshEye,
                NPCID.TheHungry,
                NPCID.TheHungryII,
                ModContent.NPCType<Cryocore>(),
                ModContent.NPCType<Cryocore2>(),
                ModContent.NPCType<IceMass>(),
                NPCID.PrimeCannon,
                NPCID.PrimeLaser,
                NPCID.PrimeSaw,
                NPCID.PrimeVice,
                ModContent.NPCType<Brimling>(),
                NPCID.TheDestroyer,
                NPCID.TheDestroyerBody,
                NPCID.TheDestroyerTail,
                ModContent.NPCType<AquaticScourgeHead>(),
                ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(),
                ModContent.NPCType<AquaticScourgeTail>(),
                ModContent.NPCType<CalamitasRun>(),
                ModContent.NPCType<CalamitasRun2>(),
                ModContent.NPCType<LifeSeeker>(),
                ModContent.NPCType<SoulSeeker>(),
                NPCID.PlanterasTentacle,
                ModContent.NPCType<AureusSpawn>(),
                NPCID.Spore,
                NPCID.GolemHead,
                NPCID.GolemHeadFree,
                NPCID.GolemFistLeft,
                NPCID.GolemFistRight,
                ModContent.NPCType<PlagueMine>(),
                ModContent.NPCType<PlagueHomingMissile>(),
                ModContent.NPCType<RavagerClawLeft>(),
                ModContent.NPCType<RavagerClawRight>(),
                ModContent.NPCType<RavagerLegLeft>(),
                ModContent.NPCType<RavagerLegLeft>(),
                ModContent.NPCType<RavagerHead>(),
                NPCID.CultistDragonHead,
                NPCID.CultistDragonBody1,
                NPCID.CultistDragonBody2,
                NPCID.CultistDragonBody3,
                NPCID.CultistDragonBody4,
                NPCID.CultistDragonTail,
                NPCID.AncientCultistSquidhead,
                NPCID.MoonLordFreeEye,
                NPCID.MoonLordHand,
                NPCID.MoonLordHead,
                ModContent.NPCType<Bumblefuck2>(),
                ModContent.NPCType<ProvSpawnOffense>(),
                ModContent.NPCType<ProvSpawnDefense>(),
                ModContent.NPCType<ProvSpawnHealer>(),
                ModContent.NPCType<DarkEnergy>(),
                ModContent.NPCType<DarkEnergy2>(),
                ModContent.NPCType<DarkEnergy3>(),
                ModContent.NPCType<CosmicLantern>(),
                ModContent.NPCType<StasisProbe>(),
                ModContent.NPCType<StasisProbeNaked>(),
                ModContent.NPCType<DevourerofGodsHead2>(),
                ModContent.NPCType<DevourerofGodsBody2>(),
                ModContent.NPCType<DevourerofGodsTail2>(),
                ModContent.NPCType<DetonatingFlare>(),
                ModContent.NPCType<DetonatingFlare2>(),
                ModContent.NPCType<SupremeCataclysm>(),
                ModContent.NPCType<SupremeCatastrophe>()
            };

            Mod thorium = ModLoader.GetMod("ThoriumMod");
            if (CalamityConfig.Instance.BuffThoriumBosses && thorium != null)
            {
                enemyImmunityList.Add(thorium.NPCType("TheGrandThunderBirdv2"));
                enemyImmunityList.Add(thorium.NPCType("QueenJelly"));
                enemyImmunityList.Add(thorium.NPCType("Viscount"));
                enemyImmunityList.Add(thorium.NPCType("GraniteEnergyStorm"));
                enemyImmunityList.Add(thorium.NPCType("TheBuriedWarrior"));
                enemyImmunityList.Add(thorium.NPCType("ThePrimeScouter"));
                enemyImmunityList.Add(thorium.NPCType("BoreanStrider"));
                enemyImmunityList.Add(thorium.NPCType("BoreanStriderPopped"));
                enemyImmunityList.Add(thorium.NPCType("FallenDeathBeholder"));
                enemyImmunityList.Add(thorium.NPCType("FallenDeathBeholder2"));
                enemyImmunityList.Add(thorium.NPCType("Lich"));
                enemyImmunityList.Add(thorium.NPCType("LichHeadless"));
                enemyImmunityList.Add(thorium.NPCType("Abyssion"));
                enemyImmunityList.Add(thorium.NPCType("AbyssionCracked"));
                enemyImmunityList.Add(thorium.NPCType("AbyssionReleased"));
                enemyImmunityList.Add(thorium.NPCType("SlagFury"));
                enemyImmunityList.Add(thorium.NPCType("Omnicide"));
                enemyImmunityList.Add(thorium.NPCType("RealityBreaker"));
                enemyImmunityList.Add(thorium.NPCType("Aquaius"));
                enemyImmunityList.Add(thorium.NPCType("Aquaius2"));

                revengeanceEnemyBuffList25Percent.Add(thorium.NPCType("TheGrandThunderBirdv2"));
				revengeanceEnemyBuffList25Percent.Add(thorium.NPCType("QueenJelly"));
				revengeanceEnemyBuffList20Percent.Add(thorium.NPCType("Viscount"));
				revengeanceEnemyBuffList20Percent.Add(thorium.NPCType("GraniteEnergyStorm"));
				revengeanceEnemyBuffList20Percent.Add(thorium.NPCType("TheBuriedWarrior"));
				revengeanceEnemyBuffList20Percent.Add(thorium.NPCType("ThePrimeScouter"));
				revengeanceEnemyBuffList20Percent.Add(thorium.NPCType("BoreanStrider"));
				revengeanceEnemyBuffList20Percent.Add(thorium.NPCType("BoreanStriderPopped"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("FallenDeathBeholder"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("FallenDeathBeholder2"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("Lich"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("LichHeadless"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("Abyssion"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("AbyssionCracked"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("AbyssionReleased"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("SlagFury"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("Omnicide"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("RealityBreaker"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("Aquaius"));
				revengeanceEnemyBuffList15Percent.Add(thorium.NPCType("Aquaius2"));

				revengeanceProjectileBuffList25Percent.Add(thorium.ProjectileType("GrandThunderBirdZap"));
				revengeanceProjectileBuffList25Percent.Add(thorium.ProjectileType("ThunderGust"));
				revengeanceProjectileBuffList25Percent.Add(thorium.ProjectileType("BubbleBomb"));
				revengeanceProjectileBuffList25Percent.Add(thorium.ProjectileType("QueenJellyArm"));
				revengeanceProjectileBuffList25Percent.Add(thorium.ProjectileType("QueenTorrent"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("ViscountRipple"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("ViscountRipple2"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("ViscountBlood"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("ViscountStomp"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("ViscountStomp2"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("ViscountRockFall"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("GraniteCharge"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("BuriedShock"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("BuriedDagger"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("BuriedArrow"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("BuriedArrow2"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("BuriedArrowF"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("BuriedArrowP"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("BuriedArrowC"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("BuriedMagic"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("BuriedMagicPop"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("MainBeamOuter"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("MainBeam"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("MainBeamCheese"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("VaporizeBlast"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("GravitonSurge"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("Vaporize"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("GravitonCharge"));
				revengeanceProjectileBuffList20Percent.Add(thorium.ProjectileType("GravitySpark"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("DoomBeholderBeam"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("VoidLaserPro"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("BeholderBeam"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("BlizzardBarrage"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("FrostSurge"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("FrostSurgeR"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("BlizzardCascade"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("BlizzardBoom"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("BlizzardFang"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("FrostMytePro"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("IceAnomaly"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("LichGaze"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("LichGazeB"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("LichFlareSpawn"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("LichFlare"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("LichPulse"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("LichMatter"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("SoulRenderLich"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("LichFlareDeathD"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("LichFlareDeathU"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("Whirlpool"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("AbyssionSpit"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("AbyssionSpit2"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("AquaRipple"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("AbyssalStrike2"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("OldGodSpit"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("OldGodSpit2"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("WaterPulse"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("TyphoonBlastHostile"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("TyphoonBlastHostileSmaller"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("AquaBarrage"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("DeathRaySpawnR"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("DeathRaySpawnL"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("DeathRaySpawn"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("OmniDeath"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("OmniSphereOrb"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("FlameLash"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("FlamePulse"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("FlamePulseTorn"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("FlameNova"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("MoltenFury"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("RealityFury"));
				revengeanceProjectileBuffList15Percent.Add(thorium.ProjectileType("UFOBlast"));
            }

            legOverrideList = new List<int>()
            {
                Instance.GetEquipSlot("ProviLegs", EquipType.Legs),
                Instance.GetEquipSlot("SirenLegAlt", EquipType.Legs),
                Instance.GetEquipSlot("SirenLeg", EquipType.Legs),
                Instance.GetEquipSlot("PopoLeg", EquipType.Legs)
            };
        }
        #endregion

        #region Vanilla Enemy DR
        private void SetupVanillaDR()
        {
            DRValues = new SortedDictionary<int, float> {
                { NPCID.AngryBonesBig, 0.2f },
                { NPCID.AngryBonesBigHelmet, 0.2f },
                { NPCID.AngryBonesBigMuscle, 0.2f },
                { NPCID.AnomuraFungus, 0.1f },
                { NPCID.Antlion, 0.1f },
                { NPCID.Arapaima, 0.1f },
                { NPCID.ArmoredSkeleton, 0.15f },
                { NPCID.ArmoredViking, 0.1f },
                { NPCID.BigMimicCorruption, 0.3f },
                { NPCID.BigMimicCrimson, 0.3f },
                { NPCID.BigMimicHallow, 0.3f },
                { NPCID.BigMimicJungle, 0.3f }, // unused vanilla enemy
                { NPCID.BlueArmoredBones, 0.2f },
                { NPCID.BlueArmoredBonesMace, 0.2f },
                { NPCID.BlueArmoredBonesNoPants, 0.2f },
                { NPCID.BlueArmoredBonesSword, 0.2f },
                { NPCID.BoneLee, 0.2f },
                { NPCID.Crab, 0.05f },
                { NPCID.Crawdad, 0.2f },
                { NPCID.Crawdad2, 0.2f },
                { NPCID.CultistBoss, 0.1f },
                { NPCID.DD2Betsy, 0.1f },
                { NPCID.DD2OgreT2, 0.1f },
                { NPCID.DD2OgreT3, 0.15f },
                { NPCID.DeadlySphere, 0.4f },
                { NPCID.DiabolistRed, 0.2f },
                { NPCID.DiabolistWhite, 0.2f },
				{ NPCID.DukeFishron, 0.1f },
                { NPCID.DungeonGuardian, 0.999999f },
                { NPCID.DungeonSpirit, 0.2f },
                { NPCID.ElfCopter, 0.15f },
                { NPCID.Everscream, 0.1f },
                { NPCID.FlyingAntlion, 0.05f },
                { NPCID.GiantCursedSkull, 0.2f },
                { NPCID.GiantShelly, 0.2f },
                { NPCID.GiantShelly2, 0.2f },
                { NPCID.GiantTortoise, 0.35f },
                { NPCID.Golem, 0.25f },
                { NPCID.GolemFistLeft, 0.25f },
                { NPCID.GolemFistRight, 0.25f },
                { NPCID.GolemHead, 0.25f },
                { NPCID.GolemHeadFree, 0.25f },
                { NPCID.GraniteFlyer, 0.1f },
                { NPCID.GraniteGolem, 0.15f },
                { NPCID.GreekSkeleton, 0.1f },
                { NPCID.HellArmoredBones, 0.2f },
                { NPCID.HellArmoredBonesMace, 0.2f },
                { NPCID.HellArmoredBonesSpikeShield, 0.2f },
                { NPCID.HellArmoredBonesSword, 0.2f },
                { NPCID.IceGolem, 0.1f },
                { NPCID.IceQueen, 0.1f },
                { NPCID.IceTortoise, 0.35f },
                { NPCID.HeadlessHorseman, 0.05f },
                { NPCID.MartianDrone, 0.2f },
                { NPCID.MartianSaucer, 0.2f },
                { NPCID.MartianSaucerCannon, 0.2f },
                { NPCID.MartianSaucerCore, 0.2f },
                { NPCID.MartianSaucerTurret, 0.2f },
                { NPCID.MartianTurret, 0.2f },
                { NPCID.MartianWalker, 0.35f },
                { NPCID.Mimic, 0.3f },
                { NPCID.MoonLordCore, 0.05f },
                { NPCID.MoonLordHand, 0.05f },
                { NPCID.MoonLordHead, 0.05f },
                { NPCID.Mothron, 0.2f },
                { NPCID.MothronEgg, 0.5f },
                { NPCID.MourningWood, 0.1f },
                { NPCID.Necromancer, 0.2f },
                { NPCID.NecromancerArmored, 0.2f },
                { NPCID.Paladin, 0.45f },
                { NPCID.PirateCaptain, 0.05f },
                { NPCID.PirateShipCannon, 0.15f },
				{ NPCID.Plantera, 0.15f },
				{ NPCID.PlanterasTentacle, 0.1f },
				{ NPCID.PossessedArmor, 0.25f },
                { NPCID.PresentMimic, 0.3f },
                { NPCID.PrimeCannon, 0.2f },
                { NPCID.PrimeLaser, 0.2f },
                { NPCID.PrimeSaw, 0.2f },
                { NPCID.PrimeVice, 0.2f },
                { NPCID.Probe, 0.2f },
                { NPCID.Pumpking, 0.1f },
                { NPCID.QueenBee, 0.05f },
                { NPCID.RaggedCaster, 0.2f },
                { NPCID.RaggedCasterOpenCoat, 0.2f },
                { NPCID.Retinazer, 0.2f },
                { NPCID.RustyArmoredBonesAxe, 0.2f },
                { NPCID.RustyArmoredBonesFlail, 0.2f },
                { NPCID.RustyArmoredBonesSword, 0.2f },
                { NPCID.RustyArmoredBonesSwordNoArmor, 0.2f },
                { NPCID.SandElemental, 0.1f },
                { NPCID.SantaNK1, 0.35f },
                { NPCID.SeaSnail, 0.05f },
                { NPCID.SkeletonArcher, 0.1f },
                { NPCID.SkeletonCommando, 0.2f },
                { NPCID.SkeletonSniper, 0.2f },
                { NPCID.SkeletronHand, 0.05f },
                { NPCID.SkeletronHead, 0.05f },
                { NPCID.SkeletronPrime, 0.2f },
                { NPCID.Spazmatism, 0.2f },
                { NPCID.TacticalSkeleton, 0.2f },
                { NPCID.TheDestroyer, 0.1f },
                { NPCID.TheDestroyerBody, 0.2f },
                { NPCID.TheDestroyerTail, 0.35f },
                { NPCID.TheHungry, 0.1f },
                { NPCID.UndeadViking, 0.1f },
                { NPCID.WalkingAntlion, 0.1f },
                { NPCID.WallofFlesh, 0.5f },
            };
        }
        #endregion

        #region Thorium Boss DR
        private void SetupThoriumBossDR(Mod thorium)
        {
            if (thorium is null || !CalamityConfig.Instance.BuffThoriumBosses)
                return;

            void ThoriumDR(string npcName, float dr) {
                int type = thorium.NPCType(npcName);
                if (DRValues.ContainsKey(type))
                    DRValues[type] = dr;
                else
                    DRValues.Add(type, dr);
            };

            ThoriumDR("Viscount", 0.05f);
            ThoriumDR("BoreanStrider", 0.05f);
            ThoriumDR("FallenDeathBeholder", 0.05f);
            ThoriumDR("Lich", 0.05f);
            ThoriumDR("AbyssionReleased", 0.05f);
            ThoriumDR("CryoCore", 0.1f);
            ThoriumDR("BioCore", 0.1f);
            ThoriumDR("PyroCore", 0.1f);
            ThoriumDR("Aquaius", 0.1f);
            ThoriumDR("ThePrimeScouter", 0.2f);
            ThoriumDR("FallenDeathBeholder2", 0.2f);
            ThoriumDR("SlagFury", 0.2f);
            ThoriumDR("Aquaius2", 0.2f);
            ThoriumDR("GraniteEnergyStorm", 0.2f);
            ThoriumDR("TheBuriedWarrior", 0.25f);
            ThoriumDR("TheBuriedWarrior1", 0.25f);
            ThoriumDR("TheBuriedWarrior2", 0.25f);
            ThoriumDR("LichHeadless", 0.25f);
            ThoriumDR("AbyssionCracked", 0.25f);
            ThoriumDR("Omnicide", 0.3f);
            ThoriumDR("Abyssion", 0.35f);
        }
		#endregion

		#region Boss Kill Times
		private void SetupBossKillTimes()
		{
			// 3600 = 1 minute

			bossKillTimes = new SortedDictionary<int, int> {
				{ NPCID.KingSlime, 3600 },
				{ NPCID.EyeofCthulhu, 5400 },
				{ NPCID.EaterofWorldsHead, 7200 },
				{ NPCID.EaterofWorldsBody, 7200 },
				{ NPCID.EaterofWorldsTail, 7200 },
				{ NPCID.BrainofCthulhu, 5400 },
				{ NPCID.Creeper, 1800 },
				{ NPCID.QueenBee, 7200 },
				{ NPCID.SkeletronHead, 9000 },
				{ NPCID.WallofFlesh, 7200 },
				{ NPCID.WallofFleshEye, 7200 },
				{ NPCID.Spazmatism, 10800 },
				{ NPCID.Retinazer, 10800 },
				{ NPCID.TheDestroyer, 10800 },
                { NPCID.TheDestroyerBody, 10800 },
				{ NPCID.TheDestroyerTail, 10800 },
				{ NPCID.SkeletronPrime, 10800 },
				{ NPCID.Plantera, 10800 },
				{ NPCID.Golem, 9000 },
				{ NPCID.GolemHead, 3600 },
				{ NPCID.DukeFishron, 9000 },
				{ NPCID.CultistBoss, 9000 },
				{ NPCID.MoonLordCore, 14400 },
				{ NPCID.MoonLordHand, 7200 },
				{ NPCID.MoonLordHead, 7200 },
				{ ModContent.NPCType<DesertScourgeHead>(), 3600 },
				{ ModContent.NPCType<DesertScourgeBody>(), 3600 },
				{ ModContent.NPCType<DesertScourgeTail>(), 3600 },
				{ ModContent.NPCType<CrabulonIdle>(), 5400 },
				{ ModContent.NPCType<HiveMind>(), 1800 },
				{ ModContent.NPCType<HiveMindP2>(), 5400 },
				{ ModContent.NPCType<PerforatorHive>(), 7200 },
				{ ModContent.NPCType<SlimeGodCore>(), 10800 },
				{ ModContent.NPCType<SlimeGod>(), 3600 },
				{ ModContent.NPCType<SlimeGodRun>(), 3600 },
				{ ModContent.NPCType<SlimeGodSplit>(), 3600 },
				{ ModContent.NPCType<SlimeGodRunSplit>(), 3600 },
				{ ModContent.NPCType<Cryogen>(), 10800 },
				{ ModContent.NPCType<AquaticScourgeHead>(), 7200 },
				{ ModContent.NPCType<AquaticScourgeBody>(), 7200 },
				{ ModContent.NPCType<AquaticScourgeBodyAlt>(), 7200 },
				{ ModContent.NPCType<AquaticScourgeTail>(), 7200 },
				{ ModContent.NPCType<BrimstoneElemental>(), 10800 },
				{ ModContent.NPCType<Calamitas>(), 1200 },
				{ ModContent.NPCType<CalamitasRun3>(), 11400 },
				{ ModContent.NPCType<Leviathan>(), 10800 },
				{ ModContent.NPCType<Siren>(), 10800 },
				{ ModContent.NPCType<AstrumAureus>(), 10800 },
				{ ModContent.NPCType<AstrumDeusHeadSpectral>(), 7200 },
				{ ModContent.NPCType<AstrumDeusBodySpectral>(), 7200 },
				{ ModContent.NPCType<AstrumDeusTailSpectral>(), 7200 },
				{ ModContent.NPCType<PlaguebringerGoliath>(), 10800 },
				{ ModContent.NPCType<RavagerBody>(), 10800 },
				{ ModContent.NPCType<ProfanedGuardianBoss>(), 5400 },
				{ ModContent.NPCType<Bumblefuck>(), 7200 },
				{ ModContent.NPCType<Providence>(), 14400 },
				{ ModContent.NPCType<DarkEnergy>(), 1200 },
				{ ModContent.NPCType<DarkEnergy2>(), 1200 },
				{ ModContent.NPCType<DarkEnergy3>(), 1200 },
				{ ModContent.NPCType<StormWeaverHeadNaked>(), 5400 },
				{ ModContent.NPCType<StormWeaverBodyNaked>(), 5400 },
				{ ModContent.NPCType<StormWeaverTailNaked>(), 5400 },
				{ ModContent.NPCType<Signus>(), 7200 },
				{ ModContent.NPCType<Polterghast>(), 10800 },
				{ ModContent.NPCType<OldDuke>(), 10800 },
				{ ModContent.NPCType<DevourerofGodsHead>(), 5400 },
				{ ModContent.NPCType<DevourerofGodsBody>(), 5400 },
				{ ModContent.NPCType<DevourerofGodsTail>(), 5400 },
				{ ModContent.NPCType<DevourerofGodsHeadS>(), 9000 },
				{ ModContent.NPCType<DevourerofGodsBodyS>(), 9000 },
				{ ModContent.NPCType<DevourerofGodsTailS>(), 9000 },
				{ ModContent.NPCType<Yharon>(), 10800 },
				{ ModContent.NPCType<SupremeCalamitas>(), 18000 }
			};
		}
		#endregion

		#region Boss Velocity Contact Damage Scale Values
		private void SetupBossVelocityScalingValues()
		{
			bossVelocityDamageScaleValues = new SortedDictionary<int, float> {
				{ NPCID.KingSlime, velocityScaleMin },
				{ NPCID.EyeofCthulhu, velocityScaleMin }, // Increases in phase 2
				{ NPCID.EaterofWorldsHead, bitingEnemeyVelocityScale },
				{ NPCID.EaterofWorldsBody, velocityScaleMin },
				{ NPCID.EaterofWorldsTail, velocityScaleMin },
				{ NPCID.Creeper, velocityScaleMin },
				{ NPCID.BrainofCthulhu, velocityScaleMin },
				{ NPCID.QueenBee, velocityScaleMin },
				{ NPCID.SkeletronHead, velocityScaleMin },
				{ NPCID.SkeletronHand, velocityScaleMin },
				{ NPCID.TheHungry, bitingEnemeyVelocityScale },
				{ NPCID.TheHungryII, bitingEnemeyVelocityScale },
				{ NPCID.LeechHead, bitingEnemeyVelocityScale },
				{ NPCID.LeechBody, velocityScaleMin },
				{ NPCID.LeechTail, velocityScaleMin },
				{ NPCID.Spazmatism, velocityScaleMin }, // Increases in phase 2
				{ NPCID.Retinazer, velocityScaleMin },
				{ NPCID.TheDestroyer, bitingEnemeyVelocityScale },
				{ NPCID.TheDestroyerBody, velocityScaleMin },
				{ NPCID.TheDestroyerTail, velocityScaleMin },
				{ NPCID.Probe, velocityScaleMin },
				{ NPCID.SkeletronPrime, velocityScaleMin },
				{ NPCID.PrimeCannon, velocityScaleMin },
				{ NPCID.PrimeLaser, velocityScaleMin },
				{ NPCID.PrimeSaw, velocityScaleMin },
				{ NPCID.PrimeVice, velocityScaleMin },
				{ NPCID.Plantera, velocityScaleMin }, // Increases in phase 2
				{ NPCID.PlanterasTentacle, bitingEnemeyVelocityScale },
				{ NPCID.Golem, velocityScaleMin },
				{ NPCID.GolemFistLeft, velocityScaleMin },
				{ NPCID.GolemFistRight, velocityScaleMin },
				{ NPCID.GolemHead, velocityScaleMin },
				{ NPCID.DukeFishron, velocityScaleMin },
				{ ModContent.NPCType<DesertScourgeHead>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<DesertScourgeBody>(), velocityScaleMin },
				{ ModContent.NPCType<DesertScourgeTail>(), velocityScaleMin },
				{ ModContent.NPCType<DesertScourgeHeadSmall>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<DesertScourgeBodySmall>(), velocityScaleMin },
				{ ModContent.NPCType<DesertScourgeTailSmall>(), velocityScaleMin },
				{ ModContent.NPCType<CrabulonIdle>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<HiveMindP2>(), velocityScaleMin },
				{ ModContent.NPCType<PerforatorHive>(), velocityScaleMin },
				{ ModContent.NPCType<PerforatorHeadLarge>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<PerforatorBodyLarge>(), velocityScaleMin },
				{ ModContent.NPCType<PerforatorTailLarge>(), velocityScaleMin },
				{ ModContent.NPCType<PerforatorHeadMedium>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<PerforatorBodyMedium>(), velocityScaleMin },
				{ ModContent.NPCType<PerforatorTailMedium>(), velocityScaleMin },
				{ ModContent.NPCType<PerforatorHeadSmall>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<PerforatorBodySmall>(), velocityScaleMin },
				{ ModContent.NPCType<PerforatorTailSmall>(), velocityScaleMin },
				{ ModContent.NPCType<SlimeGodCore>(), velocityScaleMin },
				{ ModContent.NPCType<SlimeGod>(), velocityScaleMin },
				{ ModContent.NPCType<SlimeGodRun>(), velocityScaleMin },
				{ ModContent.NPCType<SlimeGodSplit>(), velocityScaleMin },
				{ ModContent.NPCType<SlimeGodRunSplit>(), velocityScaleMin },
				{ ModContent.NPCType<SlimeSpawnCorrupt>(), velocityScaleMin },
				{ ModContent.NPCType<Cryogen>(), velocityScaleMin },
				{ ModContent.NPCType<Cryocore>(), velocityScaleMin },
				{ ModContent.NPCType<Cryocore2>(), velocityScaleMin },
				{ ModContent.NPCType<IceMass>(), velocityScaleMin },
				{ ModContent.NPCType<AquaticScourgeHead>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<AquaticScourgeBody>(), velocityScaleMin },
				{ ModContent.NPCType<AquaticScourgeBodyAlt>(), velocityScaleMin },
				{ ModContent.NPCType<AquaticScourgeTail>(), velocityScaleMin },
				{ ModContent.NPCType<BrimstoneElemental>(), velocityScaleMin },
				{ ModContent.NPCType<Calamitas>(), velocityScaleMin },
				{ ModContent.NPCType<CalamitasRun3>(), velocityScaleMin },
				{ ModContent.NPCType<Leviathan>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<Siren>(), velocityScaleMin },
				{ ModContent.NPCType<AstrumAureus>(), velocityScaleMin },
				{ ModContent.NPCType<AstrumDeusHeadSpectral>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<AstrumDeusBodySpectral>(), velocityScaleMin },
				{ ModContent.NPCType<AstrumDeusTailSpectral>(), velocityScaleMin },
				{ ModContent.NPCType<PlaguebringerGoliath>(), velocityScaleMin },
				{ ModContent.NPCType<PlaguebringerShade>(), velocityScaleMin },
				{ ModContent.NPCType<PlagueBeeG>(), velocityScaleMin },
				{ ModContent.NPCType<PlagueBeeLargeG>(), velocityScaleMin },
				{ ModContent.NPCType<RavagerBody>(), velocityScaleMin },
				{ ModContent.NPCType<RavagerClawLeft>(), velocityScaleMin },
				{ ModContent.NPCType<RavagerClawRight>(), velocityScaleMin },
				{ ModContent.NPCType<RavagerLegLeft>(), velocityScaleMin },
				{ ModContent.NPCType<RavagerLegRight>(), velocityScaleMin },
				{ ModContent.NPCType<RockPillar>(), velocityScaleMin },
				{ ModContent.NPCType<ProfanedGuardianBoss>(), velocityScaleMin },
				{ ModContent.NPCType<ProfanedGuardianBoss2>(), velocityScaleMin },
				{ ModContent.NPCType<ProfanedGuardianBoss3>(), velocityScaleMin },
				{ ModContent.NPCType<Bumblefuck>(), velocityScaleMin },
				{ ModContent.NPCType<Bumblefuck2>(), velocityScaleMin },
				{ ModContent.NPCType<CeaselessVoid>(), velocityScaleMin },
				{ ModContent.NPCType<DarkEnergy>(), velocityScaleMin },
				{ ModContent.NPCType<DarkEnergy2>(), velocityScaleMin },
				{ ModContent.NPCType<DarkEnergy3>(), velocityScaleMin },
				{ ModContent.NPCType<StormWeaverHead>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<StormWeaverBody>(), velocityScaleMin },
				{ ModContent.NPCType<StormWeaverTail>(), velocityScaleMin },
				{ ModContent.NPCType<StormWeaverHeadNaked>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<StormWeaverBodyNaked>(), velocityScaleMin },
				{ ModContent.NPCType<StormWeaverTailNaked>(), velocityScaleMin },
				{ ModContent.NPCType<StasisProbe>(), velocityScaleMin },
				{ ModContent.NPCType<StasisProbeNaked>(), velocityScaleMin },
				{ ModContent.NPCType<Signus>(), velocityScaleMin },
				{ ModContent.NPCType<CosmicLantern>(), velocityScaleMin },
				{ ModContent.NPCType<Polterghast>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<PolterPhantom>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<OldDuke>(), velocityScaleMin },
				{ ModContent.NPCType<DevourerofGodsHead>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<DevourerofGodsBody>(), velocityScaleMin },
				{ ModContent.NPCType<DevourerofGodsTail>(), velocityScaleMin },
				{ ModContent.NPCType<DevourerofGodsHead2>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<DevourerofGodsBody2>(), velocityScaleMin },
				{ ModContent.NPCType<DevourerofGodsTail2>(), velocityScaleMin },
				{ ModContent.NPCType<DevourerofGodsHeadS>(), bitingEnemeyVelocityScale },
				{ ModContent.NPCType<DevourerofGodsBodyS>(), velocityScaleMin },
				{ ModContent.NPCType<DevourerofGodsTailS>(), velocityScaleMin },
				{ ModContent.NPCType<Yharon>(), velocityScaleMin },
				{ ModContent.NPCType<DetonatingFlare>(), velocityScaleMin },
				{ ModContent.NPCType<DetonatingFlare2>(), velocityScaleMin },
				{ ModContent.NPCType<SupremeCalamitas>(), velocityScaleMin }
			};
		}
		#endregion

		#region Music
		public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (Main.musicVolume != 0)
            {
                if (Main.myPlayer != -1 && !Main.gameMenu && Main.LocalPlayer.active)
                {
                    Player p = Main.LocalPlayer;
                    if (p.InCalamity())
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Crag");
                            else
                                music = MusicID.Eerie;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InSunkenSea())
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/SunkenSea");
                            else
                                music = MusicID.Temple;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InAstral(1))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                            {
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Astral");
                            }
                            else
                                music = MusicID.Space;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InAstral(2))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                            {
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/AstralUnderground");
                            }
                            else
                                music = MusicID.Space;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InAbyss(1) || p.InAbyss(2))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/TheAbyss");
                            else
                                music = MusicID.Hell;
                            priority = MusicPriority.BiomeHigh;
                        }
                    }
                    if (p.InAbyss(3))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/TheDeepAbyss");
                            else
                                music = MusicID.Hell;
                            priority = MusicPriority.BiomeHigh;
                        }
                    }
                    if (p.InAbyss(4))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/TheVoid");
                            else
                                music = MusicID.Hell;
                            priority = MusicPriority.BiomeHigh;
                        }
                    }
                    if (p.InSulphur())
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            bool acidRain = CalamityWorld.rainingAcid;
                            if (calamityModMusic != null)
                            {
	                            string rainMusic = "Sounds/Music/AcidRain";
	                            string musicChoice = acidRain ? rainMusic + (CalamityWorld.downedPolterghast ? "2" : "1") : "Sounds/Music/Sulphur"; //replace first acidrain1 once second theme is added.
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, musicChoice);
                                
                            }
                            else
                                music = acidRain ? CalamityWorld.downedPolterghast ? MusicID.Eclipse : MusicID.OldOnesArmy : MusicID.Desert; //if you have a better choice of music, feel free to change, it was pretty random choosing ngl
                            priority = acidRain ? MusicPriority.Event : MusicPriority.BiomeHigh;
                        }
                    }
                    if (CalamityWorld.DoGSecondStageCountdown <= 540 && CalamityWorld.DoGSecondStageCountdown > 60) //8 seconds before DoG spawns
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/UniversalCollapse");
                            else
                                music = MusicID.LunarBoss;
                            priority = MusicPriority.BossMedium;
                        }
                    }
                }
            }
        }
        #endregion

        #region ModSupport
        public override void PostSetupContent() => WeakReferenceSupport.Setup();

        public override object Call(params object[] args) => ModCalls.Call(args);
        #endregion

        #region DrawingStuff
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (mouseIndex != -1)
            {
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Boss HP Bars", delegate ()
                {
                    if (Main.LocalPlayer.Calamity().drawBossHPBar)
                    {
                        BossHealthBarManager.Update();
                        BossHealthBarManager.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.None));

                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Draedon Hologram", () =>
                {
                    DraedonHologramChatUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));

                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Draedon Factory Tiles", () =>
                {
                    DraedonsFactoryUI.Draw(Main.spriteBatch);
                    DraedonsItemChargerUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.Game)); // InterfaceScaleType.Game tells the game that this UI should take zoom into account.

                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Boss HP Bars", delegate ()
                {
                    if (Main.LocalPlayer.Calamity().drawBossHPBar)
                    {
                        BossHealthBarManager.Update();
                        BossHealthBarManager.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.None));

                // Astral Arcanum overlay (if open)
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Astral Arcanum UI", delegate ()
                {
                    AstralArcanumUI.UpdateAndDraw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));

                // Rage and Adrenaline bars
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Rage and Adrenaline UI", delegate ()
                {
                    RipperUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));

                // Stealth bar
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Stealth UI", () =>
                {
                    StealthUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));

                // Popup GUIs.
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Popup GUIs", () =>
                {
                    PopupGUIManager.UpdateAndDraw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));
            }

            // Invasion UI (used for Acid Rain)
            int invasionIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Diagnose Net");
            if (invasionIndex != -1)
            {
                layers.Insert(invasionIndex, new LegacyGameInterfaceLayer("Acid Rain Invasion UI", () =>
                {
                    if (CalamityWorld.rainingAcid)
                    {
                        AcidRainUI.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.None));
            }
        }

        public static Color GetNPCColor(NPC npc, Vector2? position = null, bool effects = true, float shadowOverride = 0f)
        {
            return npc.GetAlpha(BuffEffects(
                npc, GetLightColor(position != null ? (Vector2)position : npc.Center),
                shadowOverride != 0f ? shadowOverride : 0f, effects, npc.poisoned, npc.onFire, npc.onFire2,
                Main.player[Main.myPlayer].detectCreature, false, false, false, npc.venom, npc.midas, npc.ichor,
                npc.onFrostBurn, false, false, npc.dripping, npc.drippingSlime, npc.loveStruck, npc.stinky)
            );
        }

        public static Color GetLightColor(Vector2 position) => Lighting.GetColor((int)(position.X / 16f), (int)(position.Y / 16f));

        public static Color BuffEffects(Entity codable, Color lightColor, float shadow = 0f, bool effects = true,
            bool poisoned = false, bool onFire = false, bool onFire2 = false, bool hunter = false, bool noItems = false,
            bool blind = false, bool bleed = false, bool venom = false, bool midas = false, bool ichor = false,
            bool onFrostBurn = false, bool burned = false, bool honey = false, bool dripping = false,
            bool drippingSlime = false, bool loveStruck = false, bool stinky = false)
        {
            float cr = 1f;
            float cg = 1f;
            float cb = 1f;
            float ca = 1f;
            if (effects && honey && Main.rand.NextBool(30))
            {
                int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 152, 0f, 0f, 150, default, 1f);
                Main.dust[dustID].velocity.Y = 0.3f;
                Main.dust[dustID].velocity.X *= 0.1f;
                Main.dust[dustID].scale += Main.rand.Next(3, 4) * 0.1f;
                Main.dust[dustID].alpha = 100;
                Main.dust[dustID].noGravity = true;
                Main.dust[dustID].velocity += codable.velocity * 0.1f;
                if (codable is Player)
                {
                    Main.playerDrawDust.Add(dustID);
                }
            }
            if (poisoned)
            {
                if (effects && Main.rand.NextBool(30))
                {
                    int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 46, 0f, 0f, 120, default, 0.2f);
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].fadeIn = 1.9f;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                cr *= 0.65f;
                cb *= 0.75f;
            }
            if (venom)
            {
                if (effects && Main.rand.NextBool(10))
                {
                    int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 171, 0f, 0f, 100, default, 0.5f);
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].fadeIn = 1.5f;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                cg *= 0.45f;
                cr *= 0.75f;
            }
            if (midas)
            {
                cb *= 0.3f;
                cr *= 0.85f;
            }
            if (ichor)
            {
                if (codable is NPC)
                {
                    lightColor = new Color(255, 255, 0, 255);
                }
                else
                {
                    cb = 0f;
                }
            }
            if (burned)
            {
                if (effects)
                {
                    int dustID = Dust.NewDust(new Vector2(codable.position.X - 2f, codable.position.Y - 2f), codable.width + 4, codable.height + 4, 6, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 2f);
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].velocity *= 1.8f;
                    Main.dust[dustID].velocity.Y -= 0.75f;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                if (codable is Player)
                {
                    cr = 1f;
                    cb *= 0.6f;
                    cg *= 0.7f;
                }
            }
            if (onFrostBurn)
            {
                if (effects)
                {
                    if (Main.rand.Next(4) < 3)
                    {
                        int dustID = Dust.NewDust(new Vector2(codable.position.X - 2f, codable.position.Y - 2f), codable.width + 4, codable.height + 4, 135, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 3.5f);
                        Main.dust[dustID].noGravity = true;
                        Main.dust[dustID].velocity *= 1.8f;
                        Main.dust[dustID].velocity.Y -= 0.5f;
                        if (Main.rand.NextBool(4))
                        {
                            Main.dust[dustID].noGravity = false;
                            Main.dust[dustID].scale *= 0.5f;
                        }
                        if (codable is Player)
                        {
                            Main.playerDrawDust.Add(dustID);
                        }
                    }
                    Lighting.AddLight((int)(codable.position.X / 16f), (int)(codable.position.Y / 16f + 1f), 0.1f, 0.6f, 1f);
                }
                if (codable is Player)
                {
                    cr *= 0.5f;
                    cg *= 0.7f;
                }
            }
            if (onFire)
            {
                if (effects)
                {
                    if (Main.rand.Next(4) != 0)
                    {
                        int dustID = Dust.NewDust(codable.position - new Vector2(2f, 2f), codable.width + 4, codable.height + 4, 6, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 3.5f);
                        Main.dust[dustID].noGravity = true;
                        Main.dust[dustID].velocity *= 1.8f;
                        Main.dust[dustID].velocity.Y -= 0.5f;
                        if (Main.rand.NextBool(4))
                        {
                            Main.dust[dustID].noGravity = false;
                            Main.dust[dustID].scale *= 0.5f;
                        }
                        if (codable is Player)
                        {
                            Main.playerDrawDust.Add(dustID);
                        }
                    }
                    Lighting.AddLight((int)(codable.position.X / 16f), (int)(codable.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
                }
                if (codable is Player)
                {
                    cb *= 0.6f;
                    cg *= 0.7f;
                }
            }
            if (dripping && shadow == 0f && Main.rand.Next(4) != 0)
            {
                Vector2 position = codable.position;
                position.X -= 2f;
                position.Y -= 2f;
                if (Main.rand.NextBool(2))
                {
                    int dustID = Dust.NewDust(position, codable.width + 4, codable.height + 2, 211, 0f, 0f, 50, default, 0.8f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustID].alpha += 25;
                    }
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustID].alpha += 25;
                    }
                    Main.dust[dustID].noLight = true;
                    Main.dust[dustID].velocity *= 0.2f;
                    Main.dust[dustID].velocity.Y += 0.2f;
                    Main.dust[dustID].velocity += codable.velocity;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                else
                {
                    int dustID = Dust.NewDust(position, codable.width + 8, codable.height + 8, 211, 0f, 0f, 50, default, 1.1f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustID].alpha += 25;
                    }
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustID].alpha += 25;
                    }
                    Main.dust[dustID].noLight = true;
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].velocity *= 0.2f;
                    Main.dust[dustID].velocity.Y += 1f;
                    Main.dust[dustID].velocity += codable.velocity;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
            }
            if (drippingSlime && shadow == 0f)
            {
                int alpha = 175;
                Color newColor = new Color(0, 80, 255, 100);
                if (Main.rand.Next(4) != 0)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 position2 = codable.position;
                        position2.X -= 2f;
                        position2.Y -= 2f;
                        int dustID = Dust.NewDust(position2, codable.width + 4, codable.height + 2, 4, 0f, 0f, alpha, newColor, 1.4f);
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[dustID].alpha += 25;
                        }
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[dustID].alpha += 25;
                        }
                        Main.dust[dustID].noLight = true;
                        Main.dust[dustID].velocity *= 0.2f;
                        Main.dust[dustID].velocity.Y += 0.2f;
                        Main.dust[dustID].velocity += codable.velocity;
                        if (codable is Player)
                        {
                            Main.playerDrawDust.Add(dustID);
                        }
                    }
                }
                cr *= 0.8f;
                cg *= 0.8f;
            }
            if (onFire2)
            {
                if (effects)
                {
                    if (Main.rand.Next(4) != 0)
                    {
                        int dustID = Dust.NewDust(codable.position - new Vector2(2f, 2f), codable.width + 4, codable.height + 4, 75, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 3.5f);
                        Main.dust[dustID].noGravity = true;
                        Main.dust[dustID].velocity *= 1.8f;
                        Main.dust[dustID].velocity.Y -= 0.5f;
                        if (Main.rand.NextBool(4))
                        {
                            Main.dust[dustID].noGravity = false;
                            Main.dust[dustID].scale *= 0.5f;
                        }
                        if (codable is Player)
                        {
                            Main.playerDrawDust.Add(dustID);
                        }
                    }
                    Lighting.AddLight((int)(codable.position.X / 16f), (int)(codable.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
                }
                if (codable is Player)
                {
                    cb *= 0.6f;
                    cg *= 0.7f;
                }
            }
            if (noItems)
            {
                cr *= 0.65f;
                cg *= 0.8f;
            }
            if (blind)
            {
                cr *= 0.7f;
                cg *= 0.65f;
            }
            if (bleed)
            {
                bool dead = codable is Player ? ((Player)codable).dead : codable is NPC ? ((NPC)codable).life <= 0 : false;
                if (effects && !dead && Main.rand.NextBool(30))
                {
                    int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 5, 0f, 0f, 0, default, 1f);
                    Main.dust[dustID].velocity.Y += 0.5f;
                    Main.dust[dustID].velocity *= 0.25f;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                cg *= 0.9f;
                cb *= 0.9f;
            }
            if (loveStruck && effects && shadow == 0f && Main.instance.IsActive && !Main.gamePaused && Main.rand.NextBool(5))
            {
                Vector2 value = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                value.Normalize();
                value.X *= 0.66f;
                int goreID = Gore.NewGore(codable.position + new Vector2(Main.rand.Next(codable.width + 1), Main.rand.Next(codable.height + 1)), value * Main.rand.Next(3, 6) * 0.33f, 331, Main.rand.Next(40, 121) * 0.01f);
                Main.gore[goreID].sticky = false;
                Main.gore[goreID].velocity *= 0.4f;
                Main.gore[goreID].velocity.Y -= 0.6f;
                if (codable is Player)
                {
                    Main.playerDrawGore.Add(goreID);
                }
            }
            if (stinky && shadow == 0f)
            {
                cr *= 0.7f;
                cb *= 0.55f;
                if (effects && Main.rand.NextBool(5) && Main.instance.IsActive && !Main.gamePaused)
                {
                    Vector2 value2 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                    value2.Normalize();
                    value2.X *= 0.66f;
                    value2.Y = Math.Abs(value2.Y);
                    Vector2 vector = value2 * Main.rand.Next(3, 5) * 0.25f;
                    int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 188, vector.X, vector.Y * 0.5f, 100, default, 1.5f);
                    Main.dust[dustID].velocity *= 0.1f;
                    Main.dust[dustID].velocity.Y -= 0.5f;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
            }
            lightColor.R = (byte)(lightColor.R * cr);
            lightColor.G = (byte)(lightColor.G * cg);
            lightColor.B = (byte)(lightColor.B * cb);
            lightColor.A = (byte)(lightColor.A * ca);
            if (codable is NPC)
            {
                NPCLoader.DrawEffects((NPC)codable, ref lightColor);
            }
            if (hunter && (codable is NPC ? ((NPC)codable).lifeMax > 1 : true))
            {
                if (effects && !Main.gamePaused && Main.instance.IsActive && Main.rand.NextBool(50))
                {
                    int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 15, 0f, 0f, 150, default, 0.8f);
                    Main.dust[dustID].velocity *= 0.1f;
                    Main.dust[dustID].noLight = true;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                byte colorR = 50, colorG = 255, colorB = 50;
                if (codable is NPC && !(((NPC)codable).friendly || ((NPC)codable).catchItem > 0 || (((NPC)codable).damage == 0 && ((NPC)codable).lifeMax == 5)))
                {
                    colorR = 255;
                    colorG = 50;
                }
                if (!(codable is NPC) && lightColor.R < 150)
                {
                    lightColor.A = Main.mouseTextColor;
                }
                if (lightColor.R < colorR)
                {
                    lightColor.R = colorR;
                }
                if (lightColor.G < colorG)
                {
                    lightColor.G = colorG;
                }
                if (lightColor.B < colorB)
                {
                    lightColor.B = colorB;
                }
            }
            return lightColor;
        }

        public static void DrawTexture(object sb, Texture2D texture, int shader, Entity codable, Color? overrideColor = null, bool drawCentered = false)
        {
            Color lightColor = overrideColor != null ? (Color)overrideColor : codable is NPC ? GetNPCColor((NPC)codable, codable.Center, false) : codable is Projectile ? ((Projectile)codable).GetAlpha(GetLightColor(codable.Center)) : GetLightColor(codable.Center);
            int frameCount = codable is NPC ? Main.npcFrameCount[((NPC)codable).type] : 1;
            Rectangle frame = codable is NPC ? ((NPC)codable).frame : new Rectangle(0, 0, texture.Width, texture.Height);
            float scale = codable is NPC ? ((NPC)codable).scale : ((Projectile)codable).scale;
            float rotation = codable is NPC ? ((NPC)codable).rotation : ((Projectile)codable).rotation;
            int spriteDirection = codable is NPC ? ((NPC)codable).spriteDirection : ((Projectile)codable).spriteDirection;
            float offsetY = codable is NPC ? ((NPC)codable).gfxOffY : 0f;
            DrawTexture(sb, texture, shader, codable.position + new Vector2(0f, offsetY), codable.width, codable.height, scale, rotation, spriteDirection, frameCount, frame, lightColor, drawCentered);
        }

        public static void DrawTexture(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, float scale, float rotation, int direction, int framecount, Rectangle frame, Color? overrideColor = null, bool drawCentered = false)
        {
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / framecount / 2);
            Color lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(position + new Vector2(width * 0.5f, height * 0.5f));
            if (sb is List<DrawData>)
            {
                DrawData dd = new DrawData(texture, GetDrawPosition(position, origin, width, height, texture.Width, texture.Height, framecount, scale, drawCentered), frame, lightColor, rotation, origin, scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0)
                {
                    shader = shader
                };
                ((List<DrawData>)sb).Add(dd);
            }
            else if (sb is SpriteBatch)
            {
                bool applyDye = shader > 0;
                if (applyDye)
                {
                    ((SpriteBatch)sb).End();
                    ((SpriteBatch)sb).Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                }
                ((SpriteBatch)sb).Draw(texture, GetDrawPosition(position, origin, width, height, texture.Width, texture.Height, framecount, scale, drawCentered), frame, lightColor, rotation, origin, scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                if (applyDye)
                {
                    ((SpriteBatch)sb).End();
                    ((SpriteBatch)sb).Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                }
            }
        }

        public static Vector2 GetDrawPosition(Vector2 position, Vector2 origin, int width, int height, int texWidth, int texHeight, int framecount, float scale, bool drawCentered = false)
        {
            Vector2 screenPos = new Vector2((int)Main.screenPosition.X, (int)Main.screenPosition.Y);
            if (drawCentered)
            {
                Vector2 texHalf = new Vector2(texWidth / 2, texHeight / framecount / 2);
                return position + new Vector2(width * 0.5f, height * 0.5f) - (texHalf * scale) + (origin * scale) - screenPos;
            }
            return position - screenPos + new Vector2(width * 0.5f, height) - new Vector2(texWidth * scale / 2f, texHeight * scale / framecount) + (origin * scale) + new Vector2(0f, 5f);
        }
        #endregion

        #region Recipes
        public override void AddRecipeGroups() => CalamityRecipes.AddRecipeGroups();

        public override void AddRecipes() => CalamityRecipes.AddRecipes();
        #endregion

        #region Seasons
        public static Season CurrentSeason
        {
            get
            {
                DateTime date = DateTime.Now;
                int day = date.DayOfYear - Convert.ToInt32(DateTime.IsLeapYear(date.Year) && date.DayOfYear > 59);

                if (day < 80 || day >= 355)
                {
                    return Season.Winter;
                }

                else if (day >= 80 && day < 172)
                {
                    return Season.Spring;
                }

                else if (day >= 172 && day < 266)
                {
                    return Season.Summer;
                }

                else
                {
                    return Season.Fall;
                }
            }
        }
        #endregion

        #region Lighting
        const float MaxCaveDarkness = -0.3f;
		const float MaxSignusDarkness = -0.4f;
		const float MaxAbyssDarkness = -0.7f;
        public override void ModifyLightingBrightness(ref float scale)
        {
			// Apply the calculated darkness value for the local player.
			CalamityPlayer modPlayer = Main.LocalPlayer.Calamity();
			float darkRatio = MathHelper.Clamp(modPlayer.caveDarkness, 0f, 1f);

			if (modPlayer.ZoneAbyss)
				scale += MaxAbyssDarkness * darkRatio;
			else if (CalamityWorld.death)
				scale += MaxCaveDarkness * darkRatio;

			if (CalamityWorld.revenge)
			{
				if (CalamityGlobalNPC.signus != -1)
				{
					if (Main.npc[CalamityGlobalNPC.signus].active)
					{
						if (Vector2.Distance(Main.LocalPlayer.Center, Main.npc[CalamityGlobalNPC.signus].Center) <= 5200f)
						{
							float signusLifeRatio = 1f - (Main.npc[CalamityGlobalNPC.signus].life / Main.npc[CalamityGlobalNPC.signus].lifeMax);

							// Reduce the power of Signus darkness based on your light level.
							float multiplier = 1f;
							switch (modPlayer.GetTotalLightStrength())
							{
								case 0:
									break;
								case 1:
								case 2:
									multiplier = 0.75f;
									break;
								case 3:
								case 4:
									multiplier = 0.5f;
									break;
								case 5:
								case 6:
									multiplier = 0.25f;
									break;
								default:
									multiplier = 0f;
									break;
							}

							// Increased darkness in Death Mode
							if (CalamityWorld.death)
								multiplier += (1f - multiplier) * 0.1f;

							// Total darkness
							float signusDarkness = signusLifeRatio * multiplier;
							darkRatio = MathHelper.Clamp(signusDarkness, 0f, 1f);
							scale += MaxSignusDarkness * darkRatio;
						}
					}
				}
			}
        }
        #endregion

        #region Packets
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            try
            {
                CalamityModMessageType msgType = (CalamityModMessageType)reader.ReadByte();
                switch (msgType)
                {
                    case CalamityModMessageType.MeleeLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 0);
                        break;
                    case CalamityModMessageType.RangedLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 1);
                        break;
                    case CalamityModMessageType.MagicLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 2);
                        break;
                    case CalamityModMessageType.SummonLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 3);
                        break;
                    case CalamityModMessageType.RogueLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 4);
                        break;
                    case CalamityModMessageType.ExactMeleeLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 0);
                        break;
                    case CalamityModMessageType.ExactRangedLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 1);
                        break;
                    case CalamityModMessageType.ExactMagicLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 2);
                        break;
                    case CalamityModMessageType.ExactSummonLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 3);
                        break;
                    case CalamityModMessageType.ExactRogueLevelSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 4);
                        break;
                    case CalamityModMessageType.StressSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleStress(reader);
                        break;
                    case CalamityModMessageType.BossRushStage:
                        int stage = reader.ReadInt32();
                        CalamityWorld.bossRushStage = stage;
                        break;
                    case CalamityModMessageType.AdrenalineSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleAdrenaline(reader);
                        break;
                    case CalamityModMessageType.TeleportPlayer:
                        Main.player[reader.ReadInt32()].Calamity().HandleTeleport(reader.ReadInt32(), true, whoAmI);
                        break;
                    case CalamityModMessageType.DoGCountdownSync:
                        int countdown = reader.ReadInt32();
                        CalamityWorld.DoGSecondStageCountdown = countdown;
                        break;
                    case CalamityModMessageType.BossSpawnCountdownSync:
                        int countdown2 = reader.ReadInt32();
                        CalamityWorld.bossSpawnCountdown = countdown2;
                        break;
                    case CalamityModMessageType.BRHostileProjKillSync:
                        int countdown3 = reader.ReadInt32();
                        CalamityWorld.bossRushHostileProjKillCounter = countdown3;
                        break;
                    case CalamityModMessageType.DeathBossSpawnCountdownSync:
                        int countdown4 = reader.ReadInt32();
                        CalamityWorld.deathBossSpawnCooldown = countdown4;
                        break;
                    case CalamityModMessageType.ArmoredDiggerCountdownSync:
                        int countdown5 = reader.ReadInt32();
                        CalamityWorld.ArmoredDiggerSpawnCooldown = countdown5;
                        break;
                    case CalamityModMessageType.BossTypeSync:
                        int type = reader.ReadInt32();
                        CalamityWorld.bossType = type;
                        break;
                    case CalamityModMessageType.DeathCountSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDeathCount(reader);
                        break;
                    case CalamityModMessageType.DeathModeUnderworldTimeSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDeathModeUnderworldTime(reader);
                        break;
                    case CalamityModMessageType.DeathModeBlizzardTimeSync:
                        Main.player[reader.ReadInt32()].Calamity().HandleDeathModeBlizzardTime(reader);
                        break;
                    case CalamityModMessageType.RevengeanceBoolSync:
                        bool revActive = reader.ReadBoolean();
                        CalamityWorld.revenge = revActive;
                        break;
                    case CalamityModMessageType.DeathBoolSync:
                        bool deathActive = reader.ReadBoolean();
                        CalamityWorld.death = deathActive;
                        break;
                    case CalamityModMessageType.DefiledBoolSync:
                        bool defiledActive = reader.ReadBoolean();
                        CalamityWorld.defiled = defiledActive;
                        break;
                    case CalamityModMessageType.IronHeartBoolSync:
                        bool ironHeartActive = reader.ReadBoolean();
                        CalamityWorld.ironHeart = ironHeartActive;
                        break;
                    case CalamityModMessageType.ArmageddonBoolSync:
                        bool armaActive = reader.ReadBoolean();
                        CalamityWorld.armageddon = armaActive;
                        break;
                    case CalamityModMessageType.DemonTrophyBoolSync:
                        bool demonModeBoost = reader.ReadBoolean();
                        CalamityWorld.demonMode = demonModeBoost;
                        break;
                    case CalamityModMessageType.NPCRegenerationSync:
                        byte npcIndex = reader.ReadByte();
                        Main.npc[npcIndex].lifeRegen = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.AcidRainSync:
                        CalamityWorld.rainingAcid = reader.ReadBoolean();
                        CalamityWorld.acidRainPoints = reader.ReadInt32();
                        CalamityWorld.timeSinceAcidRainKill = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.AcidRainUIDrawFadeSync:
                        CalamityWorld.acidRainExtraDrawTime = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.AcidRainOldDukeSummonSync:
                        CalamityWorld.triedToSummonOldDuke = reader.ReadBoolean();
                        break;
                    case CalamityModMessageType.GaelsGreatswordSwingSync:
                        byte playerIndex = reader.ReadByte();
                        Main.player[playerIndex].Calamity().gaelSwipes = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.SpawnSuperDummy:
                        int x = reader.ReadInt32();
                        int y = reader.ReadInt32();
                        // Not strictly necessary, but helps prevent unnecessary packetstorm in MP
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.NewNPC(x, y, ModContent.NPCType<SuperDummyNPC>());
                        break;
                    case CalamityModMessageType.DraedonGeneratorStackSync:
                        (TileEntity.ByID[reader.ReadInt32()] as TEDraedonFuelFactory).HeldItem.stack = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.DraedonChargerSync:
                        int entityID = reader.ReadInt32();
                        (TileEntity.ByID[entityID] as TEDraedonItemCharger).FuelItem.type = reader.ReadInt32();
                        (TileEntity.ByID[entityID] as TEDraedonItemCharger).FuelItem.stack = reader.ReadInt32();
                        (TileEntity.ByID[entityID] as TEDraedonItemCharger).FuelItem.position = reader.ReadVector2();
                        (TileEntity.ByID[entityID] as TEDraedonItemCharger).ItemBeingCharged.type = reader.ReadInt32();
                        (TileEntity.ByID[entityID] as TEDraedonItemCharger).ItemBeingCharged.stack = reader.ReadInt32();
                        (TileEntity.ByID[entityID] as TEDraedonItemCharger).ItemBeingCharged.prefix = reader.ReadByte();
                        (TileEntity.ByID[entityID] as TEDraedonItemCharger).ItemBeingCharged.position = reader.ReadVector2();
                        int currentCharge = reader.ReadInt32();
                        if (currentCharge != -1)
                        {
                            (TileEntity.ByID[entityID] as TEDraedonItemCharger).ItemBeingCharged.Calamity().CurrentCharge = currentCharge;
                        }
                        (TileEntity.ByID[entityID] as TEDraedonItemCharger).ActiveTimer = reader.ReadInt32();
                        (TileEntity.ByID[entityID] as TEDraedonItemCharger).DepositWithdrawCooldown = reader.ReadInt32();
                        break;
                    case CalamityModMessageType.DraedonFieldGeneratorSync:
                        int entityID2 = reader.ReadInt32();
                        (TileEntity.ByID[entityID2] as TEDraedonFieldGenerator).Time = reader.ReadInt32();
                        (TileEntity.ByID[entityID2] as TEDraedonFieldGenerator).ActiveTimer = reader.ReadInt32();
                        break;
					case CalamityModMessageType.SyncCalamityNPCAIArray:
						byte npcIndex2 = reader.ReadByte();
						Main.npc[npcIndex2].Calamity().newAI[0] = reader.ReadSingle();
						Main.npc[npcIndex2].Calamity().newAI[1] = reader.ReadSingle();
						Main.npc[npcIndex2].Calamity().newAI[2] = reader.ReadSingle();
						Main.npc[npcIndex2].Calamity().newAI[3] = reader.ReadSingle();
						break;
                    case CalamityModMessageType.ProvidenceDyeConditionSync:
                        byte npcIndex3 = reader.ReadByte();
                        (Main.npc[npcIndex3].modNPC as Providence).hasTakenDaytimeDamage = reader.ReadBoolean();
                        break;
                    case CalamityModMessageType.PSCChallengeSync:
                        byte npcIndex4 = reader.ReadByte();
                        (Main.npc[npcIndex4].modNPC as Providence).challenge = reader.ReadBoolean();
                        break;

                    default:
                        Logger.Error($"Failed to parse Calamity packet: No Calamity packet exists with ID {msgType}.");
                        break;
                }
            }
            catch(Exception e)
            {
                if (e is EndOfStreamException eose)
                    Logger.Error("Failed to parse Calamity packet: Packet was too short, missing data, or otherwise corrupt.", eose);
                else if (e is ObjectDisposedException ode)
                    Logger.Error("Failed to parse Calamity packet: Packet reader disposed or destroyed.", ode);
                else if (e is IOException ioe)
                    Logger.Error("Failed to parse Calamity packet: An unknown I/O error occurred.", ioe);
                else
                    throw e; // this either will crash the game or be caught by TML's packet policing
            }
        }
        #endregion

        #region Stop Rain
        public static void StopRain()
        {
            if (!Main.raining)
                return;
            Main.raining = false;
            UpdateServerBoolean();
        }
        #endregion

        #region Update Server Boolean
        public static void UpdateServerBoolean()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
            }
        }
		#endregion
	}

    public enum Season : byte
    {
        Winter,
        Spring,
        Summer,
        Fall
    }

    enum CalamityModMessageType : byte
    {
        MeleeLevelSync,
        RangedLevelSync,
        MagicLevelSync,
        SummonLevelSync,
        RogueLevelSync,
        ExactMeleeLevelSync,
        ExactRangedLevelSync,
        ExactMagicLevelSync,
        ExactSummonLevelSync,
        ExactRogueLevelSync,
        StressSync,
        AdrenalineSync,
        TeleportPlayer,
        BossRushStage,
        DoGCountdownSync,
        BossSpawnCountdownSync,
		BRHostileProjKillSync,
		ArmoredDiggerCountdownSync,
        BossTypeSync,
        DeathCountSync,
        RevengeanceBoolSync,
        DeathBoolSync,
        DefiledBoolSync,
        IronHeartBoolSync,
        ArmageddonBoolSync,
        DemonTrophyBoolSync,
        NPCRegenerationSync,
		DeathModeUnderworldTimeSync,
		DeathModeBlizzardTimeSync,
		DeathBossSpawnCountdownSync,
        AcidRainSync,
        AcidRainUIDrawFadeSync,
        AcidRainOldDukeSummonSync,
        GaelsGreatswordSwingSync,
        SpawnSuperDummy,
		SyncCalamityNPCAIArray,
        ProvidenceDyeConditionSync, // We shouldn't fucking need this. Die in a hole, Multiplayer.
        PSCChallengeSync, // See above
        DraedonGeneratorStackSync,
        DraedonChargerSync,
        DraedonFieldGeneratorSync
    }
}
