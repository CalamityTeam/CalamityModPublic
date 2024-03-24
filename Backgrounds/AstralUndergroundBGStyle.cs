using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class AstralUndergroundBGStyle : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            for (int i = 0; i <= 4; i++)
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot(i == 4 ? "CalamityMod/Backgrounds/AstralUG" + i.ToString() + "_" + Main.hellBackStyle : "CalamityMod/Backgrounds/AstralUG" + i.ToString());
        }
    }
}
