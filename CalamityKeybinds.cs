using Terraria.ModLoader;

namespace CalamityMod
{
    public class CalamityKeybinds : ModSystem
    {
        public static ModKeybind AccessoryParryHotKey { get; private set; }
        public static ModKeybind AdrenalineHotKey { get; private set; }
        public static ModKeybind AngelicAllianceHotKey { get; private set; }
        public static ModKeybind ArmorSetBonusHotKey { get; private set; }
        public static ModKeybind AscendantInsigniaHotKey { get; private set; }
        public static ModKeybind BoosterDashHotKey { get; private set; }
        public static ModKeybind DashHotkey { get; private set; }
        public static ModKeybind ExoChairSlowdownHotkey { get; private set; }
        public static ModKeybind GodSlayerDashHotKey { get; private set; }
        public static ModKeybind GravistarSabatonHotkey { get; private set; }
        public static ModKeybind NormalityRelocatorHotKey { get; private set; }
        public static ModKeybind RageHotKey { get; private set; }
        public static ModKeybind SandCloakHotkey { get; private set; }
        public static ModKeybind SpectralVeilHotKey { get; private set; }

        public override void Load()
        {
            // Register keybinds            
            AccessoryParryHotKey = KeybindLoader.RegisterKeybind(Mod, "ActivateAccessoryParry", "N");
            AdrenalineHotKey = KeybindLoader.RegisterKeybind(Mod, "AdrenalineMode", "B");
            AngelicAllianceHotKey = KeybindLoader.RegisterKeybind(Mod, "AngelicAllianceBlessing", "G");
            ArmorSetBonusHotKey = KeybindLoader.RegisterKeybind(Mod, "ArmorSetBonus", "Y");
            AscendantInsigniaHotKey = KeybindLoader.RegisterKeybind(Mod, "AscendantInsigniaHotKey", "K");
            BoosterDashHotKey = KeybindLoader.RegisterKeybind(Mod, "BoosterDash", "Q");
            DashHotkey = KeybindLoader.RegisterKeybind(Mod, "DashDoubleTapOverride", "F");
            ExoChairSlowdownHotkey = KeybindLoader.RegisterKeybind(Mod, "ExoChairSlowDown", "RightShift");
            GodSlayerDashHotKey = KeybindLoader.RegisterKeybind(Mod, "GodSlayerDash", "H");
            GravistarSabatonHotkey = KeybindLoader.RegisterKeybind(Mod, "GravistarSabatonHotkey", "X"); 
            NormalityRelocatorHotKey = KeybindLoader.RegisterKeybind(Mod, "NormalityRelocator", "Z");
            RageHotKey = KeybindLoader.RegisterKeybind(Mod, "RageMode", "V");
            SandCloakHotkey = KeybindLoader.RegisterKeybind(Mod, "SandCloakEffect", "C");
            SpectralVeilHotKey = KeybindLoader.RegisterKeybind(Mod, "SpectralVeilTeleport", "Z");           
        }

        public override void Unload()
        {
            AccessoryParryHotKey = null;
            AdrenalineHotKey = null;
            AngelicAllianceHotKey = null;
            ArmorSetBonusHotKey = null;
            AscendantInsigniaHotKey = null;
            BoosterDashHotKey = null;
            DashHotkey = null;
            ExoChairSlowdownHotkey = null;
            GodSlayerDashHotKey = null;
            GravistarSabatonHotkey = null;
            NormalityRelocatorHotKey = null;
            RageHotKey = null;
            SandCloakHotkey = null;
            SpectralVeilHotKey = null;
        }
    }
}
