using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class AstralDesertSurfaceBGStyle : ModSurfaceBgStyle
    {
        public override int ChooseFarTexture() => Mod.GetBackgroundSlot("Backgrounds/AstralDesertSurfaceFar");

        public override int ChooseMiddleTexture() => Mod.GetBackgroundSlot("Backgrounds/AstralDesertSurfaceMiddle");

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            b -= 75f;
            return Mod.GetBackgroundSlot("Backgrounds/AstralDesertSurfaceClose");
        }

        public override bool ChooseBgStyle() => !Main.gameMenu && Main.LocalPlayer.InAstral() && Main.LocalPlayer.ZoneDesert;

        public override void ModifyFarFades(float[] fades, float transitionSpeed)
        {
            //This just fades in the background and fades out other backgrounds.
            for (int i = 0; i < fades.Length; i++)
            {
                if (i == Slot)
                {
                    fades[i] += transitionSpeed;
                    if (fades[i] > 1f)
                    {
                        fades[i] = 1f;
                    }
                }
                else
                {
                    fades[i] -= transitionSpeed;
                    if (fades[i] < 0f)
                    {
                        fades[i] = 0f;
                    }
                }
            }
        }
    }
}
