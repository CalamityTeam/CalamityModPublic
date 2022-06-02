namespace CalamityMod.Balancing
{
    public static class BalancingConstants
    {
        // Permanent move speed boost given to the player at all stages of the game
        internal static readonly float DefaultMoveSpeedBoost = 0.5f;
        
        // The multiplier for the player's gravity (downwards acceleration) while they are holding the Down button (S by default).
        internal static readonly float HoldingDownGravityMultiplier = 2f;

        // Altered jump speed boost provided by Shiny Red Balloon via IL edit
        // This is a const because it replaces a hardcoded value in vanilla
        internal const float BalloonJumpSpeedBoost = 0.75f;

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
        internal static readonly int BrainDodgeCooldown = 7200;
        internal static readonly int MirrorDodgeCooldown = 5400;
        internal static readonly int DaedalusReflectCooldown = 5400;
        internal static readonly int EvolutionReflectCooldown = 7200;

        // Internal vanilla item damage variables
        internal static readonly float LeatherWhipTagDamageMultiplier = 1.08f;
        internal static readonly float SnapthornTagDamageMultiplier = 1.04f;
        internal static readonly float SpinalTapTagDamageMultiplier = 1.08f;
        internal static readonly float FirecrackerExplosionDamageMultiplier = 2.25f; // Relative to the whip, not the summon.
        internal static readonly float DurendalTagDamageMultiplier = 1.09f;
        internal static readonly float MorningStarTagDamageMultiplier = 1.11f;
        internal static readonly float KaleidoscopeTagDamageMultiplier = 1.12f;

        // Rogue stealth
        // If stealth is too weak, increase this number. If stealth is too strong, decrease this number.
        // This value is intentionally not readonly.
        public static double UniversalStealthStrikeDamageFactor = 0.5;
        internal static readonly float BaseStealthGenTime = 9f; // 9 seconds
        internal static readonly float MovingStealthGenRatio = 0.333333f;

        // Rippers
        internal static readonly float FullAdrenalineDR = 0.5f; // 50%
        internal static readonly float AdrenalineDRPerBooster = 0.05f; // +5% per booster

        // Summon damage bonuses counting less towards "scales with your best class"
        internal static readonly float SummonAllClassScalingFactor = 0.75f;

        // Minimum and maximum allowed attack speed ratios when using Calamity Global Item Tweaks
        internal static readonly float MinimumAllowedAttackSpeed = 0.25f;
        internal static readonly float MaximumAllowedAttackSpeed = 10f;

        // TODO -- Add all balance related constants here that don't belong in other files.
        // These constants were just the ones sitting in CalamityPlayer.
        // Review all constants and static readonlys in the entire mod to find things to add.
    }
}
