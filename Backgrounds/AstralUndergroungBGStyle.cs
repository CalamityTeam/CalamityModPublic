using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class ExampleUgBgStyle : ModUgBgStyle
    {
        public override bool ChooseBgStyle()
        {
            return Main.LocalPlayer.GetModPlayer<CalamityPlayer>().ZoneAstral;
        }

        public override void FillTextureArray(int[] textureSlots)
        {
            textureSlots[0] = mod.GetBackgroundSlot("Backgrounds/AstralUG0");
            textureSlots[1] = mod.GetBackgroundSlot("Backgrounds/AstralUG1");
            textureSlots[2] = mod.GetBackgroundSlot("Backgrounds/AstralUG2");
            textureSlots[3] = mod.GetBackgroundSlot("Backgrounds/AstralUG3");
        }
    }
}