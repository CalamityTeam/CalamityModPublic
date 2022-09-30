using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonSchematicSunkenSeaGUI : DraedonsLogGUI
    {
        public override int TotalPages => 1;
        public override string GetTextByPage()
        {
            return "The weaponry I supply to the workers of the laboratories is weak. Hardly suited for battle.\n" +
            "However, they suffice for self defense against any lab mechanisms or creations which may have gone rogue.\n" +
            "Addendum: For those who think themselves powerful, search the upper bounds of this planet’s atmosphere for a structure similar to that of the Sunken Seas.\n" +
            "I will know by the end if you are worthy of battling my creations.\n";
        }
        public override Texture2D GetTextureByPage()
        {
            return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogSunkenSeaBiome").Value;
        }
    }
}
