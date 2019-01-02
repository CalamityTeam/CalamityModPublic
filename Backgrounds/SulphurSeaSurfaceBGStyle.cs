using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Backgrounds
{
    public class SulphurSeaSurfaceBGStyle : ModSurfaceBgStyle
    {
        public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) 
        {
            scale *= 0.75f;
            return mod.GetBackgroundSlot("Backgrounds/SulphurSeaSurfaceClose");
        }

        public override bool ChooseBgStyle()
        {
            return !Main.gameMenu && Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur;
        }

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
