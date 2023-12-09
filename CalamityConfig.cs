using System.ComponentModel;
using System.Runtime.Serialization;
using CalamityMod.UI;
using CalamityMod.UI.DraedonsArsenal;
using CalamityMod.UI.Rippers;
using CalamityMod.UI.SulphurousWaterMeter;
using Terraria;
using Terraria.ModLoader.Config;

namespace CalamityMod
{
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
            ParticleLimit = (int)Utils.Clamp(ParticleLimit, MinParticleLimit, MaxParticleLimit);
        }

        #region Graphics Changes
        [Header("Graphics")]

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool Afterimages { get; set; }

        private const int MinParticleLimit = 100;
        private const int MaxParticleLimit = 2000;
        
        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(MinParticleLimit, MaxParticleLimit)]
        [DefaultValue(500)]
        public int ParticleLimit { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        public bool BossesStopWeather { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool Screenshake { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool StealthInvisibility { get; set; }
        #endregion

        #region UI Changes
        [Header("UI")]

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool MusicModReminderMessage { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool WikiStatusMessage { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool ShopNewAlert { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool BossHealthBarExtraInfo { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool DebuffDisplay { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 2f)]
        [DefaultValue(2f)]
        [Increment(1f)]
        [DrawTicks]
        public float CooldownDisplay { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool VanillaCooldownDisplay { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool MeterPosLock { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool StealthMeter { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(StealthUI.DefaultStealthPosX)]
        public float StealthMeterPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(StealthUI.DefaultStealthPosY)]
        public float StealthMeterPosY { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(SulphurousWaterMeterUI.DefaultPosX)]
        public float SulphuricWaterMeterPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(SulphurousWaterMeterUI.DefaultPosY)]
        public float SulphuricWaterMeterPosY { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool ChargeMeter { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(ChargeMeterUI.DefaultChargePosX)]
        public float ChargeMeterPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(ChargeMeterUI.DefaultChargePosY)]
        public float ChargeMeterPosY { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        public bool SpeedrunTimer { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(SpeedrunTimerUI.DefaultTimerPosX)]
        public float SpeedrunTimerPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(SpeedrunTimerUI.DefaultTimerPosY)]
        public float SpeedrunTimerPosY { get; set; }
        
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool FlightBar { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(UI.FlightBar.DefaultFlightPosX)]
        public float FlightBarPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(UI.FlightBar.DefaultFlightPosY)]
        public float FlightBarPosY { get; set; }
        #endregion

        #region General Gameplay Changes
        [Header("Gameplay")]

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool FasterFallHotkey { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool RemoveReforgeRNG { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool EarlyHardmodeProgressionRework { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool BossZen { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool PotionSelling { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        public bool TownNPCsSpawnAtNight { get; set; }

        private const int MinTownNPCSpawnMultiplier = 1;
        private const int MaxTownNPCSpawnMultiplier = 10;

        [BackgroundColor(192, 54, 64, 192)]
        [Range(MinTownNPCSpawnMultiplier, MaxTownNPCSpawnMultiplier)]
        [Increment(1)]
        [DrawTicks]
        [DefaultValue(MinTownNPCSpawnMultiplier)]
        public int TownNPCSpawnRateMultiplier { get; set; }

        private const int MinPlayerRespawnTime_BossAlive = 15;
        private const int MaxPlayerRespawnTime_BossAlive = 60;

        [BackgroundColor(192, 54, 64, 192)]
        [Range(MinPlayerRespawnTime_BossAlive, MaxPlayerRespawnTime_BossAlive)]
        [Increment(1)]
        [DrawTicks]
        [DefaultValue(MinPlayerRespawnTime_BossAlive)]
        public int PlayerRespawnTime_BossAlive { get; set; }

        private const float MinBossHealthBoost = 0f;
        private const float MaxBossHealthBoost = 900f;

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(MinBossHealthBoost, MaxBossHealthBoost)]
        [Increment(25f)]
        [DrawTicks]
        [DefaultValue(MinBossHealthBoost)]
        public float BossHealthBoost { get; set; }
        #endregion

        #region Default Player Stat Boosts
        [Header("BaseBoosts")]

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool DefaultDashEnabled { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool FasterBaseSpeed { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool HigherJumpHeight { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool FasterJumpSpeed { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool FasterTilePlacement { get; set; }
        #endregion

        #region Expert and Master Mode Changes
        [Header("ExpertMaster")]

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool NerfExpertDebuffs { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool ChilledWaterRework { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(false)]
        public bool ForceTownSafety { get; set; }
        #endregion

        #region Revengeance Mode Changes
        [Header("Revengeance")]

        private const float MinMeterShake = 0f;
        private const float MaxMeterShake = 4f;

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(MinMeterShake, MaxMeterShake)]
        [Increment(1f)]
        [DrawTicks]
        [DefaultValue(2f)]
        public float RipperMeterShake { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(RipperUI.DefaultRagePosX)]
        public float RageMeterPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(RipperUI.DefaultRagePosY)]
        public float RageMeterPosY { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(RipperUI.DefaultAdrenPosX)]
        public float AdrenalineMeterPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(RipperUI.DefaultAdrenPosY)]
        public float AdrenalineMeterPosY { get; set; }
        #endregion
    }
}
