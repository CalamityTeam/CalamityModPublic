using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Dyes.HairDye;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.UI;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using CalamityMod.Items.PermanentBoosters;

namespace CalamityMod
{
    [Label("$Mods.CalamityMod.Config.MainTitle")]
    [BackgroundColor(49, 32, 36, 216)]
    public class CalamityConfig : ModConfig
    {
        public static CalamityConfig Instance;

        // TODO -- Not all Calamity config settings should be considered client side.
        // There are many configs which are server side and should stay that way.
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message) => true;

        // Clamps values that would cause ugly problems if loaded directly without sanitization.
        [OnDeserialized]
        internal void ClampValues(StreamingContext context)
        {
            BossHealthBoost = Utils.Clamp(BossHealthBoost, MinBossHealthBoost, MaxBossHealthBoost);
            RipperMeterShake = Utils.Clamp(RipperMeterShake, MinMeterShake, MaxMeterShake);
        }

        #region Graphics Changes
        [Header("$Mods.CalamityMod.Config.SectionTitle.Graphics")]

        [Label("$Mods.CalamityMod.Config.EntryTitle.Afterimages")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.Afterimages")]
        public bool Afterimages { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.ParticleLimit")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0, 1000)]
        [DefaultValue(500)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.ParticleLimit")]
        public int ParticleLimit { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.Screenshake")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.Screenshake")]
        public bool Screenshake { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.StealthInvisibility")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.StealthInvisibility")]
        public bool StealthInvisibility { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.ShopNewAlert")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.ShopNewAlert")]
        public bool ShopNewAlert { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.WikiStatusMessage")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.WikiStatusMessage")]
        public bool WikiStatusMessage { get; set; }
        #endregion

        #region UI Changes
        [Header("$Mods.CalamityMod.Config.SectionTitle.UI")]

        [Label("$Mods.CalamityMod.Config.EntryTitle.BossHealthBarExtraInfo")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.BossHealthBarExtraInfo")]
        public bool BossHealthBarExtraInfo { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.DebuffDisplay")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.DebuffDisplay")]
        public bool DebuffDisplay { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.CooldownDisplay")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 2f)]
        [DefaultValue(2f)]
        [Increment(1f)]
        [DrawTicks]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.CooldownDisplay")]
        public float CooldownDisplay { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.VanillaCooldownDisplay")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.VanillaCooldownDisplay")]
        public bool VanillaCooldownDisplay { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [Label("$Mods.CalamityMod.Config.EntryTitle.MeterPosLock")]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.MeterPosLock")]
        public bool MeterPosLock { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.StealthMeter")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.StealthMeter")]
        public bool StealthMeter { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.StealthMeterPosX")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(StealthUI.DefaultStealthPosX)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.StealthMeterPosX")]
        public float StealthMeterPosX { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.StealthMeterPosY")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(StealthUI.DefaultStealthPosY)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.StealthMeterPosY")]
        public float StealthMeterPosY { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.ChargeMeter")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.ChargeMeter")]
        public bool ChargeMeter { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.ChargeMeterPosX")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(ChargeMeterUI.DefaultChargePosX)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.ChargeMeterPosX")]
        public float ChargeMeterPosX { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.ChargeMeterPosY")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(ChargeMeterUI.DefaultChargePosY)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.ChargeMeterPosY")]
        public float ChargeMeterPosY { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.SpeedrunTimer")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.SpeedrunTimer")]
        public bool SpeedrunTimer { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.SpeedrunTimerPosX")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(SpeedrunTimerUI.DefaultTimerPosX)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.SpeedrunTimerPosX")]
        public float SpeedrunTimerPosX { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.SpeedrunTimerPosY")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(SpeedrunTimerUI.DefaultTimerPosY)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.SpeedrunTimerPosY")]
        public float SpeedrunTimerPosY { get; set; }
        #endregion

        #region General Gameplay Changes
        [Header("$Mods.CalamityMod.Config.SectionTitle.Gameplay")]

        [Label("$Mods.CalamityMod.Config.EntryTitle.RemoveReforgeRNG")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.RemoveReforgeRNG")]
        public bool RemoveReforgeRNG { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.EarlyHardmodeProgressionRework")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.EarlyHardmodeProgressionRework")]
        public bool EarlyHardmodeProgressionRework { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.BossZen")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.BossZen")]
        public bool BossZen { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.TownNPCsSpawnAtNight")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.TownNPCsSpawnAtNight")]
        public bool TownNPCsSpawnAtNight { get; set; }

        private const int MinTownNPCSpawnMultiplier = 1;
        private const int MaxTownNPCSpawnMultiplier = 10;

        [Label("$Mods.CalamityMod.Config.EntryTitle.TownNPCSpawnRateMultiplier")]
        [BackgroundColor(192, 54, 64, 192)]
        [Range(MinTownNPCSpawnMultiplier, MaxTownNPCSpawnMultiplier)]
        [Increment(1)]
        [DrawTicks]
        [DefaultValue(MinTownNPCSpawnMultiplier)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.TownNPCSpawnRateMultiplier")]
        public int TownNPCSpawnRateMultiplier { get; set; }

        private const float MinBossHealthBoost = 0f;
        private const float MaxBossHealthBoost = 900f;

        [Label("$Mods.CalamityMod.Config.EntryTitle.BossHealthBoost")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(MinBossHealthBoost, MaxBossHealthBoost)]
        [Increment(25f)]
        [DrawTicks]
        [DefaultValue(MinBossHealthBoost)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.BossHealthBoost")]
        public float BossHealthBoost { get; set; }
        #endregion

        #region Default Player Stat Boosts
        [Header("$Mods.CalamityMod.Config.SectionTitle.BaseBoosts")]

        [Label("$Mods.CalamityMod.Config.EntryTitle.FasterBaseSpeed")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.FasterBaseSpeed")]
        public bool FasterBaseSpeed { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.HigherJumpHeight")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.HigherJumpHeight")]
        public bool HigherJumpHeight { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.FasterJumpSpeed")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.FasterJumpSpeed")]
        public bool FasterJumpSpeed { get; set; }


        [Label("$Mods.CalamityMod.Config.EntryTitle.FasterFallHotkey")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.FasterFallHotkey")]
        public bool FasterFallHotkey { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.FasterTilePlacement")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.FasterTilePlacement")]
        public bool FasterTilePlacement { get; set; }
        #endregion

        #region Expert and Master Mode Changes
        [Header("$Mods.CalamityMod.Config.SectionTitle.ExpertMaster")]

        [Label("$Mods.CalamityMod.Config.EntryTitle.NerfExpertDebuffs")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.NerfExpertDebuffs")]
        public bool NerfExpertDebuffs { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.ChilledWaterRework")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.ChilledWaterRework")]
        public bool ChilledWaterRework { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.ForceTownSafety")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.ForceTownSafety")]
        public bool ForceTownSafety { get; set; }
        #endregion

        #region Revengeance Mode Changes
        [Header("$Mods.CalamityMod.Config.SectionTitle.Revengeance")]

        private const float MinMeterShake = 0f;
        private const float MaxMeterShake = 4f;

        [Label("$Mods.CalamityMod.Config.EntryTitle.RipperMeterShake")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(MinMeterShake, MaxMeterShake)]
        [Increment(1f)]
        [DrawTicks]
        [DefaultValue(2f)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.RipperMeterShake")]
        public float RipperMeterShake { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.RageMeterPosX")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(RipperUI.DefaultRagePosX)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.RageMeterPosX")]
        public float RageMeterPosX { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.RageMeterPosY")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(RipperUI.DefaultRagePosY)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.RageMeterPosY")]
        public float RageMeterPosY { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.AdrenalineMeterPosX")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(RipperUI.DefaultAdrenPosX)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.AdrenalineMeterPosX")]
        public float AdrenalineMeterPosX { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.AdrenalineMeterPosY")]
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(RipperUI.DefaultAdrenPosY)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.AdrenalineMeterPosY")]
        public float AdrenalineMeterPosY { get; set; }
        #endregion

        #region Boss Rush Curses
        [Header("$Mods.CalamityMod.Config.SectionTitle.BossRushCurses")]

        [Label("$Mods.CalamityMod.Config.EntryTitle.BossRushRegenCurse")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.BossRushRegenCurse")]
        public bool BossRushRegenCurse { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.BossRushDashCurse")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.BossRushDashCurse")]
        public bool BossRushDashCurse { get; set; }

        [Label("$Mods.CalamityMod.Config.EntryTitle.BossRushIFrameCurse")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        [Tooltip("$Mods.CalamityMod.Config.EntryTooltip.BossRushIFrameCurse")]
        public bool BossRushIFrameCurse { get; set; }
        #endregion

        #region Dynamic Localization (Item Icon Injection)
        
        // Because it is impossible to know the item IDs of Calamity items until runtime,
        // and we have many config titles which use embedded Calamity items,
        // we need to dynamically declare those items and inject them as dynamic localization.
        internal static void RegisterDynamicLocalization()
        {
            static string EmbedItem(int itemID, string suffix = "") => $"[i:{itemID}] {suffix}";

            var configLabelItemEmbeds = new KeyValuePair<string, int>[]
            {
                new("Afterimages", ItemID.SteampunkGoggles),
                new("Screenshake", ModContent.ItemType<WavePounder>()),
                new("ParticleLimit", ItemID.FragmentStardust),
                new("StealthInvisibility", ModContent.ItemType<StealthHairDye>()),
                new("ShopNewAlert", ItemID.GoldChest),
                new("WikiStatusMessage", ItemID.Book),

                new("BossHealthBarExtraInfo", ModContent.ItemType<EncryptedSchematicPlanetoid>()),
                new("DebuffDisplay", ItemID.FlaskofIchor),
                new("CooldownDisplay", ModContent.ItemType<NebulousCore>()),
                new("VanillaCooldownDisplay", ItemID.HealingPotion),
                new("MeterPosLock", ItemID.GemLockTopaz),

                new("StealthMeter", ModContent.ItemType<EclipseMirror>()),
                new("StealthMeterPosX", ItemID.LaserRuler),
                new("StealthMeterPosY", ItemID.LaserRuler),

                new("ChargeMeter", ModContent.ItemType<DraedonPowerCell>()),
                new("ChargeMeterPosX", ItemID.LaserRuler),
                new("ChargeMeterPosY", ItemID.LaserRuler),

                new("SpeedrunTimer", ItemID.Stopwatch),
                new("SpeedrunTimerPosX", ItemID.LaserRuler),
                new("SpeedrunTimerPosY", ItemID.LaserRuler),

                new("RemoveReforgeRNG", ItemID.TinHammer),
                new("EarlyHardmodeProgressionRework", ItemID.Pwnhammer),
                new("BossZen", ModContent.ItemType<ZenPotion>()),
                new("TownNPCsSpawnAtNight", ItemID.ClothierVoodooDoll),
                new("TownNPCSpawnRateMultiplier", ItemID.GuideVoodooDoll),
                new("BossHealthBoost", ItemID.LifeCrystal),

                new("FasterBaseSpeed", ItemID.HermesBoots),
                new("HigherJumpHeight", ItemID.ShinyRedBalloon),
                new("FasterJumpSpeed", ItemID.FrogLeg),
                new("FasterFallHotkey", ModContent.ItemType<BallAndChain>()),
                new("FasterTilePlacement", ItemID.ArchitectGizmoPack),

                new("NerfExpertDebuffs", ItemID.AnkhCharm),
                new("ChilledWaterRework", ItemID.ArcticDivingGear),
                new("ForceTownSafety", ItemID.Sunflower),

                new("RipperMeterShake", ModContent.ItemType<RedLightningContainer>()),
                new("RageMeterPosX", ItemID.LaserRuler),
                new("RageMeterPosY", ItemID.LaserRuler),
                new("AdrenalineMeterPosX", ItemID.LaserRuler),
                new("AdrenalineMeterPosY", ItemID.LaserRuler),

                new("BossRushRegenCurse", ItemID.Shackle),
                new("BossRushDashCurse", ItemID.Shackle),
                new("BossRushIFrameCurse", ItemID.Shackle),
            };

            // TODO -- Next month's TML Stable will have this function be public
            MethodInfo getTrans = typeof(LocalizationLoader).GetMethod("GetOrCreateTranslation", BindingFlags.Static | BindingFlags.NonPublic, new[] { typeof(string), typeof(bool) });
            
            // Stopgap for if the function becomes public sooner
            if (getTrans is null)
                getTrans = typeof(LocalizationLoader).GetMethod("GetOrCreateTranslation", BindingFlags.Static | BindingFlags.Public, new[] { typeof(string), typeof(bool) });

            // If the reflection STILL somehow doesn't work, just give up and log an error
            if (getTrans is null)
            {
                CalamityMod.Instance.Logger.Error("LocalizationLoader reflection failed. Config will not contain icons!");
                return;
            }

            foreach (KeyValuePair<string, int> kv in configLabelItemEmbeds)
            {
                string localizationKey = $"Mods.CalamityMod.Config.EntryTitle.{kv.Key}";
                ModTranslation trans = getTrans.Invoke(null, new object[] { localizationKey, true }) as ModTranslation;
                var culture = Language.ActiveCulture;
                trans.AddTranslation(culture, EmbedItem(kv.Value, trans.GetTranslation(culture)));
            }
        }
        #endregion
    }
}
