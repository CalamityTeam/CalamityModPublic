using System;
using System.Collections.Generic;

namespace CalamityMod.UI.DraedonSummoning
{
    public static class DraedonDialogRegistry
    {
        public const string CommunicationLocalizationBase = "Mods.CalamityMod.UI.Communication.";

        public static readonly DraedonDialogEntry WhoAreYou = CreateFromKey("WhoAreYou");

        public static readonly DraedonDialogEntry CalamitasBeforeHerDefeat = CreateFromKey("CalamitasBeforeDefeat", () => !DownedBossSystem.downedCalamitas);

        public static readonly DraedonDialogEntry CalamitasAfterHerDefeat = CreateFromKey("CalamitasAfterDefeat", () => DownedBossSystem.downedCalamitas);

        public static readonly DraedonDialogEntry Plague = CreateFromKey("Plague", () => DownedBossSystem.downedPlaguebringer);

        public static readonly DraedonDialogEntry SulphurousSea = CreateFromKey("SulphuricSea");

        public static readonly DraedonDialogEntry TheTyrant = CreateFromKey("TheTyrant", () => DownedBossSystem.downedCalamitas);

        internal static List<DraedonDialogEntry> DialogOptions = new()
        {
            // The first index is assumed to be the "who are you?" dialog by the UI. All other indices can be freely swapped around, however.
            WhoAreYou,

            CalamitasBeforeHerDefeat,
            CalamitasAfterHerDefeat,
            Plague,
            SulphurousSea,
            TheTyrant
        };

        internal static DraedonDialogEntry CreateFromKey(string key, Func<bool> condition = null) =>
            new($"{CommunicationLocalizationBase}{key}", condition);
    }
}
