﻿
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Dusts
{
    public class AstralBlue : ModDust
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
            dust.scale -= 0.02f;

            if (!dust.noLight)
            {
                Lighting.AddLight(dust.position, 0.1f * dust.scale, 0.3f * dust.scale, 0.4f * dust.scale);
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
