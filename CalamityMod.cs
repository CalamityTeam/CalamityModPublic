using CalamityMod.CalPlayer;
using CalamityMod.Effects;
using CalamityMod.Events;
using CalamityMod.ILEditing;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Dyes.HairDye;
using CalamityMod.Localization;
using CalamityMod.NPCs;
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
using CalamityMod.Projectiles.Summon;
using CalamityMod.Schematics;
using CalamityMod.Skies;
using CalamityMod.TileEntities;
using CalamityMod.Tiles.DraedonStructures;
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
            BossRushEvent.Load();
            thorium = ModLoader.GetMod("ThoriumMod");

            BossHealthBarManager.Load(this);

            CalamityLists.LoadLists();
            SetupVanillaDR();
            SetupBossKillTimes();
            SetupBossVelocityScalingValues();
            SetupThoriumBossDR(thorium);

            CalamityLocalization.AddLocalizations();

            On.Terraria.Player.TileInteractionsUse += Player_TileInteractionsUse;
            On.Terraria.WorldGen.OpenDoor += LabDoorsOpen;
            On.Terraria.WorldGen.CloseDoor += LabDoorsClose;
        }

        private static bool LabDoorsOpen(On.Terraria.WorldGen.orig_OpenDoor orig, int i, int j, int direction)
        {
            Tile tile = Main.tile[i, j];
            if (tile.type == ModContent.TileType<AgedLaboratoryDoorOpen>() || tile.type == ModContent.TileType<AgedLaboratoryDoorClosed>() ||
            tile.type == ModContent.TileType<LaboratoryDoorOpen>() || tile.type == ModContent.TileType<LaboratoryDoorClosed>())
            {
                return false;
            }
            else
            {
                return orig(i, j, direction);
            }
        }

        private static bool LabDoorsClose(On.Terraria.WorldGen.orig_CloseDoor orig, int i, int j, bool forced)
        {
            Tile tile = Main.tile[i, j];
            if (tile.type == ModContent.TileType<AgedLaboratoryDoorOpen>() || tile.type == ModContent.TileType<AgedLaboratoryDoorClosed>() ||
            tile.type == ModContent.TileType<LaboratoryDoorOpen>() || tile.type == ModContent.TileType<LaboratoryDoorClosed>())
            {
                return false;
            }
            else
            {
                return orig(i, j, forced);
            }
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

        public static void DoorSwap(int type1, int type2, int i, int j, bool forced = false)
        {
            if (PlayerInput.Triggers.JustPressed.MouseRight || forced)
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

            AddEquipTexture(new OmegaBlueTransformationHead(), null, EquipType.Head, "OmegaBlueTransformationHead", "CalamityMod/Items/Armor/OmegaBlueHelmet_HeadMadness");

            AddEquipTexture(new AbyssDivingGearHair(), null, EquipType.Head, "AbyssDivingGearHead", "CalamityMod/Items/Accessories/AbyssalDivingGear_Face");

            AstralCactusTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Tiles/AstralCactus");
            AstralCactusGlowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Tiles/AstralCactusGlow");
            AstralSky = ModContent.GetTexture("CalamityMod/ExtraTextures/AstralSky");

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

            SkyManager.Instance["CalamityMod:Astral"] = new AstralSky();
            SkyManager.Instance["CalamityMod:Cryogen"] = new CryogenSky();

            CalamityShaders.LoadShaders();

            RipperUI.Reset();
            AstralArcanumUI.Load(this);

            GameShaders.Hair.BindShader(ModContent.ItemType<AdrenalineHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(0, 255, 171), ((float)player.Calamity().adrenaline / (float)player.Calamity().adrenalineMax))));
            GameShaders.Hair.BindShader(ModContent.ItemType<RageHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(255, 83, 48), ((float)player.Calamity().rage / (float)player.Calamity().rageMax))));
            GameShaders.Hair.BindShader(ModContent.ItemType<WingTimeHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(139, 205, 255), ((float)player.wingTime / (float)player.wingTimeMax))));
            GameShaders.Hair.BindShader(ModContent.ItemType<StealthHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(186, 85, 211), (player.Calamity().rogueStealth / player.Calamity().rogueStealthMax))));

            SchematicLoader.LoadEverything();

            PopupGUIManager.LoadGUIs();
            InvasionProgressUIManager.LoadGUIs();
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

            CalamityLists.UnloadLists();

            thorium = null;
            fargosMutant = false;

            Instance = null;

            PopupGUIManager.UnloadGUIs();
            InvasionProgressUIManager.UnloadGUIs();
            BossRushEvent.Unload();
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
        public override void MidUpdateTimeWorld() =>  TileEntityTimeHandler.Update();
        #endregion
    }

    public enum Season : byte
    {
        Winter,
        Spring,
        Summer,
        Fall
    }
}
