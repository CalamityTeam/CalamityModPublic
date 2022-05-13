namespace CalamityMod.Balancing
{
    public static class BalancingConstants
    {
        // The multiplier for the player's gravity (downwards acceleration) while they are holding the Down button (S by default).
        internal static readonly float HoldingDownGravityMultiplier = 2f;

        // Altered jump speed boost provided by Shiny Red Balloon via IL edit
        // This is a const because it replaces a hardcoded value in vanilla
        internal const float BalloonJumpSpeedBoost = 0.75f;

        // Vanilla shield slam stats
        // These are consts because they replace hardcoded values in vanilla
        internal const int ShieldOfCthulhuBonkNoCollideFrames = 6;
        internal const int SolarFlareIFrames = 12;
        internal const float SolarFlareBaseDamage = 400f;

        // Dodge cooldowns (in frames)
        // TODO -- Some of these could be moved to the respective item files
        internal static readonly int BeltDodgeCooldown = 5400;
        internal static readonly int MirrorDodgeCooldown = 5400;
        internal static readonly int DaedalusReflectCooldown = 5400;
        internal static readonly int EvolutionReflectCooldown = 7200;

        // TODO -- Add all balance related constants here that don't belong in other files.
        // These constants were just the ones sitting in CalamityPlayer.
        // Review all constants and static readonlys in the entire mod to find things to add.
    }
}
