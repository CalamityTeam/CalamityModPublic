using CalamityMod.Items.DraedonMisc;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonLogHellGUI : DraedonsLogGUI
    {
        public override int TotalPages => 3;
        public override string GetTextByPage()
        {
            return CalamityUtils.GetTextValueFromModItem<DraedonsLogHell>("ContentPage" + (Page + 1));
        }
        public override Texture2D GetTextureByPage()
        {
            switch (Page)
            {
                case 0:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogCragsBiome").Value;
                case 1:
                    return null;
                default:
                    return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogWallOfFlesh").Value;
            }
        }
    }
}
