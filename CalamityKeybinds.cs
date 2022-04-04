using Terraria.ModLoader;

namespace CalamityMod

{
    public class CalamityKeybinds : ModSystem
    {
        public static ModKeybind NormalityRelocatorHotKey { get; private set; }
        public static ModKeybind AegisHotKey { get; private set; }
        public static ModKeybind SetBonusHotKey { get; private set; }
        public static ModKeybind RageHotKey { get; private set; }
        public static ModKeybind AdrenalineHotKey { get; private set; }
        public static ModKeybind AstralTeleportHotKey { get; private set; }
        public static ModKeybind AstralArcanumUIHotkey { get; private set; }
        public static ModKeybind MomentumCapacitatorHotkey { get; private set; }
        public static ModKeybind SandCloakHotkey { get; private set; }
        public static ModKeybind SpectralVeilHotKey { get; private set; }
        public static ModKeybind PlaguePackHotKey { get; private set; }
        public static ModKeybind AngelicAllianceHotKey { get; private set; }
        public static ModKeybind GodSlayerDashHotKey { get; private set; }
        public static ModKeybind ExoChairSpeedupHotkey { get; private set; }
        public static ModKeybind ExoChairSlowdownHotkey { get; private set; }

        public override void Load()
        {
            // Register keybinds
            NormalityRelocatorHotKey = KeybindLoader.RegisterKeybind(Mod, "Normality Relocator", "Z");
            RageHotKey = KeybindLoader.RegisterKeybind(Mod, "Rage Mode", "V");
            AdrenalineHotKey = KeybindLoader.RegisterKeybind(Mod, "Adrenaline Mode", "B");
            AegisHotKey = KeybindLoader.RegisterKeybind(Mod, "Elysian Guard", "N");
            SetBonusHotKey = KeybindLoader.RegisterKeybind(Mod, "Armor Set Bonus", "Y");
            AstralTeleportHotKey = KeybindLoader.RegisterKeybind(Mod, "Astral Teleport", "P");
            AstralArcanumUIHotkey = KeybindLoader.RegisterKeybind(Mod, "Astral Arcanum UI Toggle", "O");
            MomentumCapacitatorHotkey = KeybindLoader.RegisterKeybind(Mod, "Momentum Capacitor Effect", "U");
            SandCloakHotkey = KeybindLoader.RegisterKeybind(Mod, "Sand Cloak Effect", "C");
            SpectralVeilHotKey = KeybindLoader.RegisterKeybind(Mod, "Spectral Veil Teleport", "Z");
            PlaguePackHotKey = KeybindLoader.RegisterKeybind(Mod, "Booster Dash", "Q");
            AngelicAllianceHotKey = KeybindLoader.RegisterKeybind(Mod, "Angelic Alliance Blessing", "G");
            GodSlayerDashHotKey = KeybindLoader.RegisterKeybind(Mod, "God Slayer Dash", "H");
            ExoChairSpeedupHotkey = KeybindLoader.RegisterKeybind(Mod, "Exo Chair Speed Up", "LeftShift");
            ExoChairSlowdownHotkey = KeybindLoader.RegisterKeybind(Mod, "Exo Chair Slow Down", "RightShift");
        }

        public override void Unload()
        {
            // Not required anymore, but nice nontheless

            NormalityRelocatorHotKey = null;
            RageHotKey = null;
            AdrenalineHotKey = null;
            AegisHotKey = null;
            SetBonusHotKey = null;
            AstralTeleportHotKey = null;
            AstralArcanumUIHotkey = null;
            MomentumCapacitatorHotkey = null;
            SandCloakHotkey = null;
            SpectralVeilHotKey = null;
            PlaguePackHotKey = null;
            AngelicAllianceHotKey = null;
            GodSlayerDashHotKey = null;
        }
    }
}
