namespace CalamityMod.Dusts
{
    public enum CalamityDusts : int
    {
        // Note: Astral dusts are not included because:
        // 1. Their internal value is not constant, meaning it cannot be used in this enum, and
        // 2. It's a custom dust.

        SulfurousSeaAcid = 75,
        Brimstone = 235,
        Ectoplasm = 180,
        Polterplasm = 60,
        Plague = 46,
        ProfanedFire = 244,
        Nightwither = 185,
        PurpleCosmilite = 173,
        // This is what God Slayer Slugs use, at least. I'm not sure about other Cosmilite stuff, didn't bother to check.
        BlueCosmilite = 180,
        MiracleBlight = 163,
    }
}
