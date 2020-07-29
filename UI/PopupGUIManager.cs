using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace CalamityMod.UI
{
    public static class PopupGUIManager
    {
        private static readonly List<PopupGUI> gUIs = new List<PopupGUI>();
        public static DraedonLogSunkenSeaGUI DraedonLogSunkenSeaGUI;
        public static bool GUIActive(PopupGUI gui) => gui.Active || gui.FadeTime > 0;
        public static bool AnyGUIsActive => gUIs.Any(GUIActive);
        public static PopupGUI GetActiveGUI => gUIs.FirstOrDefault(GUIActive);
        public static void SuspendAll()
        {
            DraedonLogSunkenSeaGUI.Active = false;
            DraedonLogSunkenSeaGUI.FadeTime = 0;
        }
        public static void UpdateAndDraw(SpriteBatch spriteBatch)
        {
            if (Main.ingameOptionsWindow || Main.inFancyUI)
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
        public static void LoadGUIs()
        {
            DraedonLogSunkenSeaGUI = new DraedonLogSunkenSeaGUI();
            gUIs.Add(DraedonLogSunkenSeaGUI);
        }
        public static void UnloadGUIs()
        {
            DraedonLogSunkenSeaGUI = null;
            gUIs.Clear();
        }
    }
}
