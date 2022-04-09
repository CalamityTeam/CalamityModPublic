using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class AstralSnowSurfaceBGStyle : ModSurfaceBackgroundStyle
    {
        public override int ChooseFarTexture() => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralSnowSurfaceFar");

        public override int ChooseMiddleTexture() => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralSnowSurfaceMiddle");

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => BackgroundTextureLoader.GetBackgroundSlot("CalamityMod/Backgrounds/AstralSurfaceClose");

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            // This just fades in the background and fades out other backgrounds.
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                        fades[i] = 1f;
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                        fades[i] = 0f;
                }
            }
        }
    }
}
