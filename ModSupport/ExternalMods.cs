using Terraria.ModLoader;

namespace CalamityMod
{
    public sealed class ExternalMods : ModSystem
    {
        // This is Calamity's official music mod, CalamityModMusic. It is now a hard dependency.
        internal static Mod musicMod = null;
        internal static bool MusicAvailable => musicMod is not null;

        // This is Vanilla Calamity Mod Music, internally named UnCalamityModMusic.
        // VCMM is an official music add-on. Unlike the main music mod, it is not a dependency.
        internal static Mod vcmm = null;
        internal static bool VCMMAvailable => vcmm is not null;

        // Please keep this in alphabetical order so it's easy to read
        internal static Mod ancientsAwakened = null;
        internal static Mod biomeLava = null;
        internal static Mod bossChecklist = null;
        internal static Mod coloredDamageTypes = null;
        internal static Mod crouchMod = null;
        internal static Mod dialogueTweak = null;
        internal static Mod fargos = null;
        internal static Mod luminance = null;
        internal static Mod magicStorage = null;
        internal static Mod overhaul = null;
        internal static Mod redemption = null;
        internal static Mod remnants = null;
        internal static Mod soa = null;
        internal static Mod speedrunDisplay = null;
        internal static Mod subworldLibrary = null;
        internal static Mod summonersAssociation = null;
        internal static Mod thorium = null;
        internal static Mod varia = null;
        internal static Mod wikithis = null;

        public override void Load()
        {
            musicMod = null;
            ModLoader.TryGetMod("CalamityModMusic", out musicMod);
            vcmm = null;
            ModLoader.TryGetMod("UnCalamityModMusic", out vcmm);

            ancientsAwakened = null;
            ModLoader.TryGetMod("AAMod", out ancientsAwakened);
            biomeLava = null;
            ModLoader.TryGetMod("BiomeLava", out biomeLava);
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
            remnants = null;
            ModLoader.TryGetMod("Remnants", out remnants);
            soa = null;
            ModLoader.TryGetMod("SacredTools", out soa);
            speedrunDisplay = null;
            ModLoader.TryGetMod("SpeedrunDisplay", out speedrunDisplay);
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
        }

        public override void Unload()
        {
            musicMod = null;
            vcmm = null;

            ancientsAwakened = null;
            biomeLava = null;
            bossChecklist = null;
            coloredDamageTypes = null;
            crouchMod = null;
            dialogueTweak = null;
            fargos = null;
            luminance = null;
            magicStorage = null;
            overhaul = null;
            redemption = null;
            remnants = null;
            soa = null;
            speedrunDisplay = null;
            subworldLibrary = null;
            summonersAssociation = null;
            thorium = null;
            varia = null;
            wikithis = null;
        }
    }
}
