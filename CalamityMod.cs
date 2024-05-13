using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Cooldowns;
using CalamityMod.DataStructures;
using CalamityMod.Effects;
using CalamityMod.Events;
using CalamityMod.FluidSimulation;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items;
using CalamityMod.Items.Dyes.HairDye;
using CalamityMod.Items.VanillaArmorChanges;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
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
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Particles;
using CalamityMod.Projectiles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Schematics;
using CalamityMod.Skies;
using CalamityMod.UI;
using CalamityMod.UI.CalamitasEnchants;
using CalamityMod.UI.DraedonsArsenal;
using CalamityMod.UI.Rippers;
using CalamityMod.Waters;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Dyes;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

[assembly: InternalsVisibleTo("CalTestHelpers")]
[assembly: InternalsVisibleTo("InfernumMode")]
namespace CalamityMod
{
    public class CalamityMod : Mod
    {
        // TODO -- A huge amount of random floating variables exist here.
        // These should all be moved to other files, whether that's CalamityLists or brand new ModSystems.
        // It is best to have a ton of small ModSystems.

        // Boss Spawners
        public static int ghostKillCount = 0;
        public static int sharkKillCount = 0;

        public static Asset<Texture2D> carpetOriginal;

        // Astral Sky/BG
        public static Texture2D AstralSky;
        public static Texture2D AstralSurfaceFront;
        public static Texture2D AstralSurfaceFrontGlow;
        public static Texture2D AstralSurfaceClose;
        public static Texture2D AstralSurfaceCloseGlow;
        public static Texture2D AstralSurfaceMiddle;
        public static Texture2D AstralSurfaceMiddleGlow;

        // Astral Desert Sky/BG
        public static Texture2D AstralDesertSurfaceClose;
        public static Texture2D AstralDesertSurfaceMiddle;

        // Astral Snow Sky/BG
        public static Texture2D AstralSnowSurfaceMiddle;

        // Sulpher Sea Sky/BG
        public static Texture2D SulphurSeaSky;
        public static Texture2D SulphurSeaSkyFront;
        public static Texture2D SulphurSeaSurface;

        // Destroyer glowmasks
        public static Asset<Texture2D>[] DestroyerGlowmasks = new Asset<Texture2D>[3];

        // Wall of Flesh glowmasks
        public static Asset<Texture2D> WallOfFleshEyeGlowmask;
        public static Asset<Texture2D> WallOfFleshDemonSickleTexture;

        // Master Rev+ Skeletron Prime
        public static Asset<Texture2D> ChadPrime;
        public static Asset<Texture2D> ChadPrimeEyeGlowmask;

        // DR data structure
        public static SortedDictionary<int, float> DRValues;

        // Boss Kill Time data structure
        public static SortedDictionary<int, int> bossKillTimes;

        // Speedrun timer
        internal static Stopwatch SpeedrunTimer = new Stopwatch();

        // External flag to disable non-Revengeance boss AI edits
        // This can be edited by other mods using reflection to prevent compatibility issues
        public static bool ExternalFlag_DisableNonRevBossAI = false;

        internal static CalamityMod Instance;

        // TODO -- Mod references should be contained in a ModSystem (example name "ModLoadedChecker")
        internal Mod musicMod = null; // This is Calamity's official music mod, CalamityModMusic
        internal bool MusicAvailable => !(musicMod is null);

        // Please keep this in alphabetical order so it's easy to read
        internal Mod ancientsAwakened = null;
        internal Mod bossChecklist = null;
        internal Mod coloredDamageTypes = null;
        internal Mod crouchMod = null;
        internal Mod dialogueTweak = null;
        internal Mod fargos = null;
        internal Mod luminance = null;
        internal Mod magicStorage = null;
        internal Mod overhaul = null;
        internal Mod redemption = null;
        internal Mod soa = null;
        internal Mod subworldLibrary = null;
        internal Mod summonersAssociation = null;
        internal Mod thorium = null;
        internal Mod varia = null;
        internal Mod wikithis = null;

        //hell background
        //private List<HellBGLoad> loadCache;

        #region Load
        public override void Load()
        {
            Instance = this;

            carpetOriginal = TextureAssets.FlyingCarpet;

            // If any of these mods aren't loaded, it will simply keep them as null.
            musicMod = null;
            ModLoader.TryGetMod("CalamityModMusic", out musicMod);
            ancientsAwakened = null;
            ModLoader.TryGetMod("AAMod", out ancientsAwakened);
            bossChecklist = null;
            ModLoader.TryGetMod("BossChecklist", out bossChecklist);
            coloredDamageTypes = null;
            ModLoader.TryGetMod("ColoredDamageTypes", out coloredDamageTypes);
            crouchMod = null;
            ModLoader.TryGetMod("CrouchMod", out crouchMod);
            dialogueTweak = null;
            ModLoader.TryGetMod("DialogueTweak", out dialogueTweak);
            fargos = null;
            ModLoader.TryGetMod("Fargowiltas", out fargos);
            luminance = null;
            ModLoader.TryGetMod("Luminance", out luminance);
            magicStorage = null;
            ModLoader.TryGetMod("MagicStorage", out magicStorage);
            overhaul = null;
            ModLoader.TryGetMod("TerrariaOverhaul", out overhaul);
            redemption = null;
            ModLoader.TryGetMod("Redemption", out redemption);
            soa = null;
            ModLoader.TryGetMod("SacredTools", out soa);
            subworldLibrary = null;
            ModLoader.TryGetMod("SubworldLibrary", out subworldLibrary);
            summonersAssociation = null;
            ModLoader.TryGetMod("SummonersAssociation", out summonersAssociation);
            thorium = null;
            ModLoader.TryGetMod("ThoriumMod", out thorium);
            varia = null;
            ModLoader.TryGetMod("Varia", out varia);
            wikithis = null;
            ModLoader.TryGetMod("Wikithis", out wikithis);

            // Initialize the EnemyStats struct as early as it is safe to do so
            NPCStats.Load();

            // Initialize Calamity Lists so they may be used elsewhere immediately
            CalamityLists.LoadLists();

            // Initialize Calamity Balance, since it is tightly coupled with the remaining lists
            CalamityGlobalItem.LoadTweaks();
            CalamityGlobalProjectile.LoadTweaks();

            // Mount balancing occurs during runtime and is undone when Calamity is unloaded.
            Mount.mounts[MountID.Unicorn].dashSpeed *= CalamityPlayer.UnicornSpeedNerfPower;
            Mount.mounts[MountID.Unicorn].runSpeed *= CalamityPlayer.UnicornSpeedNerfPower;

            // Buff DCU's pickaxe power to equal PML pickaxe capabilities
            Mount.drillPickPower = 225;

            // Make Graveyard biomes require more Gravestones
            SceneMetrics.GraveyardTileMax = 60;
            SceneMetrics.GraveyardTileMin = 40;
            SceneMetrics.GraveyardTileThreshold = 52;

            if (!Main.dedServ)
            {
                LoadClient();
                GeneralParticleHandler.Load();
                PrimitiveRenderer.Initialize();
                ForegroundDrawing.ForegroundManager.Load();

                // Wikithis support
                WeakReferenceSupport.WikiThisSupport();
            }

            CooldownRegistry.Load();
            BossRushEvent.Load();
            // TODO -- As ModBossBarStyle is a ModType, its Load function does not need to be called directly here.
            BossHealthBarManager.Load(this);
            EnchantmentManager.LoadAllEnchantments();
            VanillaArmorChangeManager.Load();
            SetupVanillaDR();
            SetupBossKillTimes();
            SchematicManager.Load();
            CustomLavaManagement.Load();
            Attunement.Load();
            BalancingChangesManager.Load();
            BaseIdleHoldoutProjectile.LoadAll();
            PlayerDashManager.Load();

            /*
            //keep this disabled for now, hell bg system isnt used and there is a better way to load it
            //hell background loading
            HellBGManager.Load();

            //load stuff for hell background
            loadCache = new List<HellBGLoad>();

            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(HellBGLoad)))
                {
                    var instance = Activator.CreateInstance(type);
                    loadCache.Add(instance as HellBGLoad);
                }
            }

            for (int k = 0; k < loadCache.Count; k++)
            {
                loadCache[k].Load();
            }
            */
        }

        private void LoadClient()
        {
            // Astral Sky/BG
            AstralSky = ModContent.Request<Texture2D>("CalamityMod/Skies/AstralSky", AssetRequestMode.ImmediateLoad).Value;
            AstralSurfaceFront = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceFront", AssetRequestMode.ImmediateLoad).Value;
            AstralSurfaceFrontGlow = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceFrontGlow", AssetRequestMode.ImmediateLoad).Value;
            AstralSurfaceClose = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceClose", AssetRequestMode.ImmediateLoad).Value;
            AstralSurfaceCloseGlow = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceCloseGlow", AssetRequestMode.ImmediateLoad).Value;
            AstralSurfaceMiddle = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceMiddle", AssetRequestMode.ImmediateLoad).Value;
            AstralSurfaceMiddleGlow = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSurfaceMiddleGlow", AssetRequestMode.ImmediateLoad).Value;

            //Astral Desert Sky/BG
            AstralDesertSurfaceClose = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralDesertSurfaceClose", AssetRequestMode.ImmediateLoad).Value;
            AstralDesertSurfaceMiddle = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralDesertSurfaceMiddle", AssetRequestMode.ImmediateLoad).Value;

            //Astral Snow Sky/BG
            AstralSnowSurfaceMiddle = ModContent.Request<Texture2D>("CalamityMod/Backgrounds/AstralSnowSurfaceMiddle", AssetRequestMode.ImmediateLoad).Value;

            // Sulpher Sea Sky/BG
            SulphurSeaSky = ModContent.Request<Texture2D>("CalamityMod/Skies/SulphurSeaSky", AssetRequestMode.ImmediateLoad).Value;
            SulphurSeaSkyFront = ModContent.Request<Texture2D>("CalamityMod/Skies/SulphurSeaSkyFront", AssetRequestMode.ImmediateLoad).Value;
            SulphurSeaSurface = ModContent.Request<Texture2D>("CalamityMod/Skies/SulphurSeaSurface", AssetRequestMode.ImmediateLoad).Value;

            // Destroyer glowmasks
            DestroyerGlowmasks[0] = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBossGlowmasks/DestroyerHeadGlow", AssetRequestMode.AsyncLoad);
            DestroyerGlowmasks[1] = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBossGlowmasks/DestroyerBodyGlow", AssetRequestMode.AsyncLoad);
            DestroyerGlowmasks[2] = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBossGlowmasks/DestroyerTailGlow", AssetRequestMode.AsyncLoad);

            // Wall of Flesh glowmasks
            WallOfFleshEyeGlowmask = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/VanillaBossGlowmasks/WallOfFleshEyeTelegraphGlow", AssetRequestMode.AsyncLoad);
            WallOfFleshDemonSickleTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/ForbiddenOathbladeProjectile", AssetRequestMode.AsyncLoad);

            // Master Rev+ Skeletron Prime textures
            ChadPrime = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ChadPrime", AssetRequestMode.AsyncLoad);
            ChadPrimeEyeGlowmask = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ChadPrimeHeadGlow", AssetRequestMode.AsyncLoad);

            // TODO -- Sky shaders should probably be loaded in a ModSystem
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

            Filters.Scene["CalamityMod:SupremeCalamitas"] = new Filter(new SCalScreenShaderData("FilterMiniTower").UseColor(Color.Transparent).UseOpacity(0f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:SupremeCalamitas"] = new SCalSky();

            Filters.Scene["CalamityMod:Signus"] = new Filter(new SignusScreenShaderData("FilterMiniTower").UseColor(0.35f, 0.1f, 0.55f).UseOpacity(0.35f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:Signus"] = new SignusSky();

            Filters.Scene["CalamityMod:BossRush"] = new Filter(new BossRushScreenShader("FilterMiniTower").UseOpacity(0.75f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:BossRush"] = new BossRushSky();

            Filters.Scene["CalamityMod:ExoMechs"] = new Filter(new ExoMechsScreenShaderData("FilterMiniTower").UseColor(ExoMechsSky.DrawColor).UseOpacity(0.25f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:ExoMechs"] = new ExoMechsSky();

            Filters.Scene["CalamityMod:MonolithAccursed"] = new Filter(new MonolithScreenShaderData("FilterMiniTower").UseColor(1.1f, 0.3f, 0.3f).UseOpacity(0.65f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:MonolithAccursed"] = new MonolithSky();

            // Normal intensity is 4f
            Texture2D DistortionTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/BlobbyNoise", AssetRequestMode.ImmediateLoad).Value;
            Filters.Scene["CalamityMod:DrunkCrabulon"] = new Filter(new DrunkCrabScreenShaderData("FilterHeatDistortion").UseImage(DistortionTexture, 0, null).UseIntensity(20f), EffectPriority.VeryHigh);

            Filters.Scene["CalamityMod:BrimstoneCrag"] = new Filter(new MonolithScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:BrimstoneCrag"] = new BrimstoneCragSky();

            SkyManager.Instance["CalamityMod:Astral"] = new AstralSky();
            SkyManager.Instance["CalamityMod:SulphurSea"] = new SulphurSeaSky();
            SkyManager.Instance["CalamityMod:Cryogen"] = new CryogenSky();
            SkyManager.Instance["CalamityMod:StormWeaverFlash"] = new StormWeaverFlashSky();

            CalamityShaders.LoadShaders();

            // This must be done separately from immediate loading, as loading is now multithreaded.
            // However, render targets and certain other graphical objects can only be created on the main thread.
            Main.QueueMainThreadAction(() =>
            {
                Main.OnPreDraw += PrepareRenderTargets;
            });

            RipperUI.Load();
            StealthUI.Load();
            ChargeMeterUI.Load();
            FlightBar.Load();

            // TODO -- Is this not possible to place in ModNPC.Load or ModNPC.SetStaticDefaults ?
            // Centralizing head texture registration like this seems absurdly stiff
            Apollo.LoadHeadIcons();
            Artemis.LoadHeadIcons();
            Cryogen.LoadHeadIcons();
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

            // TODO -- Is this not possible to place in ModItem.Load or ModItem.SetStaticDefaults ?
            // Centralizing hair dye shaders like this seems absurdly stiff
            GameShaders.Hair.BindShader(ModContent.ItemType<AdrenalineHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(0, 255, 171), ((float)player.Calamity().adrenaline / (float)player.Calamity().adrenalineMax))));
            GameShaders.Hair.BindShader(ModContent.ItemType<RageHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) => Color.Lerp(player.hairColor, new Color(255, 83, 48), ((float)player.Calamity().rage / (float)player.Calamity().rageMax))));
            GameShaders.Hair.BindShader(ModContent.ItemType<WingTimeHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) =>
            {
                float flightTimeInterpolant = player.wingTime / player.wingTimeMax;
                if (player.mount.Active)
                    flightTimeInterpolant = 1f;
                else if (float.IsInfinity(flightTimeInterpolant) || float.IsNaN(flightTimeInterpolant))
                    flightTimeInterpolant = 0f;

                return Color.Lerp(player.hairColor, new Color(139, 205, 255), flightTimeInterpolant);
            }));
            GameShaders.Hair.BindShader(ModContent.ItemType<StealthHairDye>(), new LegacyHairShaderData().UseLegacyMethod((Player player, Color newColor, ref bool lighting) =>
            {
                float stealthInterpolant = player.Calamity().rogueStealth / player.Calamity().rogueStealthMax;
                if (float.IsInfinity(stealthInterpolant) || float.IsNaN(stealthInterpolant))
                    stealthInterpolant = 0f;

                return Color.Lerp(player.hairColor, new Color(186, 85, 211), stealthInterpolant);
            }));

            InvasionProgressUIManager.LoadGUIs();
        }
        #endregion

        #region Unload
        public override void Unload()
        {
            musicMod = null;

            ancientsAwakened = null;
            bossChecklist = null;
            coloredDamageTypes = null;
            crouchMod = null;
            dialogueTweak = null;
            fargos = null;
            luminance = null;
            magicStorage = null;
            overhaul = null;
            redemption = null;
            soa = null;
            subworldLibrary = null;
            summonersAssociation = null;
            thorium = null;
            varia = null;
            wikithis = null;

            AstralSky = null;

            DRValues?.Clear();
            DRValues = null;
            bossKillTimes?.Clear();
            bossKillTimes = null;

            BalancingChangesManager.Unload();
            Attunement.Unload();
            EnchantmentManager.UnloadAllEnchantments();
            VanillaArmorChangeManager.Unload();
            CalamityLists.UnloadLists();
            NPCStats.Unload();
            CalamityGlobalItem.UnloadTweaks();
            CalamityGlobalProjectile.UnloadTweaks();

            PopupGUIManager.UnloadGUIs();
            InvasionProgressUIManager.UnloadGUIs();
            BossRushEvent.Unload();
            SchematicManager.Unload();
            CustomLavaManagement.Unload();
            CooldownRegistry.Unload();
            PlayerDashManager.Unload();

            TileFraming.Unload();

            Main.QueueMainThreadAction(() =>
            {
                Main.OnPreDraw -= PrepareRenderTargets;
            });

            RipperUI.Unload();
            StealthUI.Unload();
            ChargeMeterUI.Unload();
            FlightBar.Unload();

            if (!Main.dedServ)
            {
                TextureAssets.FlyingCarpet = carpetOriginal;
                GeneralParticleHandler.Unload();
            }

            Mount.mounts[MountID.Unicorn].dashSpeed /= CalamityPlayer.UnicornSpeedNerfPower;
            Mount.mounts[MountID.Unicorn].runSpeed /= CalamityPlayer.UnicornSpeedNerfPower;

            Mount.drillPickPower = 210;

            SceneMetrics.GraveyardTileMax = 36;
            SceneMetrics.GraveyardTileMin = 16;
            SceneMetrics.GraveyardTileThreshold = 28;

            carpetOriginal = null;

            /*
            //unload hell background stuff
            HellBGManager.Unload();

            if (loadCache != null)
            {
                foreach (var loadable in loadCache)
                {
                    loadable.Unload();
                }
            }

            loadCache = null;
            */

            Instance = null;
            base.Unload();
        }
        #endregion

        #region Render Target Management

        public static void PrepareRenderTargets(GameTime gameTime)
        {
            DeathAshParticle.PrepareRenderTargets();
            FluidFieldManager.Update();
        }
        #endregion Render Target Management

        #region Force ModConfig save (Reflection)
        internal static void SaveConfig(CalamityConfig cfg)
        {
            // There is no current way to manually save a mod configuration file in tModLoader.
            // The method which saves mod config files is private in ConfigManager, so reflection is used to invoke it.
            try
            {
                MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
                if (saveMethodInfo is not null)
                    saveMethodInfo.Invoke(null, new object[] { cfg });
                else
                    Instance.Logger.Error("TML ConfigManager.Save reflection failed. Method signature has changed. Notify Calamity Devs if you see this in your log.");
            }
            catch
            {
                Instance.Logger.Error("An error occurred while manually saving Calamity mod configuration. This may be due to a complex mod conflict. It is safe to ignore this error.");
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
                { NPCID.Deerclops, 0.05f },
                { NPCID.DD2Betsy, 0.1f },
                { NPCID.DD2OgreT2, 0.1f },
                { NPCID.DD2OgreT3, 0.15f },
                { NPCID.DeadlySphere, 0.4f },
                { NPCID.DiabolistRed, 0.2f },
                { NPCID.DiabolistWhite, 0.2f },
                { NPCID.DukeFishron, 0.15f },
                { NPCID.DungeonGuardian, 0.9f },
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
                { NPCID.HallowBoss, 0.15f },
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
                { NPCID.KingSlime, 5400 }, // 1:30 (90 seconds)
                { NPCID.EyeofCthulhu, 5400 }, // 1:30 (90 seconds)
                { NPCID.EaterofWorldsHead, 7200 }, // 2:00 (120 seconds)
                { NPCID.EaterofWorldsBody, 7200 },
                { NPCID.EaterofWorldsTail, 7200 },
                { NPCID.BrainofCthulhu, 7200 }, // 2:00 (120 seconds, total length of fight including Creepers phase)
                { NPCID.Creeper, 1800 }, // 0:30 (30 seconds, length of Creepers phase)
                { NPCID.Deerclops, 5400 }, // 1:30 (90 seconds)
                { NPCID.QueenBee, 7200 }, // 2:00 (120 seconds)
                { NPCID.SkeletronHead, 7200 }, // 2:00 (120 seconds)
                { NPCID.WallofFlesh, 7200 }, // 2:00 (120 seconds)
                { NPCID.WallofFleshEye, 7200 },
                { NPCID.QueenSlimeBoss, 7200 }, // 2:00 (120 seconds)
                { NPCID.Spazmatism, 10800 }, // 3:00 (180 seconds)
                { NPCID.Retinazer, 10800 },
                { NPCID.TheDestroyer, 10800 }, // 3:00 (180 seconds)
                { NPCID.TheDestroyerBody, 10800 },
                { NPCID.TheDestroyerTail, 10800 },
                { NPCID.SkeletronPrime, 10800 }, // 3:00 (180 seconds)
                { NPCID.Plantera, 10800 }, // 3:00 (180 seconds)
                { NPCID.HallowBoss, 10800 }, // 3:00 (180 seconds)
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
                { ModContent.NPCType<DesertScourgeHead>(), 5400 }, // 1:30 (90 seconds)
                { ModContent.NPCType<DesertScourgeBody>(), 5400 },
                { ModContent.NPCType<DesertScourgeTail>(), 5400 },
                { ModContent.NPCType<Crabulon>(), 5400 }, // 1:30 (90 seconds)
                { ModContent.NPCType<HiveMind>(), 7200 }, // 2:00 (120 seconds)
                { ModContent.NPCType<PerforatorHive>(), 7200 }, // 2:00 (120 seconds)
                { ModContent.NPCType<SlimeGodCore>(), 9000 }, // 2:30 (150 seconds) -- total length of Slime God fight
                { ModContent.NPCType<EbonianPaladin>(), 4500 }, // 1:15 (75 seconds)
                { ModContent.NPCType<CrimulanPaladin>(), 4500 }, // 1:15 (75 seconds)
                { ModContent.NPCType<SplitEbonianPaladin>(), 4500 }, // 1:15 (75 seconds) -- split slimes should spawn at 1:15 and die at around 2:30
                { ModContent.NPCType<SplitCrimulanPaladin>(), 4500 }, // 1:15 (75 seconds)
                { ModContent.NPCType<Cryogen>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<AquaticScourgeHead>(), 7200 }, // 2:00 (120 seconds)
                { ModContent.NPCType<AquaticScourgeBody>(), 7200 },
                { ModContent.NPCType<AquaticScourgeBodyAlt>(), 7200 },
                { ModContent.NPCType<AquaticScourgeTail>(), 7200 },
                { ModContent.NPCType<BrimstoneElemental>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<CalamitasClone>(), 14400 }, // 4:00 (240 seconds)
                { ModContent.NPCType<Anahita>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<Leviathan>(), 10800 },
                { ModContent.NPCType<AstrumAureus>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<AstrumDeusHead>(), 7200 }, // 2:00 (120 seconds) -- first phase is 1:00
                { ModContent.NPCType<AstrumDeusBody>(), 7200 },
                { ModContent.NPCType<AstrumDeusTail>(), 7200 },
                { ModContent.NPCType<PlaguebringerGoliath>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<RavagerBody>(), 10800 }, // 3:00 (180 seconds)
                { ModContent.NPCType<ProfanedGuardianCommander>(), 5400 }, // 1:30 (90 seconds)
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
                { ModContent.NPCType<DevourerofGodsBody>(), 14400 }, // DoG Phase 1 is 1:30, DoG Phase 2 is 2:30
                { ModContent.NPCType<DevourerofGodsTail>(), 14400 },
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
                { ModContent.NPCType<PrimordialWyrmHead>(), 18000 } // 5:00 (300 seconds)
            };
        }
        #endregion

        #region Music

        // This function returns an available Calamity Music Mod track, or null if the Calamity Music Mod is not available.
        public int? GetMusicFromMusicMod(string songFilename) => MusicAvailable ? MusicLoader.GetMusicSlot(musicMod, "Sounds/Music/" + songFilename) : null;

        #endregion

        #region Mod Support
        public override void PostSetupContent() => WeakReferenceSupport.Setup();

        public override object Call(params object[] args) => ModCalls.Call(args);
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
    }
}
