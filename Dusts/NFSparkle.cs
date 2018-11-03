using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts {
public class NFSparkle : ModDust
{
    public override void OnSpawn(Dust dust)
    {
    	dust.velocity *= 0.25f;
        dust.noGravity = true;
        dust.alpha = 50;
        dust.noLight = true;
        dust.scale = 0.225f;
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.rotation += dust.velocity.X * 0.5f;
		dust.scale *= 0.99f;
        float light = 0.35f * dust.scale;
        Lighting.AddLight(dust.position, 0.01f, 0.2f, 0.2f);
        if (dust.scale < 0.175f)
        {
            dust.active = false;
        }
        return false;
    }
}}