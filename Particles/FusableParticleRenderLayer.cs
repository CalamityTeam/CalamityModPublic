namespace CalamityMod.Particles
{
    // The order of entries in this enumeration are in line with draw order 
    // e.g. the first entry is the first drawn thing, the last entry is the last drawn thing.
    public enum FusableParticleRenderLayer
    {
        OverNPCsBeforeProjectiles,
        OverPlayers,
        OverWater
    }
}
