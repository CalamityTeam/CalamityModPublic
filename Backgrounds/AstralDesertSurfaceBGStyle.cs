﻿using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Backgrounds
{
    public class AstralDesertSurfaceBGStyle : ModSurfaceBgStyle
    {
        public override int ChooseFarTexture() => mod.GetBackgroundSlot("Backgrounds/AstralDesertSurfaceFar");

        public override int ChooseMiddleTexture() => mod.GetBackgroundSlot("Backgrounds/AstralDesertSurfaceMiddle");

        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
        {
            b -= 75f;
            return mod.GetBackgroundSlot("Backgrounds/AstralDesertSurfaceClose");
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
