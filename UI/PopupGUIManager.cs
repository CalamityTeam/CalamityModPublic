using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace CalamityMod.UI
{
    public static class PopupGUIManager
    {
        private static readonly List<PopupGUI> gUIs = new List<PopupGUI>();
        public static DraedonsLogUI DraedonLogGUI;
        public static bool GUIActive(PopupGUI gui) => gui.Active || gui.FadeTime > 0;
        public static bool AnyGUIsActive => gUIs.Any(GUIActive);
        public static PopupGUI GetActiveGUI => gUIs.FirstOrDefault(GUIActive);
        public static void SuspendAll()
        {
            DraedonLogGUI.Active = false;
        }
        public static void UpdateAndDraw(SpriteBatch spriteBatch)
        {
            if (AnyGUIsActive)
            {
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
            DraedonLogGUI = new DraedonsLogUI();
            gUIs.Add(DraedonLogGUI);
        }
        public static void UnloadGUIs()
        {
            DraedonLogGUI = null;
            gUIs.Clear();
        }
    }
}
