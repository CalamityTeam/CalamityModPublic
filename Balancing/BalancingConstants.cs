namespace CalamityMod.Balancing
{
    public static class BalancingConstants
    {
        // When the relevant config is enabled: Gives the player a substantial +50% move speed boost at all times
        internal static readonly float DefaultMoveSpeedBoost = 0.5f;

        // When the relevant config is enabled: Increases the player's base jump height by 10%
        internal static readonly float ConfigBoostedBaseJumpHeight = 5.51f; // vanilla = 5.01f

        // When the relevant config is enabled: Allows the player to hold the Down button (S by default) to fast fall.
        // This is the multiplier for the player's gravity (downwards acceleration) while holding Down.
        internal static readonly float HoldingDownGravityMultiplier = 2f;

        // Altered jump speed boost provided by Shiny Red Balloon via IL edit
        // This is a const because it replaces a hardcoded value in vanilla
        internal const float BalloonJumpSpeedBoost = 0.75f;

        // Altered default damage deviation
        internal const int NewDefaultDamageVariationPercent = 5;

        // Sharpening Station grants this much armor penetration to melee weapons.
        internal const float SharpeningStationArmorPenetration = 5f;

        // Dash cooldowns (in frames)
        internal const int UniversalDashCooldown = 30;
        internal const int UniversalShieldSlamCooldown = 30;
        internal const int UniversalShieldBonkCooldown = 30;
        internal const int OnShieldBonkCooldown = 30;

        // Vanilla shield slam stats
        // These are consts because they replace hardcoded values in vanilla
        internal const int ShieldOfCthulhuBonkNoCollideFrames = 6;
        internal const int SolarFlareIFrames = 12;
        internal const float SolarFlareBaseDamage = 400f;

        // Dodge cooldowns (in frames)
        // TODO -- Some of these could be moved to the respective item files
        internal static readonly int BeltDodgeCooldown = 5400;
        internal static readonly int BrainDodgeCooldown = 5400;
        internal static readonly int AmalgamDodgeCooldown = 5400;
        internal static readonly int MirrorDodgeCooldown = 5400;
        internal static readonly int DaedalusReflectCooldown = 5400;
        internal static readonly int EvolutionReflectCooldown = 5400;

        // Internal vanilla item damage variables
        internal static readonly float LeatherWhipTagDamageMultiplier = 1.08f;
        internal static readonly float SnapthornTagDamageMultiplier = 1.04f;
        internal static readonly float SpinalTapTagDamageMultiplier = 1.08f;
        internal static readonly float FirecrackerExplosionDamageMultiplier = 2f; // Note: Lasts for 1 hit
        internal static readonly float CoolWhipTagDamageMultiplier = 1.08f;
        internal static readonly float DurendalTagDamageMultiplier = 1.09f;
        internal static readonly float MorningStarTagDamageMultiplier = 1.11f;
        internal static readonly float KaleidoscopeTagDamageMultiplier = 1.12f;

        // Summoner cross class nerf
        internal static readonly float SummonerCrossClassNerf = 0.75f;

        // Rogue stealth
        // If stealth is too weak, increase this number. If stealth is too strong, decrease this number.
        // This value is intentionally not readonly.
        public static double UniversalStealthStrikeDamageFactor = 0.42;
        // Shade 23/10/2023: So stealth apparently was indeed way too strong after the bugfix with nearly every weapon being way stronger than before
        // due to Flawless now working properly and thus the stealth factor was changed back to 0.42 from 0.5.
        // This nerf takes feedback from various players as well as my own personal experience with testing rogue stuff today; it feels too strong and
        // something needed to be done about it.

        internal static readonly float BaseStealthGenTime = 4f; // 4 seconds
        internal static readonly float MovingStealthGenRatio = 0.5f;

        // Rage
        internal static readonly int DefaultRageDuration = CalamityUtils.SecondsToFrames(9); // Rage lasts 9 seconds by default.
        internal static readonly int RageDurationPerBooster = CalamityUtils.SecondsToFrames(1); // Each booster is +1 second: 10, 11, 12.
        internal static readonly int RageCombatDelayTime = CalamityUtils.SecondsToFrames(10);
        internal static readonly int RageFadeTime = CalamityUtils.SecondsToFrames(30);
        internal static readonly float DefaultRageDamageBoost = 0.35f; // +35%

        // Proximity Rage
        // These variables should be used in general to classify "enemies" vs "non-enemies" as well.
        // See NPCUtils.IsAnEnemy
        internal const int TinyHealthThreshold = 5;
        internal const int TinyDamageThreshold = 5;
        // If an enemy has more health than this, they are considered an enemy even if they have 0 contact damage
        internal const int NoContactDamageHealthThreshold = 3000;
        internal const int UnreasonableHealthThreshold = 25000000; // 25 million

        // Adrenaline
        internal static readonly float AdrenalineDamageBoost = 2f; // +200%
        internal static readonly float AdrenalineDamagePerBooster = 0.15f; // +15%
        internal static readonly float FullAdrenalineDR = 0.5f; // 50%
        internal static readonly float AdrenalineDRPerBooster = 0.05f; // +5% per booster

        // Summon damage bonuses counting less towards "scales with your best class"
        internal static readonly float SummonAllClassScalingFactor = 0.75f;

        // Minimum and maximum allowed attack speed ratios when using Calamity Global Item Tweaks
        internal static readonly float MinimumAllowedAttackSpeed = 0.25f;
        internal static readonly float MaximumAllowedAttackSpeed = 10f;

        // Defense Damage
        internal const double DefaultDefenseDamageRatio = 0.3333;

        // Defense damage floor: PHM | HM | PML
        //
        // Normal/Expert: 3 |  8 | 16
        // Revengeance:   4 | 10 | 20
        // Death Mode:    5 | 12 | 24
        // Boss Rush:     25
        internal static readonly int DefenseDamageFloor_NormalPHM = 3;
        internal static readonly int DefenseDamageFloor_NormalHM  = 8;
        internal static readonly int DefenseDamageFloor_NormalPML = 16;
        internal static readonly int DefenseDamageFloor_RevPHM    = 4;
        internal static readonly int DefenseDamageFloor_RevHM     = 10;
        internal static readonly int DefenseDamageFloor_RevPML    = 20;
        internal static readonly int DefenseDamageFloor_DeathPHM  = 5;
        internal static readonly int DefenseDamageFloor_DeathHM   = 12;
        internal static readonly int DefenseDamageFloor_DeathPML  = 24;
        internal static readonly int DefenseDamageFloor_BossRush  = 25;

        // TODO -- Add all balance related constants here that don't belong in other files.
        // Review all constants and static readonlys in the entire mod to find things to add.
    }
}
