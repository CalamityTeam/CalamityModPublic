namespace CalamityMod.Balancing
{
    public static class BalancingConstants
    {
        #region Movement
        // When the relevant config is enabled: Multiplies the player's movement speed by 1.5x at all times
        internal static readonly float DefaultMoveSpeedBoost = 1.5f;

        // When the relevant config is enabled: Increases the player's base jump speed by 13.97% (allows you to jump 7 blocks)
        internal static readonly float ConfigBoostedBaseJumpSpeed = 5.71f; // vanilla = 5.01f

        // When the relevant config is enabled: Allows the player to hold the Down button (S by default) to fast fall.
        // This is the multiplier for the player's gravity (downwards acceleration) while holding Down.
        internal static readonly float HoldingDownGravityMultiplier = 2f;

        // Altered jump speed boost provided by Shiny Red Balloon via IL edit
        // This is a const because it replaces a hardcoded value in vanilla
        internal const float BalloonJumpSpeedBoost = 0.75f;

        // Default jump speed boost provided by Frog Leg and tinker combinations in vanilla
        internal const float VanillaFrogLegJumpSpeedBoost = 1.6f;

        // Altered jump speed boost provided by Amphibian Boots via global item UpdateAccessory
        //doze 23apr2025 - Undid nerf, it's now the same as frog leg. If it's a problem, maybe retier it?
        internal static readonly float AmphibianBootsJumpSpeedBoost = 1.6f;

        // Altered movement stats provided by Shadow Armor via IL edit
        // This item is modified to not stack with Magiluminescence if on the ground. If in the air, it still functions.
        internal static readonly float ShadowArmorRunAccelerationMultiplier = 1.25f; // Vanilla and Magi = 1.75f
        internal static readonly float ShadowArmorMaxRunSpeedMultiplier = 1.05f; // Vanilla and Magi = 1.15f
        internal static readonly float ShadowArmorAccRunSpeedMultiplier = 1.05f; // Vanilla and Magi = 1.15f
        internal static readonly float ShadowArmorRunSlowdownMultiplier = 1.5f; // Vanilla and Magi = 1.75f

        // Altered run acceleration multiplier provided by Soaring Insignia via IL edit
        // This is a const because it replaces a hardcoded value in vanilla
        internal const float SoaringInsigniaRunAccelerationMultiplier = 1.25f; // Vanilla = 1.75f
        #endregion

        #region Immunity Frames
        // Default immunity frame rules
        internal const int VanillaDefaultIFrames = 40;
        internal const int VanillaParryIFrames = 60;
        internal const int VanillaDodgeIFrames = 80;

        // In vanilla, the Cross Necklace grants +40 iframes to all hits except for in PvP (where it does nothing)
        // and Brand of the Inferno (where it only gives +30 instead).
        internal const int CrossNecklaceIFrameBoost = 40;
        internal const int CrossNecklaceIFrameBoost_Parry = 30;
        #endregion

        #region Dashes and Dodges
        // Dash cooldowns (in frames)
        internal const int UniversalDashCooldown = 30;
        internal const int UniversalSashDashCooldown = 30;
        internal const int UniversalShieldSlamCooldown = 30;
        internal const int UniversalShieldBonkCooldown = 30;
        internal const int OnShieldBonkCooldown = 30;

        // Vanilla shield slam stats
        // These are consts because they replace hardcoded values in vanilla
        internal const int ShieldOfCthulhuBonkNoCollideFrames = 6;
        internal const int SolarFlareIFrames = 12;
        internal const float SolarFlareBaseDamage = 400f;

        /// <summary>
        /// How long the dodge cooldown is at maximum damage
        /// </summary>
        internal static readonly int DodgeCooldownMax = 5400; // 90 seconds
        /// <summary>
        /// How much the dodge cooldown is reduced when at minimum damage
        /// </summary>
        internal static float DodgeCooldownDamageMult => 0.33f; // 30 seconds
        /// <summary>
        /// How much the dodge cooldown is reduced per stacked dodge. Each stack applies multiplicatively.
        /// </summary>
        internal static float DodgeCooldownMultPerStack => 0.85f; // -15% per stack, multiplicatively
        #endregion

        #region Damage
        // Altered default damage deviation
        internal const int NewDefaultDamageVariationPercent = 5;

        // Summoner cross class nerf
        internal static readonly float SummonerCrossClassNerf = 0.75f;

        // Summon damage bonuses counting less towards "scales with your best class"
        internal static readonly float SummonAllClassScalingFactor = 0.75f;

        // Minimum and maximum allowed attack speed ratios when using Calamity Global Item Tweaks
        internal static readonly float MinimumAllowedAttackSpeed = 0.25f;
        internal static readonly float MaximumAllowedAttackSpeed = 10f;

        // Internal vanilla whip damage variables
        internal static readonly float LeatherWhipTagDamageMultiplier = 1.08f;
        internal static readonly float SnapthornTagDamageMultiplier = 1.04f;
        internal static readonly float SpinalTapTagDamageMultiplier = 1.08f;
        internal static readonly float FirecrackerExplosionDamageMultiplier = 2f; // Note: Lasts for 1 hit
        internal static readonly float CoolWhipTagDamageMultiplier = 1.08f;
        internal static readonly float DurendalTagDamageMultiplier = 1.09f;
        internal static readonly float MorningStarTagDamageMultiplier = 1.11f;
        internal static readonly float KaleidoscopeTagDamageMultiplier = 1.12f;

        // Sharpening Station grants this much armor penetration to melee weapons.
        internal const float SharpeningStationArmorPenetration = 5f;

        // Beetle Scale Mail stats
        internal static readonly float BeetleScaleMailMeleeDamagePerBeetle = 0.1f;
        internal static readonly float BeetleScaleMailMeleeSpeedPerBeetle = 0.05f;

        // Nebula Armor Damage Booster
        internal static readonly float NebulaDamagePerBooster = 0.075f; // 0.15f in vanilla

        // Nebula Armor Mana Booster
        // nothing here yet
        #endregion

        #region Life Steal
        // Life steal cap (Maximum HP healed per instance of lifesteal)
        internal static readonly int LifeStealCap = 100;

        // Base life steal caps from vanilla (aka, how much life steal the player is allowed before it goes on cooldown)
        internal static readonly float LifeStealCap_Classic = 80f;
        internal static readonly float LifeStealCap_Expert = 70f;

        // The base life steal cooldowns from vanilla
        internal static readonly float LifeStealRecoveryRate_Classic = 0.6f;
        internal static readonly float LifeStealRecoveryRate_Expert = 0.5f;

        // Nerfs the life steal recovery rate in Classic from 0.6 to 0.15 (36 HP/s to 9 HP/s)
        internal static readonly float LifeStealRecoveryRateReduction_Classic = 0.45f;

        // Nerfs the life steal recovery rate in Expert from 0.5 to 0.1 (30 HP/s to 6 HP/s)
        internal static readonly float LifeStealRecoveryRateReduction_Expert = 0.4f;
        #endregion

        #region Rogue Base Stats
        // If stealth is too weak, increase this number. If stealth is too strong, decrease this number.
        // This value is intentionally not readonly.
        public static double UniversalStealthStrikeDamageFactor = 0.42;
        // Shade 23/10/2023: So stealth apparently was indeed way too strong after the bugfix with nearly every weapon being way stronger than before
        // due to Flawless now working properly and thus the stealth factor was changed back to 0.42 from 0.5.
        // This nerf takes feedback from various players as well as my own personal experience with testing rogue stuff today; it feels too strong and
        // something needed to be done about it.

        internal static readonly float BaseStealthGenTime = 4f; // 4 seconds
        internal static readonly float MovingStealthGenRatio = 0.5f;
        #endregion

        #region Defense, Health and Mana (Armor set stuff)
        // Beetle Shell's multiplicative DR is removed by Calamity. In compensation, you get this much regular DR per beetle.
        internal static readonly float BeetleShellDRPerBeetle = 0.1f;

        // Solar Flare Armor's multiplicative DR is removed by Calamity. In compensation, you get this much DR from having at least one solar shield up.
        internal static readonly float SolarFlareShieldDR = 0.25f;

        // Nebula Armor Life Regen
        internal static readonly int NebulaLifeRegenPerBooster = 4; // 6 in vanilla

        // Nebula Armor Mana Regen
        // This value works differently than you might expect. Raising it actually nerfs the mana regeneration of Nebula Armor
        // Every frame, Terraria adds 1 to a counter for each Mana Booster the player has.
        // If this counter reaches the below threshold, that player gains 1 mana.
        // By default, it's 6, which has the following effects:
        // 1 booster  = +1 mana every 6 frames = +10 mana per second
        // 2 boosters = +1 mana every 3 frames = +20 mana per second
        // 3 boosters = +1 mana every 2 frames = +30 mana per second
        //
        // Calamity just halves this, that's probably good enough.
        internal static readonly int NebulaManaRegenFrameCounterThreshold = 12; // 6 in vanilla
        #endregion

        #region Defense Damage
        internal const double DefaultDefenseDamageRatio = 0.3333;

        // Defense damage floor: PHM | HM | PML
        //
        // Normal/Expert: 3 |  8 | 16
        // Revengeance:   4 | 10 | 20
        // Death Mode:    5 | 12 | 24
        // Boss Rush:     25
        internal static readonly int DefenseDamageFloor_NormalPHM = 3;
        internal static readonly int DefenseDamageFloor_NormalHM = 8;
        internal static readonly int DefenseDamageFloor_NormalPML = 16;
        internal static readonly int DefenseDamageFloor_RevPHM = 4;
        internal static readonly int DefenseDamageFloor_RevHM = 10;
        internal static readonly int DefenseDamageFloor_RevPML = 20;
        internal static readonly int DefenseDamageFloor_DeathPHM = 5;
        internal static readonly int DefenseDamageFloor_DeathHM = 12;
        internal static readonly int DefenseDamageFloor_DeathPML = 24;
        internal static readonly int DefenseDamageFloor_BossRush = 25;
        #endregion

        #region Rage and Adrenaline (Rippers)
        internal static readonly int DefaultRageDuration = CalamityUtils.SecondsToFrames(9); // Rage lasts 9 seconds by default.
        internal static readonly int RageDurationPerBooster = CalamityUtils.SecondsToFrames(1); // Each booster is +1 second: 10, 11, 12.
        internal static readonly int RageCombatDelayTime = CalamityUtils.SecondsToFrames(10);
        internal static readonly int RageFadeTime = CalamityUtils.SecondsToFrames(30);
        internal static readonly float DefaultRageDamageBoost = 0.35f; // +35%

        internal static readonly float AdrenalineDamageBoost = 1.5f; // +150%
        internal static readonly float AdrenalineDamagePerBooster = 0.2f; // +20%
        internal static readonly float FullAdrenalineDR = 0.5f; // 50%
        internal static readonly float AdrenalineDRPerBooster = 0.05f; // +5% per booster

        internal static readonly int AdrenalinePauseAfterDamage = CalamityUtils.SecondsToFrames(1);
        internal static readonly float MinimumAdrenalineLoss = 0.25f; // No matter how small a hit, you will always lose at least 25% current Adrenaline
        internal static readonly float AdrenalineFalloffTinyHitHealthRatio = 0.05f; // Hits for 5% max HP or less result in less Adrenaline loss

        internal static readonly float TrueMeleeRipperReductionFactor = 0.5f; // True melee benefits less from rippers to prevent excessive melting.
        #endregion

        #region Pierce Resistance
        // Both of the below variables only apply when an NPC has universal pierce resistance enabled via attribute.
        // Projectiles may be marked as exempt from this pierce resistance by using an attribute.

        /// <summary> Constant variable used to determine the percentage a projectile's damage is reduced by pierce resist on each hit. </summary>
        public const float PierceResistHarshness = 0.12f;
        /// <summary> Constant variable used to determine the maximum percentage a projectile's damage can be reduced by pierce resist. </summary>
        public const float PierceResistCap = 0.8f;
        #endregion

        // TODO -- NPC classification is not done consistently with predictable thresholds.
        // These variables should be used in general to classify "enemies" vs "non-enemies" as well.
        // See NPCUtils.IsAnEnemy
        #region NPC Classification
        internal const int TinyHealthThreshold = 5;
        internal const int TinyDamageThreshold = 5;
        // If an enemy has more health than this, they are considered an enemy even if they have 0 contact damage
        internal const int NoContactDamageHealthThreshold = 3000;
        internal const int UnreasonableHealthThreshold = 25000000; // 25 million
        #endregion

        #region Multiplayer Boss Health Scaling Overrides
        // These values are used to replace the vanilla 1.35f and 1.9166666666666666f for the 2-player and 3-player cases, respectively.
        // Editing them via reflection or otherwise will apply to newly spawned bosses during gameplay.
        public static float ExpertHealthScalingOverride_2Players = 1.75f;
        public static float ExpertHealthScalingOverride_3Players = 2.25f;
        #endregion

        // TODO -- Add all balance related constants here that don't belong in other files.
        // Review all constants and static readonlys in the entire mod to find things to add.
    }
}
