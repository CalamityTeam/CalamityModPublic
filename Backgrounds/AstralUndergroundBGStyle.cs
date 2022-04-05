using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class AstralUndergroundBGStyle : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            for (int i = 0; i <= 3; i++)
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot("Backgrounds/AstralUG" + i.ToString());
        }
    }
}
