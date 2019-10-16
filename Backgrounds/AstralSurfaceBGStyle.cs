using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Backgrounds
{
    public class AstralSurfaceBGStyle : ModSurfaceBgStyle
    {
        public override int ChooseFarTexture() => mod.GetBackgroundSlot("Backgrounds/AstralSurfaceFar");

        public override int ChooseMiddleTexture() => mod.GetBackgroundSlot("Backgrounds/AstralSurfaceMiddle");

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            scale *= 0.75f;
            return mod.GetBackgroundSlot("Backgrounds/AstralSurfaceClose");
        }

        public override bool ChooseBgStyle() => !Main.gameMenu && Main.LocalPlayer.InAstral() && !Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneSnow;

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
