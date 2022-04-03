using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class AstralUndergroundBGStyle : ModUgBgStyle
    {
        public override bool ChooseBgStyle() => Main.LocalPlayer.InAstral();

        public override void FillTextureArray(int[] textureSlots)
        {
            for (int i = 0; i <= 3; i++)
            {
                textureSlots[i] = Mod.GetBackgroundSlot("Backgrounds/AstralUG" + i.ToString());
            }
        }
    }
}
