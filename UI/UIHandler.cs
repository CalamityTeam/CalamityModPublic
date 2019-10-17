using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityMod.UI
{
    public class UIHandler
    {
        public static UserInterface userBar;
        public static UserInterface userBar2;
        public static UIBar uiBar;
        public static UIBar2 uiBar2;

        //call this in mod.Load()
        public static void OnLoad(Mod mod)
        {
            int offset = 0; // How many pixels inwards to stick the bar

            Texture2D borderTex = mod.GetTexture("ExtraTextures/UI/BarStressBorder"), barTex = mod.GetTexture("ExtraTextures/UI/BarStress"); //replace 'null' with your textures.
            uiBar = new UIBar(borderTex, barTex, offset);
            userBar = new UserInterface();
            userBar.SetState(uiBar);

            Texture2D borderTex2 = mod.GetTexture("ExtraTextures/UI/BarAdrenalineBorder"), barTex2 = mod.GetTexture("ExtraTextures/UI/BarAdrenaline"); //replace 'null' with your textures.
            uiBar2 = new UIBar2(borderTex2, barTex2, offset);
            userBar2 = new UserInterface();
            userBar2.SetState(uiBar2);
        }

        //call this in mod.ModifyInterfaceLayers()
        public static void ModifyInterfaceLayers(Mod mod, List<GameInterfaceLayer> layers)
        {
            AddInterfaceLayer(mod, layers, userBar, uiBar, "Calamity: Stress Bar", "Vanilla: Mouse Text", true);
            AddInterfaceLayer(mod, layers, userBar2, uiBar2, "Calamity: Adrenaline Bar", "Vanilla: Mouse Text", true);
        }

        public static void AddInterfaceLayer(Mod mod, List<GameInterfaceLayer> list, UserInterface uInterface, UIElement uElement, string layerName, string parent, bool first)
        {
            GameInterfaceLayer item = new LegacyGameInterfaceLayer(mod.Name + ":" + layerName, delegate
            {
                uInterface.Update(Main._drawInterfaceGameTime);
                uElement.Draw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.UI);

            int insertAt = -1;
            for (int m = 0; m < list.Count; m++)
            {
                GameInterfaceLayer dl = list[m];
                if (dl.Name.Contains(parent))
                { insertAt = m; break; }
            }
            if (insertAt == -1)
                list.Add(item);
            else
                list.Insert(first ? insertAt : insertAt + 1, item);
        }
    }
}
