using Terraria.ModLoader;

namespace CalamityMod
{
    public class CalamityKeybinds : ModSystem
    {
        public static ModKeybind NormalityRelocatorHotKey { get; private set; }
        public static ModKeybind BlazingCoreHotKey { get; private set; }
        public static ModKeybind SetBonusHotKey { get; private set; }
        public static ModKeybind RageHotKey { get; private set; }
        public static ModKeybind AdrenalineHotKey { get; private set; }
        public static ModKeybind AstralTeleportHotKey { get; private set; }
        public static ModKeybind AstralArcanumUIHotkey { get; private set; }
        public static ModKeybind SandCloakHotkey { get; private set; }
        public static ModKeybind SpectralVeilHotKey { get; private set; }
        public static ModKeybind PlaguePackHotKey { get; private set; }
        public static ModKeybind AngelicAllianceHotKey { get; private set; }
        public static ModKeybind GodSlayerDashHotKey { get; private set; }
        public static ModKeybind ExoChairSlowdownHotkey { get; private set; }
        public static ModKeybind DashHotkey { get; private set; }
        public static ModKeybind GravistarSabatonHotkey { get; private set; }

        public override void Load()
        {
            // Register keybinds
            NormalityRelocatorHotKey = KeybindLoader.RegisterKeybind(Mod, "NormalityRelocator", "Z");
            RageHotKey = KeybindLoader.RegisterKeybind(Mod, "RageMode", "V");
            AdrenalineHotKey = KeybindLoader.RegisterKeybind(Mod, "AdrenalineMode", "B");
            BlazingCoreHotKey = KeybindLoader.RegisterKeybind(Mod, "BlazingCoreParry", "N");
            SetBonusHotKey = KeybindLoader.RegisterKeybind(Mod, "ArmorSetBonus", "Y");
            AstralTeleportHotKey = KeybindLoader.RegisterKeybind(Mod, "AstralTeleport", "P");
            AstralArcanumUIHotkey = KeybindLoader.RegisterKeybind(Mod, "AstralArcanumUIToggle", "O");
            SandCloakHotkey = KeybindLoader.RegisterKeybind(Mod, "SandCloakEffect", "C");
            SpectralVeilHotKey = KeybindLoader.RegisterKeybind(Mod, "SpectralVeilTeleport", "Z");
            PlaguePackHotKey = KeybindLoader.RegisterKeybind(Mod, "BoosterDash", "Q");
            AngelicAllianceHotKey = KeybindLoader.RegisterKeybind(Mod, "AngelicAllianceBlessing", "G");
            GodSlayerDashHotKey = KeybindLoader.RegisterKeybind(Mod, "GodSlayerDash", "H");
            ExoChairSlowdownHotkey = KeybindLoader.RegisterKeybind(Mod, "ExoChairSlowDown", "RightShift");
            DashHotkey = KeybindLoader.RegisterKeybind(Mod, "DashDoubleTapOverride", "F");
            GravistarSabatonHotkey = KeybindLoader.RegisterKeybind(Mod, "GravistarSabatonHotkey", "X");
        }

        public override void Unload()
        {
            NormalityRelocatorHotKey = null;
            RageHotKey = null;
            AdrenalineHotKey = null;
            BlazingCoreHotKey = null;
            SetBonusHotKey = null;
            AstralTeleportHotKey = null;
            AstralArcanumUIHotkey = null;
            SandCloakHotkey = null;
            SpectralVeilHotKey = null;
            PlaguePackHotKey = null;
            AngelicAllianceHotKey = null;
            GodSlayerDashHotKey = null;
            ExoChairSlowdownHotkey = null;
            DashHotkey = null;
            GravistarSabatonHotkey = null;
        }
    }
}
