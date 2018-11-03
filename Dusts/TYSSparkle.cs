using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts {
public class TYSSparkle : ModDust
{
    public override void OnSpawn(Dust dust)
    {
    	dust.velocity *= 0.5f;
    	dust.alpha = 55;
        dust.noGravity = false;
        dust.noLight = false;
        dust.scale = 0.75f;
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity * 1.5f;
        dust.rotation += dust.velocity.X * 1.1f;
		dust.scale *= 0.99f;
        float light = 0.35f * dust.scale;
        Lighting.AddLight(dust.position, 0.5f, 0.05f, 0.05f);
        if (dust.scale < 0.25f)
        {
            dust.active = false;
        }
        return false;
    }
}}