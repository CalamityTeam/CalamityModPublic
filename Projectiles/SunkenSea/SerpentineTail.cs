using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.SunkenSea
{
    public class SerpentineTail : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Serpentine");
		}

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
			projectile.tileCollide = false;
			projectile.alpha = 255;
            projectile.penetrate = -1;
			projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
		{
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0.25f) / 255f);
			int num1051 = 10;
			Vector2 value68 = Vector2.Zero;
			float num1064 = 0f;
			float scaleFactor17 = 0f;
			float scaleFactor18 = 1f;
			if (projectile.ai[1] == 1f)
			{
				projectile.ai[1] = 0f;
				projectile.netUpdate = true;
			}
			int chase = (int)projectile.ai[0];
			if (chase >= 0 && Main.projectile[chase].active)
			{
				value68 = Main.projectile[chase].Center;
				Vector2 arg_2DE6A_0 = Main.projectile[chase].velocity;
				num1064 = Main.projectile[chase].rotation;
				scaleFactor18 = MathHelper.Clamp(Main.projectile[chase].scale, 0f, 50f);
				scaleFactor17 = 16f;
				Main.projectile[chase].localAI[0] = projectile.localAI[0] + 1f;
			}
			else
			{
				for (int k = 0; k < 8; k++)
				{
					int num114 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 68, 0f, 0f, 100, default(Color), 1.25f);
					Dust dust = Main.dust[num114];
					dust.velocity *= 0.3f;
					Main.dust[num114].position.X = projectile.position.X + (float)(projectile.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
					Main.dust[num114].position.Y = projectile.position.Y + (float)(projectile.height / 2) + (float)Main.rand.Next(-4, 5);
					Main.dust[num114].noGravity = true;
				}
				projectile.active = false;
				projectile.Kill();
				return;
			}
			if (projectile.alpha > 0)
			{
				projectile.alpha -= 40;
			}
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
			projectile.velocity = Vector2.Zero;
			Vector2 vector134 = value68 - projectile.Center;
			if (num1064 != projectile.rotation)
			{
				float num1068 = MathHelper.WrapAngle(num1064 - projectile.rotation);
				vector134 = vector134.RotatedBy((double)(num1068 * 0.1f), default(Vector2));
			}
			projectile.rotation = vector134.ToRotation() + 1.57079637f;
			projectile.position = projectile.Center;
			projectile.scale = scaleFactor18;
			projectile.width = (projectile.height = (int)((float)num1051 * projectile.scale));
			projectile.Center = projectile.position;
			if (vector134 != Vector2.Zero)
			{
				projectile.Center = value68 - Vector2.Normalize(vector134) * scaleFactor17 * scaleFactor18;
			}
			projectile.spriteDirection = ((vector134.X > 0f) ? 1 : -1);
			return;
		}
	}
}
