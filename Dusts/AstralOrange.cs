using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts
{
    public class AstralOrange : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
        }

        public override bool Update(Dust dust)
        {
            //update position
            dust.position += dust.velocity;

            //shrink scale
            dust.scale = MathHelper.Clamp(dust.scale - 0.02f, 0f, float.MaxValue);

            if (!dust.noLight)
            {
                Lighting.AddLight(dust.position, 0.4f * dust.scale, 0.24f * dust.scale, 0.04f * dust.scale);
            }

            if (dust.customData != null)
            {
                if (dust.customData is bool && (bool)dust.customData) //slowdown or nah
                {
                    dust.velocity *= 0.94f;
                }
                else if (dust.customData is float)
                {
                    dust.scale -= (float)dust.customData;
                }
            }

            if (dust.scale < 0.1f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}
