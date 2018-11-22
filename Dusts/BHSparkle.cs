using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts {
public class BHSparkle : ModDust
{
    public override void OnSpawn(Dust dust)
    {
    	dust.velocity *= 0.94f;
        dust.noGravity = true;
        dust.noLight = true;
        dust.scale = 1.5f;
        dust.alpha = 100;
    }

    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.rotation += dust.velocity.X * 0.05f;
		dust.scale *= 0.99f;
        float light = 0.35f * dust.scale;
        if (dust.scale >= 1.5)
		{
			dust.scale -= 0.01f;
		}
		else
		{
			dust.scale -= 0.05f;
		}
		if (dust.scale <= 0.5)
		{
			dust.scale -= 0.05f;
		}
		if (dust.scale <= 0.25)
		{
			dust.scale -= 0.05f;
		}
        if (dust.scale < 0.3f)
        {
            dust.active = false;
        }
        float num94 = dust.scale;
		if (dust.noLight)
		{
			num94 *= 0.1f;
			dust.scale -= 0.06f;
			if (dust.scale < 1f)
			{
				dust.scale -= 0.06f;
			}
			if (Main.player[Main.myPlayer].wet)
			{
				dust.position += Main.player[Main.myPlayer].velocity * 0.5f;
			}
			else
			{
				dust.position += Main.player[Main.myPlayer].velocity;
			}
		}
		if (num94 > 1f)
		{
			num94 = 1f;
		}
		Lighting.AddLight(dust.position, 0.25f, 0.15f, 0.01f);
    	return false;
    	}
	}
}