using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts {
public class SWSparkle : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.velocity *= 0.1f;
        dust.noGravity = false;
        dust.noLight = true;
        dust.scale *= 0.25f;
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.rotation += dust.velocity.X * 0.05f;
        dust.scale *= 0.99f;
        float light = 0.35f * dust.scale;
        Lighting.AddLight(dust.position, 0.15f, 0.01f, 0.15f);
        if(dust.scale < 0.1f)
        {
            dust.active = false;
        }
        return false;
    }
}}