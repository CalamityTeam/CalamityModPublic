using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI.DraedonLogs
{
    public class DraedonSchematicIceGUI : DraedonsLogGUI
    {
        public override int TotalPages => 1;
        public override string GetTextByPage()
        {
            return "I have since made progress to even greater weapons than these, but they remain creations to be proud of.\n" + 
                "No progress can be made without a desire that comes from dissatisfaction.\n" +
                "Addendum: The time has come. You are ready.";
        }
        public override Texture2D GetTextureByPage()
        {
            return ModContent.Request<Texture2D>("CalamityMod/UI/DraedonLogs/DraedonsLogIceBiome").Value;
        }
    }
}
