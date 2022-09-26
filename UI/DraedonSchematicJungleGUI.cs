using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class DraedonSchematicJungleGUI : DraedonsLogGUI
    {
        public override int TotalPages => 1;
        public override string GetTextByPage()
        {
            return "As rank progresses, so often does the lethality of equipment. In the hands of competent soldiers, the weapons have the ability to make change.\n" + 
                "However, competent soldiers take no action but orders from above.\n" +
                "Addendum: If you read this, you have come far. Do not disappoint.\n" + 
                "Go now to Hell, for the next component stored in what were once my forges.";
        }
        public override Texture2D GetTextureByPage()
        {
            return ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/UI/DraedonsLogJungleBiome").Value;
        }
    }
}
