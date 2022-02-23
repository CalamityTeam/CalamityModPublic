using CalamityMod.Balancing;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Effects;
using CalamityMod.Events;
using CalamityMod.ILEditing;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Dyes.HairDye;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.Localization;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AdultEidolonWyrm;
using CalamityMod.NPCs.AquaticScourge;
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
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
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
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Schematics;
using CalamityMod.Skies;
using CalamityMod.TileEntities;
using CalamityMod.UI;
using CalamityMod.UI.CalamitasEnchants;
using CalamityMod.Waters;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Dyes;
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
        // TODO -- 1.4 fixes the crit reforge price calculation bug, so GetWeaponCrit everywhere can go.

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
        public static ModHotKey AngelicAllianceHotKey;
        public static ModHotKey GodSlayerDashHotKey;
        public static ModHotKey ExoChairSpeedupHotkey;
        public static ModHotKey ExoChairSlowdownHotkey;

        // Boss Spawners
        public static int ghostKillCount = 0;
        public static int sharkKillCount = 0;

        // Textures
        public static Texture2D heartOriginal2;
        public static Texture2D heartOriginal;
        public static Texture2D rainOriginal;
        public static Texture2D manaOriginal;
        public static Texture2D carpetOriginal;
        public static Texture2D AstralCactusTexture;
        public static Texture2D AstralCactusGlowTexture;
        public static Texture2D AstralSky;

        // DR data structure
        public static SortedDictionary<int, float> DRValues;

        // Boss Kill Time data structure
        public static SortedDictionary<int, int> bossKillTimes;

        // Boss velocity scaling data structure
        public static SortedDictionary<int, float> bossVelocityDamageScaleValues;
        public const float velocityScaleMin = 0.5f;
        public const float bitingEnemeyVelocityScale = 0.8f;

        // Life steal cap
        public const int lifeStealCap = 10;

        // Speedrun timer
        internal static Stopwatch SpeedrunTimer = new Stopwatch();

        // Debuff immunities, these are used in the NPCDebuffs file
        public static int[] slimeEnemyImmunities = new int[1] { BuffID.Poisoned };
        public static int[] iceEnemyImmunities = new int[3] { BuffID.Frostburn, ModContent.BuffType<GlacialState>(), ModContent.BuffType<ExoFreeze>() };
        public static int[] sulphurEnemyImmunities = new int[4] { BuffID.Poisoned, BuffID.Venom, ModContent.BuffType<SulphuricPoisoning>(), ModContent.BuffType<Irradiated>() };
        public static int[] sunkenSeaEnemyImmunities = new int[2] { ModContent.BuffType<Eutrophication>(), ModContent.BuffType<PearlAura>() };
        public static int[] abyssEnemyImmunities = new int[1] { ModContent.BuffType<CrushDepth>() };
        public static int[] cragEnemyImmunities = new int[3] { BuffID.OnFire, ModContent.BuffType<AbyssalFlames>(), ModContent.BuffType<BrimstoneFlames>() };
        public static int[] astralEnemyImmunities = new int[2] { BuffID.Poisoned, ModContent.BuffType<AstralInfectionDebuff>() };
        public static int[] plagueEnemyImmunities = new int[3] { BuffID.Poisoned, BuffID.Venom, ModContent.BuffType<Plague>() };
        public static int[] holyEnemyImmunities = new int[3] { BuffID.OnFire, ModContent.BuffType<HolyFlames>(), ModContent.BuffType<Nightwither>() };

        internal static CalamityMod Instance;
        internal Mod musicMod = null; // This is Calamity's official music mod, CalamityModMusic
        internal bool MusicAvailable => !(musicMod is null);
        internal Mod ancientsAwakened = null;
        internal Mod bossChecklist = null;
        internal Mod census = null;
        internal Mod crouchMod = null;
        internal Mod fargos = null;
        internal Mod redemption = null;
        internal Mod soa = null;
        internal Mod summonersAssociation = null;
        internal Mod thorium = null;
        internal Mod varia = null;

        #region Load
        public override void Load()
        {
            Instance = this;

            // Save vanilla textures.
            heartOriginal2 = Main.heartTexture;
            heartOriginal = Main.heart2Texture;
            rainOriginal = Main.rainTexture;
            manaOriginal = Main.manaTexture;
            carpetOriginal = Main.flyingCarpetTexture;

            // Apply IL edits instantly afterwards.
            ILChanges.Load();

            // If any of these mods aren't loaded, it will simply keep them as null.
            musicMod = ModLoader.GetMod("CalamityModMusic");
            ancientsAwakened = ModLoader.GetMod("AAMod");
            bossChecklist = ModLoader.GetMod("BossChecklist");
            census = ModLoader.GetMod("Census");
            crouchMod = ModLoader.GetMod("CrouchMod");
            fargos = ModLoader.GetMod("Fargowiltas");
            redemption = ModLoader.GetMod("Redemption");
            soa = ModLoader.GetMod("SacredTools");
            summonersAssociation = ModLoader.GetMod("SummonersAssociation");
            thorium = ModLoader.GetMod("ThoriumMod");
            varia = ModLoader.GetMod("Varia");

            // Initialize the EnemyStats struct as early as it is safe to do so
            NPCStats.Load();

            // Initialize Calamity Lists so they may be used elsewhere immediately
            CalamityLists.LoadLists();

            // Initialize Calamity Balance, since it is tightly coupled with the remaining lists
            CalamityGlobalItem.LoadTweaks();

            // Mount balancing occurs during runtime and is undone when Calamity is unloaded.
            Mount.mounts[Mount.Unicorn].dashSpeed *= CalamityPlayer.UnicornSpeedNerfPower;
            Mount.mounts[Mount.Unicorn].runSpeed *= CalamityPlayer.UnicornSpeedNerfPower;
            Mount.mounts[Mount.MinecartMech].dashSpeed *= CalamityPlayer.MechanicalCartSpeedNerfPower;
            Mount.mounts[Mount.MinecartMech].runSpeed *= CalamityPlayer.MechanicalCartSpeedNerfPower;

            NormalityRelocatorHotKey = RegisterHotKey("Normality Relocator", "Z");
            RageHotKey = RegisterHotKey("Rage Mode", "V");
            AdrenalineHotKey = RegisterHotKey("Adrenaline Mode", "B");
            AegisHotKey = RegisterHotKey("Elysian Guard", "N");
            TarraHotKey = RegisterHotKey("Armor Set Bonus", "Y");
            AstralTeleportHotKey = RegisterHotKey("Astral Teleport", "P");
            AstralArcanumUIHotkey = RegisterHotKey("Astral Arcanum UI Toggle", "O");
            MomentumCapacitatorHotkey = RegisterHotKey("Momentum Capacitor Effect", "U");
            SandCloakHotkey = RegisterHotKey("Sand Cloak Effect", "C");
            SpectralVeilHotKey = RegisterHotKey("Spectral Veil Teleport", "Z");
            PlaguePackHotKey = RegisterHotKey("Booster Dash", "Q");
            AngelicAllianceHotKey = RegisterHotKey("Angelic Alliance Blessing", "G");
            GodSlayerDashHotKey = RegisterHotKey("God Slayer Dash", "H");
            ExoChairSpeedupHotkey = RegisterHotKey("Exo Chair Speed Up", "LeftShift");
            ExoChairSlowdownHotkey = RegisterHotKey("Exo Chair Slow Down", "RightShift");

            if (!Main.dedServ)
            {
                LoadClient();
                GeneralParticleHandler.Load();
            }

            BossRushEvent.Load();
            BossHealthBarManager.Load(this);
            DraedonStructures.Load();
            EnchantmentManager.LoadAllEnchantments();
            VanillaArmorChangeManager.Load();
            SetupVanillaDR();
            SetupBossKillTimes();
            SetupBossVelocityScalingValues();
            CalamityLocalization.AddLocalizations();
            CalamityConfig.LoadConfigLabels(ModContent.GetInstance<CalamityMod>());
            SchematicManager.Load();
            CustomLavaManagement.Load();
            Attunement.Load();
            BalancingChangesManager.Load();
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

            AddEquipTexture(new OmegaBlueTransformationHead(), null, EquipType.Head, "OmegaBlueTransformationHead", "CalamityMod/Items/Armor/OmegaBlueHelmet_HeadMadness");

            AddEquipTexture(new AbyssDivingGearHair(), null, EquipType.Head, "AbyssDivingGearHead", "CalamityMod/Items/Accessories/AbyssalDivingGear_Face");
            AddEquipTexture(new FeatherCrownHair(), null, EquipType.Head, "FeatherCrownHead", "CalamityMod/Items/Accessories/FeatherCrown_Face");
            AddEquipTexture(new MoonstoneCrownHair(), null, EquipType.Head, "MoonstoneCrownHead", "CalamityMod/Items/Accessories/MoonstoneCrown_Face");

            AddEquipTexture(null, EquipType.Legs, "CirrusDress_Legs", "CalamityMod/Items/Armor/CirrusDress_Legs");

            AstralCactusTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Tiles/AstralCactus");
            AstralCactusGlowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Tiles/AstralCactusGlow");
            AstralSky = ModContent.GetTexture("CalamityMod/ExtraTextures/AstralSky");

            Filters.Scene["CalamityMod:DevourerofGodsHead"] = new Filter(new DoGScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.1f, 1.0f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:DevourerofGodsHead"] = new DoGSky();

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

            Filters.Scene["CalamityMod:AdultEidolonWyrm"] = new Filter(new AEWScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0.25f).UseOpacity(0.35f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:AdultEidolonWyrm"] = new AEWSky();

            Filters.Scene["CalamityMod:Signus"] = new Filter(new SignusScreenShaderData("FilterMiniTower").UseColor(0.35f, 0.1f, 0.55f).UseOpacity(0.35f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:Signus"] = new SignusSky();

            Filters.Scene["CalamityMod:BossRush"] = new Filter(new BossRushScreenShader("FilterMiniTower").UseOpacity(0.75f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:BossRush"] = new BossRushSky();

            Filters.Scene["CalamityMod:ExoMechs"] = new Filter(new ExoMechsScreenShaderData("FilterMiniTower").UseColor(ExoMechsSky.DrawColor).UseOpacity(0.25f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:ExoMechs"] = new ExoMechsSky();

            SkyManager.Instance["CalamityMod:Astral"] = new AstralSky();
            SkyManager.Instance["CalamityMod:Cryogen"] = new CryogenSky();
            SkyManager.Instance["CalamityMod:StormWeaverFlash"] = new StormWeaverFlashSky();

            CalamityShaders.LoadShaders();
            FusableParticleManager.LoadParticleRenderSets();
            Main.OnPreDraw += PrepareRenderTargets;

            RipperUI.Load();
            AstralArcanumUI.Load(this);

            Apollo.LoadHeadIcons();
            Artemis.LoadHeadIcons();
            DevourerofGodsHead.LoadHeadIcons();
            DevourerofGodsBody.LoadHeadIcons();
            DevourerofGodsTail.LoadHeadIcons();
            HiveMind.LoadHeadIcons();
            Polterghast.LoadHeadIcons();
            StormWeaverHead.LoadHeadIcons();
            SupremeCalamitas.LoadHeadIcons();
            ThanatosHead.LoadHeadIcons();
            ThanatosBody1.LoadHeadIcons();
            ThanatosBody2.LoadHeadIcons();
            ThanatosTail.LoadHeadIcons();

            GameShaders.Hair.BindShader(ModContent.ItemType<AdrenalineHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(0, 255, 171), ((float)player.Calamity().adrenaline / (float)player.Calamity().adrenalineMax))));
            GameShaders.Hair.BindShader(ModContent.ItemType<RageHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(255, 83, 48), ((float)player.Calamity().rage / (float)player.Calamity().rageMax))));
            GameShaders.Hair.BindShader(ModContent.ItemType<WingTimeHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(139, 205, 255), ((float)player.wingTime / (float)player.wingTimeMax))));
            GameShaders.Hair.BindShader(ModContent.ItemType<StealthHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(186, 85, 211), (player.Calamity().rogueStealth / player.Calamity().rogueStealthMax))));

            PopupGUIManager.LoadGUIs();
            InvasionProgressUIManager.LoadGUIs();
        }
        #endregion

        #region Unload
        public override void Unload()
        {
            musicMod = null;
            ancientsAwakened = null;
            bossChecklist = null;
            census = null;
            crouchMod = null;
            fargos = null;
            redemption = null;
            soa = null;
            summonersAssociation = null;
            thorium = null;
            varia = null;

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
            AngelicAllianceHotKey = null;
            GodSlayerDashHotKey = null;

            AstralCactusTexture = null;
            AstralCactusGlowTexture = null;
            AstralSky = null;

            DRValues?.Clear();
            DRValues = null;
            bossKillTimes?.Clear();
            bossKillTimes = null;
            bossVelocityDamageScaleValues?.Clear();
            bossVelocityDamageScaleValues = null;

            BalancingChangesManager.Unload();
            Attunement.Unload();
            EnchantmentManager.UnloadAllEnchantments();
            VanillaArmorChangeManager.Unload();
            CalamityLists.UnloadLists();
            NPCStats.Unload();
            CalamityGlobalItem.UnloadTweaks();

            PopupGUIManager.UnloadGUIs();
            InvasionProgressUIManager.UnloadGUIs();
            BossRushEvent.Unload();
            SchematicManager.Unload();
            CustomLavaManagement.Unload();
            BossHealthBarManager.Unload();
            DraedonStructures.Unload();

            TileFraming.Unload();

            FusableParticleManager.UnloadParticleRenderSets();
            Main.OnPreDraw -= PrepareRenderTargets;

            RipperUI.Unload();
            AstralArcanumUI.Unload();

            if (!Main.dedServ)
            {
                Main.heartTexture = heartOriginal2;
                Main.heart2Texture = heartOriginal;
                Main.rainTexture = rainOriginal;
                Main.manaTexture = manaOriginal;
                Main.flyingCarpetTexture = carpetOriginal;
                GeneralParticleHandler.Unload();
            }
            Mount.mounts[Mount.Unicorn].dashSpeed /= CalamityPlayer.UnicornSpeedNerfPower;
            Mount.mounts[Mount.Unicorn].runSpeed /= CalamityPlayer.UnicornSpeedNerfPower;
            Mount.mounts[Mount.MinecartMech].dashSpeed /= CalamityPlayer.MechanicalCartSpeedNerfPower;
            Mount.mounts[Mount.MinecartMech].runSpeed /= CalamityPlayer.MechanicalCartSpeedNerfPower;

            heartOriginal2 = null;
            heartOriginal = null;
            rainOriginal = null;
            manaOriginal = null;
            carpetOriginal = null;

            ILChanges.Unload();
            Instance = null;
            base.Unload();
        }
        #endregion

        #region Late Loading
        public override void PostAddRecipes()
        {
            // This is placed here so that all tiles from all mods are guaranteed to be loaded at this point.
            TileFraming.Load();
        }
        #endregion

        #region Render Target Management
        public static void PrepareRenderTargets(GameTime gameTime)
        {
            FusableParticleManager.PrepareFusableParticleTargets();
            DeathAshParticle.PrepareRenderTargets();
        }
        #endregion Render Target Management

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

        #region Fusable Particle Updating
        public override void MidUpdateProjectileItem()
        {
            // Update all fusable particles.
            // These are really only visual and as such don't really need any complex netcode.
            foreach (BaseFusableParticleSet.FusableParticleRenderCollection particleSet in FusableParticleManager.ParticleSets)
            {
                foreach (BaseFusableParticleSet.FusableParticle particle in particleSet.ParticleSet.Particles)
                    particleSet.ParticleSet.UpdateBehavior(particle);
            }
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
                { NPCID.CultistBoss, 0.15f },
                { NPCID.DD2Betsy, 0.1f },
                { NPCID.DD2OgreT2, 0.1f },
                { NPCID.DD2OgreT3, 0.15f },
                { NPCID.DeadlySphere, 0.4f },
                { NPCID.DiabolistRed, 0.2f },
                { NPCID.DiabolistWhite, 0.2f },
                { NPCID.DukeFishron, 0.15f },
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
                { NPCID.MoonLordCore, 0.15f },
                { NPCID.MoonLordHand, 0.15f },
                { NPCID.MoonLordHead, 0.15f },
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

        #region Boss Kill Times
        private void SetupBossKillTimes()
        {
            // Kill times are measured exactly in frames.
            // 60   frames = 1 second
            // 3600 frames = 1 minute
            bossKillTimes = new SortedDictionary<int, int> {
                //
                // VANILLA BOSSES
                //
                { NPCID.KingSlime, 3600 }, // 1:00 (60 seconds)
                { NPCID.EyeofCthulhu, 5400 }, // 1:30 (90 seconds)
                { NPCID.EaterofWorldsHead, 7200 }, // 2:00 (120 seconds)
                { NPCID.EaterofWorldsBody, 7200 },
                { NPCID.EaterofWorldsTail, 7200 },
                { NPCID.BrainofCthulhu, 5400 }, // 1:30 (90 seconds)
                { NPCID.Creeper, 1800 }, // 0:30 (30 seconds)
                { NPCID.QueenBee, 7200 }, // 2:00 (120 seconds)
                { NPCID.SkeletronHead, 9000 }, // 2:30 (150 seconds)
                { NPCID.WallofFlesh, 7200 }, // 2:00 (120 seconds)
                { NPCID.WallofFleshEye, 7200 },
                { NPCID.Spazmatism, 10800 }, // 3:00 (180 seconds)
                { NPCID.Retinazer, 10800 },
                { NPCID.TheDestroyer, 10800 }, // 3:00 (180 seconds)
                { NPCID.TheDestroyerBody, 10800 },
                { NPCID.TheDestroyerTail, 10800 },
                { NPCID.SkeletronPrime, 10800 }, // 3:00 (180 seconds)
                { NPCID.Plantera, 10800 }, // 3:00 (180 seconds)
                { NPCID.Golem, 9000 }, // 2:30 (150 seconds)
                { NPCID.GolemHead, 3600 }, // 1:00 (60 seconds)
                { NPCID.DukeFishron, 9000 }, // 2:30 (150 seconds)
                { NPCID.CultistBoss, 9000 }, // 2:30 (150 seconds)
                { NPCID.MoonLordCore, 14400 }, // 4:00 (240 seconds)
                { NPCID.MoonLordHand, 7200 }, // 2:00 (120 seconds)
                { NPCID.MoonLordHead, 7200 }, // 2:00 (120 seconds)

                //
                // CALAMITY BOSSES
                //
                { ModContent.NPCType<DesertScourgeHead>(), 3600 }, // 1:00 (60 seconds)
                { ModContent.NPCType<DesertScourgeBody>(), 3600 },
                { ModContent.NPCType<DesertScourgeTail>(), 3600 },
                { ModContent.NPCType<CrabulonIdle>(), 5400 }, // 1:30 (90 seconds)
                { ModContent.NPCType<HiveMind>(), 7200 }, // 2:00 (120 seconds)
                { ModContent.NPCType<PerforatorHive>(), 7200 }, // 2:00 (120 seconds)
                { ModContent.NPCType<SlimeGodCore>(), 10800 }, // 3:00 (180 seconds) -- total length of Slime God fight
                { ModContent.NPCType<SlimeGod>(), 3600 }, // 1:00 (60 seconds)
                { ModContent.NPCType<SlimeGodRun>(), 3600 }, // 1:00 (60 seconds)
                { ModContent.NPCType<SlimeGodSplit>(), 3600 }, // 1:00 (60 seconds) -- split slimes should spawn at 1:00 and die at around 2:00
                { ModContent.NPCType<SlimeGodRunSplit>(), 3600 }, // 1:00 (60 seconds)
                { ModContent.NPCType<Cryogen>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<AquaticScourgeHead>(), 7200 }, // 2:00 (120 seconds)
                { ModContent.NPCType<AquaticScourgeBody>(), 7200 },
                { ModContent.NPCType<AquaticScourgeBodyAlt>(), 7200 },
                { ModContent.NPCType<AquaticScourgeTail>(), 7200 },
                { ModContent.NPCType<BrimstoneElemental>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<CalamitasRun3>(), 14400 }, // 4:00 (240 seconds)
                { ModContent.NPCType<Siren>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<Leviathan>(), 10800 },
                { ModContent.NPCType<AstrumAureus>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<AstrumDeusHeadSpectral>(), 7200 }, // 2:00 (120 seconds) -- first phase is 1:00
                { ModContent.NPCType<AstrumDeusBodySpectral>(), 7200 },
                { ModContent.NPCType<AstrumDeusTailSpectral>(), 7200 },
                { ModContent.NPCType<PlaguebringerGoliath>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<RavagerBody>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<ProfanedGuardianBoss>(), 5400 }, // 1:30 (90 seconds)
                { ModContent.NPCType<Bumblefuck>(), 7200 }, // 2:00 (120 seconds)
                { ModContent.NPCType<Providence>(), 14400 }, // 4:00 (240 seconds)
                { ModContent.NPCType<CeaselessVoid>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<DarkEnergy>(), 1200 }, // 0:20 (20 seconds)
                { ModContent.NPCType<StormWeaverHead>(), 8100 }, // 2:15 (135 seconds)
                { ModContent.NPCType<StormWeaverBody>(), 8100 },
                { ModContent.NPCType<StormWeaverTail>(), 8100 },
                { ModContent.NPCType<Signus>(), 7200 }, // 2:00 (120 seconds)
                { ModContent.NPCType<Polterghast>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<OldDuke>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<DevourerofGodsHead>(), 14400 }, // 4:00 (240 seconds)
                { ModContent.NPCType<DevourerofGodsBody>(), 14400 }, // NOTE: Sentinels Phase takes 1:00, so with that included it's 5:00
                { ModContent.NPCType<DevourerofGodsTail>(), 14400 }, // DoG Phase 1 is 1:30, DoG Phase 2 is 2:30
                { ModContent.NPCType<Yharon>(), 14700 }, // 4:05 (245 seconds) -- he spends 5 seconds invincible where you can't do anything
                { ModContent.NPCType<Apollo>(), 21600 }, // 6:00 (360 seconds)
                { ModContent.NPCType<Artemis>(), 21600 },
                { ModContent.NPCType<AresBody>(), 21600 }, // 6:00 (360 seconds)
                { ModContent.NPCType<AresGaussNuke>(), 21600 },
                { ModContent.NPCType<AresLaserCannon>(), 21600 },
                { ModContent.NPCType<AresPlasmaFlamethrower>(), 21600 },
                { ModContent.NPCType<AresTeslaCannon>(), 21600 },
                { ModContent.NPCType<ThanatosHead>(), 21600 }, // 6:00 (360 seconds)
                { ModContent.NPCType<ThanatosBody1>(), 21600 },
                { ModContent.NPCType<ThanatosBody2>(), 21600 },
                { ModContent.NPCType<ThanatosTail>(), 21600 },
                { ModContent.NPCType<SupremeCalamitas>(), 18000 }, // 5:00 (300 seconds)
                { ModContent.NPCType<EidolonWyrmHeadHuge>(), 18000 } // 5:00 (300 seconds)
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
                { ModContent.NPCType<DesertNuisanceHead>(), bitingEnemeyVelocityScale },
                { ModContent.NPCType<DesertNuisanceBody>(), velocityScaleMin },
                { ModContent.NPCType<DesertNuisanceTail>(), velocityScaleMin },
                { ModContent.NPCType<CrabulonIdle>(), bitingEnemeyVelocityScale },
                { ModContent.NPCType<HiveMind>(), velocityScaleMin },
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
                { ModContent.NPCType<AquaticScourgeHead>(), bitingEnemeyVelocityScale },
                { ModContent.NPCType<AquaticScourgeBody>(), velocityScaleMin },
                { ModContent.NPCType<AquaticScourgeBodyAlt>(), velocityScaleMin },
                { ModContent.NPCType<AquaticScourgeTail>(), velocityScaleMin },
                { ModContent.NPCType<BrimstoneElemental>(), velocityScaleMin },
                { ModContent.NPCType<CalamitasRun>(), bitingEnemeyVelocityScale },
                { ModContent.NPCType<CalamitasRun2>(), bitingEnemeyVelocityScale },
                { ModContent.NPCType<CalamitasRun3>(), velocityScaleMin },
                { ModContent.NPCType<Leviathan>(), bitingEnemeyVelocityScale },
                { ModContent.NPCType<Siren>(), velocityScaleMin },
                { ModContent.NPCType<AstrumAureus>(), velocityScaleMin },
                { ModContent.NPCType<AstrumDeusHeadSpectral>(), bitingEnemeyVelocityScale },
                { ModContent.NPCType<AstrumDeusBodySpectral>(), velocityScaleMin },
                { ModContent.NPCType<AstrumDeusTailSpectral>(), velocityScaleMin },
                { ModContent.NPCType<PlaguebringerGoliath>(), velocityScaleMin },
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
                { ModContent.NPCType<StormWeaverHead>(), bitingEnemeyVelocityScale },
                { ModContent.NPCType<StormWeaverBody>(), velocityScaleMin },
                { ModContent.NPCType<StormWeaverTail>(), velocityScaleMin },
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
                { ModContent.NPCType<Yharon>(), velocityScaleMin },
                { ModContent.NPCType<SupremeCalamitas>(), velocityScaleMin },
                { ModContent.NPCType<Apollo>(), velocityScaleMin }, // Increases in phase 2
                { ModContent.NPCType<Artemis>(), velocityScaleMin },
                { ModContent.NPCType<ThanatosHead>(), bitingEnemeyVelocityScale },
                { ModContent.NPCType<ThanatosBody1>(), velocityScaleMin },
                { ModContent.NPCType<ThanatosBody2>(), velocityScaleMin },
                { ModContent.NPCType<ThanatosTail>(), velocityScaleMin },
                { ModContent.NPCType<EidolonWyrmHeadHuge>(), bitingEnemeyVelocityScale }
            };
        }
        #endregion

        #region Music

        // This function returns an available Calamity Music Mod track, or null if the Calamity Music Mod is not available.
        public int? GetMusicFromMusicMod(string songFilename) => MusicAvailable ? (int?)musicMod.GetSoundSlot(SoundType.Music, "Sounds/Music/" + songFilename) : null;

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.musicVolume != 0)
            {
                if (Main.myPlayer != -1 && !Main.gameMenu && Main.LocalPlayer.active)
                {
                    Player p = Main.LocalPlayer;
                    if (p.InCalamity())
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            music = GetMusicFromMusicMod("Crag") ?? MusicID.Eerie;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InSunkenSea())
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            music = GetMusicFromMusicMod("SunkenSea") ?? MusicID.Temple;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InAstral(1))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            music = GetMusicFromMusicMod("Astral") ?? MusicID.Space;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InAstral(2))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            music = GetMusicFromMusicMod("AstralUnderground") ?? MusicID.Space;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InAbyss(1) || p.InAbyss(2))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            music = GetMusicFromMusicMod("Abyss1") ?? MusicID.Hell;
                            priority = MusicPriority.BiomeHigh;
                        }
                    }
                    if (p.InAbyss(3))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            music = GetMusicFromMusicMod("Abyss2") ?? MusicID.Hell;
                            priority = MusicPriority.BiomeHigh;
                        }
                    }
                    if (p.InAbyss(4))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            music = GetMusicFromMusicMod("Abyss3") ?? MusicID.Hell;
                            priority = MusicPriority.BiomeHigh;
                        }
                    }
                    if (p.InSulphur())
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            bool acidRain = CalamityWorld.rainingAcid;
                            priority = acidRain ? MusicPriority.Event : MusicPriority.BiomeHigh;

                            // Acid Rain themes
                            if (acidRain)
                                music = CalamityWorld.downedPolterghast
                                    ? GetMusicFromMusicMod("AcidRain2") ?? MusicID.Eclipse // Acid Rain Tier 3
                                    : GetMusicFromMusicMod("AcidRain1") ?? MusicID.OldOnesArmy; // Acid Rain Tier 1 + 2

                            // Regular Sulphur Sea theme, when Acid Rain is not occurring
                            else
                                music = GetMusicFromMusicMod("SulphurousSea") ?? MusicID.Desert;
                        }
                    }
                    if (CalamityWorld.DoGSecondStageCountdown <= 530 && CalamityWorld.DoGSecondStageCountdown > 50) // 8 seconds before DoG returns
                    {
                        music = GetMusicFromMusicMod("DevourerOfGodsP2") ?? MusicID.LunarBoss;
                        priority = MusicPriority.BossMedium;
                    }

                    // This section handles boss rush music. However, at the time of PR-ing the boss rush visuals branch not all
                    // of the boss rush themes have been completed. As such, the custom music is intentionally omitted for the time being.
                    /*
                    if (BossRushEvent.BossRushActive && BossRushEvent.StartTimer >= BossRushEvent.StartEffectTotalTime)
                    {
                        music = BossRushEvent.MusicToPlay;
                        priority = MusicPriority.BossHigh;
                    }
                    */
                }
            }
        }
        #endregion

        #region Lighting Effects
        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (Main.gameMenu)
                BossRushEvent.StartTimer = 0;

            float bossRushWhiteFade = BossRushEvent.StartTimer / (float)BossRushEvent.StartEffectTotalTime;
            if (BossRushSky.ShouldDrawRegularly)
                bossRushWhiteFade = 1f;

            if (BossRushEvent.BossRushActive || BossRushEvent.StartTimer > 0 || BossRushSky.ShouldDrawRegularly)
            {
                backgroundColor = Color.Lerp(backgroundColor, Color.LightGray, bossRushWhiteFade);
                tileColor = Color.Lerp(tileColor, Color.LightGray, bossRushWhiteFade);
            }
            else if (SkyManager.Instance["CalamityMod:ExoMechs"].IsActive())
            {
                float intensity = SkyManager.Instance["CalamityMod:ExoMechs"].Opacity;
                backgroundColor = Color.Lerp(backgroundColor, Color.DarkGray, intensity * 0.9f);
                backgroundColor = Color.Lerp(backgroundColor, Color.Black, intensity * 0.67f);
                tileColor = Color.Lerp(tileColor, Color.DarkGray, intensity * 0.8f);
                tileColor = Color.Lerp(tileColor, Color.Black, intensity * 0.3f);
            }
        }
        #endregion

        #region Mod Support
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
                    LabHologramProjectorUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));

                // For these layers, InterfaceScaleType.Game tells the game that this UI should take zoom into account.
                // These must be separate layers or they will malfunction when hovering one at non-100% zoom.

                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Charging Station UI", () =>
                {
                    ChargingStationUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.Game));

                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Power Cell Factory UI", () =>
                {
                    PowerCellFactoryUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.Game));

                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Boss HP Bars", delegate ()
                {
                    if (Main.LocalPlayer.Calamity().drawBossHPBar)
                    {
                        BossHealthBarManager.Update();
                        BossHealthBarManager.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.None));

                // Mode Indicator UI.
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Mode Indicator UI", delegate ()
                {
                    ModeIndicatorUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.UI));

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

                // Charge meter
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Charge UI", () =>
                {
                    ChargeMeterUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));

                // Enchantment meters
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Enchantment Meters", () =>
                {
                    EnchantmentMetersUI.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));

                // Calamitas Enchantment UI
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Calamitas Enchantment", () =>
                {
                    CalamitasEnchantUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));

                // Codebreaker UI.
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Codebreaker Decryption GUI", () =>
                {
                    CodebreakerUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));

                // Popup GUIs.
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Popup GUIs", () =>
                {
                    PopupGUIManager.UpdateAndDraw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));

                // Exo Mech selection.
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Exo Mech Selection", () =>
                {
                    if (Main.LocalPlayer.Calamity().AbleToSelectExoMech)
                        ExoMechSelectionUI.Draw(Main.spriteBatch);
                    return true;
                }, InterfaceScaleType.None));
            }

            // Invasion UIs.
            int invasionIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Diagnose Net");
            if (invasionIndex != -1)
            {
                layers.Insert(invasionIndex, new LegacyGameInterfaceLayer("Calamity Invasion UIs", () =>
                {
                    InvasionProgressUIManager.UpdateAndDraw(Main.spriteBatch);
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
        const float MaxSignusDarkness = -0.4f;
        const float MaxAbyssDarkness = -0.7f;
        public override void ModifyLightingBrightness(ref float scale)
        {
            // Apply the calculated darkness value for the local player.
            CalamityPlayer modPlayer = Main.LocalPlayer.Calamity();
            float darkRatio = MathHelper.Clamp(modPlayer.caveDarkness, 0f, 1f);

            if (modPlayer.ZoneAbyss)
                scale += MaxAbyssDarkness * darkRatio;

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

        #region Stop Rain
        public static void StopRain()
        {
            if (!Main.raining)
                return;
            Main.raining = false;
            CalamityNetcode.SyncWorld();
        }
        #endregion

        #region Netcode
        public override void HandlePacket(BinaryReader reader, int whoAmI) => CalamityNetcode.HandlePacket(this, reader, whoAmI);
        #endregion

        #region Tile Entity Time Handler
        public override void MidUpdateTimeWorld() => TileEntityTimeHandler.Update();
        #endregion

        #region Ash Drawing
        public override void MidUpdateItemDust() => DeathAshParticle.UpdateAll();
        #endregion Ash Drawing

        #region Post NPC Updating
        // TODO - Apply caching to this process. For now most of the looping issues should be eradicated but it can be reduced further.
        public override void MidUpdateNPCGore() => CalamityGlobalNPC.ResetTownNPCNameBools();
        #endregion

        #region Speedrun Timer Stopper
        public override void PreSaveAndQuit() => SpeedrunTimer?.Stop();
        #endregion

        #region Particles updating
        public override void PostUpdateEverything()
        {
            if (!Main.dedServ)
            {
                GeneralParticleHandler.Update();
            }
        }
        #endregion
    }
}
