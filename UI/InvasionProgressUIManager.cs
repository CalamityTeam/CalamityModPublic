using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalamityMod.UI
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class InvasionProgressUIManager
    {
        private static readonly List<InvasionProgressUI> gUIs = new List<InvasionProgressUI>();
        public static int TotalGUIsActive => gUIs.Count(gui => gui.IsActive);
        public static bool AnyGUIsActive => TotalGUIsActive > 0;
        public static InvasionProgressUI GetActiveGUI => gUIs.FirstOrDefault(gui => gui.IsActive);
        public static void UpdateAndDraw(SpriteBatch spriteBatch)
        {
            if (AnyGUIsActive)
            {
                if (GetActiveGUI is null)
                    return;
                GetActiveGUI.Draw(spriteBatch);
            }
        }

        public static void LoadGUIs()
        {
            // Look through every type in the mod, and check if it's derived from InvasionProgressUI. If it is, create a copy and save it in the static list.
            foreach (Type type in typeof(CalamityMod).Assembly.GetTypes())
            {
                // Don't load abstract classes since they cannot have instances.
                if (type.IsAbstract)
                    continue;
                if (type.IsSubclassOf(typeof(InvasionProgressUI)))
                    gUIs.Add(Activator.CreateInstance(type) as InvasionProgressUI);
            }
        }
        public static void UnloadGUIs() => gUIs.Clear();
    }
}
