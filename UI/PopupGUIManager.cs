using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace CalamityMod.UI
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class PopupGUIManager
    {
        private static readonly List<PopupGUI> gUIs = new List<PopupGUI>();
        public static bool GUIActive(PopupGUI gui) => gui.Active || gui.FadeTime > 0;
        public static bool AnyGUIsActive => gUIs.Any(GUIActive);
        public static PopupGUI GetActiveGUI => gUIs.FirstOrDefault(GUIActive);
        public static void SuspendAll()
        {
            for (int i = 0; i < gUIs.Count; i++)
            {
                gUIs[i].Active = false;
                gUIs[i].FadeTime = 0;
            }
        }
        public static void UpdateAndDraw(SpriteBatch spriteBatch)
        {
            if (Main.ingameOptionsWindow || Main.inFancyUI || Main.InGameUI.IsVisible)
            {
                SuspendAll();
                return;
            }
            if (AnyGUIsActive)
            {
                Main.playerInventory = false;
                if (Main.LocalPlayer.sign > 0 || Main.LocalPlayer.talkNPC > 0)
                    Main.CloseNPCChatOrSign();
                GetActiveGUI.Update();
                if (GetActiveGUI == null)
                    return;
                if (GetActiveGUI.FadeTime == 1 && !GetActiveGUI.Active)
                {
                    spriteBatch.End();
                    spriteBatch.Begin();
                    return;
                }
                GetActiveGUI.Draw(spriteBatch);
            }
        }

        // Pretty self-explanatory. Made so that you don't have to play around with LINQ and Reflection manually.
        public static void FlipActivityOfGUIWithType(Type type)
        {
            // End early if the designated type does not exist in the list.
            if (!gUIs.Any(gui => gui.GetType() == type))
                return;
            gUIs.First(gui => gui.GetType() == type).Active = !gUIs.First(gui => gui.GetType() == type).Active;
        }

        public static void LoadGUIs()
        {
            // Look through every type in the mod, and check if it's derived from PopupGUI. If it is, create a copy and save it in the static list.
            foreach (Type type in typeof(CalamityMod).Assembly.GetTypes())
            {
                // Don't load abstract classes since they cannot have instances.
                if (type.IsAbstract)
                    continue;
                if (type.IsSubclassOf(typeof(PopupGUI)))
                    gUIs.Add(Activator.CreateInstance(type) as PopupGUI);
            }
        }
        public static void UnloadGUIs() => gUIs.Clear();
    }
}
