using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AsteroidMolten : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Asteroid");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.extraUpdates = 2;
        }

        public override void AI()
        {
        	if (projectile.position.Y > Main.player[projectile.owner].position.Y - 300f)
			{
				projectile.tileCollide = true;
			}
			if ((double)projectile.position.Y < Main.worldSurface * 16.0)
			{
				projectile.tileCollide = true;
			}
			projectile.scale = projectile.ai[1];
			projectile.rotation += projectile.velocity.X * 2f;
			Vector2 position = projectile.Center + Vector2.Normalize(projectile.velocity) * 10f;
			Dust dust20 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f)];
			dust20.position = position;
			dust20.velocity = projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2)) * 0.33f + projectile.velocity / 4f;
			dust20.position += projectile.velocity.RotatedBy(1.5707963705062866, default(Vector2));
			dust20.fadeIn = 0.5f;
			dust20.noGravity = true;
			dust20 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f)];
			dust20.position = position;
			dust20.velocity = projectile.velocity.RotatedBy(-1.5707963705062866, default(Vector2)) * 0.33f + projectile.velocity / 4f;
			dust20.position += projectile.velocity.RotatedBy(-1.5707963705062866, default(Vector2));
			dust20.fadeIn = 0.5f;
			dust20.noGravity = true;
			for (int num189 = 0; num189 < 1; num189++)
			{
				int num190 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 0, new Color(255, Main.DiscoG, 0), 1f);
				Main.dust[num190].velocity *= 0.5f;
				Main.dust[num190].scale *= 1.3f;
				Main.dust[num190].fadeIn = 1f;
				Main.dust[num190].noGravity = true;
			}
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item89, projectile.position);
			projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
			projectile.width = (int)(128f * projectile.scale);
			projectile.height = (int)(128f * projectile.scale);
			projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
			projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
			for (int num336 = 0; num336 < 8; num336++)
			{
				Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 1.5f);
			}
			for (int num337 = 0; num337 < 32; num337++)
			{
				int num338 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 2.5f);
				Main.dust[num338].noGravity = true;
				Main.dust[num338].velocity *= 3f;
				num338 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 244, 0f, 0f, 100, new Color(255, Main.DiscoG, 0), 1.5f);
				Main.dust[num338].velocity *= 2f;
				Main.dust[num338].noGravity = true;
			}
			for (int num339 = 0; num339 < 2; num339++)
			{
				int num340 = Gore.NewGore(projectile.position + new Vector2((float)(projectile.width * Main.rand.Next(100)) / 100f, (float)(projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num340].velocity *= 0.3f;
				Gore expr_B4D2_cp_0 = Main.gore[num340];
				expr_B4D2_cp_0.velocity.X = expr_B4D2_cp_0.velocity.X + (float)Main.rand.Next(-10, 11) * 0.05f;
				Gore expr_B502_cp_0 = Main.gore[num340];
				expr_B502_cp_0.velocity.Y = expr_B502_cp_0.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.05f;
			}
			if (projectile.owner == Main.myPlayer)
			{
				projectile.localAI[1] = -1f;
				projectile.maxPenetrate = 0;
				projectile.Damage();
			}
			for (int num341 = 0; num341 < 5; num341++)
			{
				int num342 = Utils.SelectRandom<int>(Main.rand, new int[]
				{
					244,
					259,
					158
				});
				int num343 = Dust.NewDust(projectile.position, projectile.width, projectile.height, num342, 2.5f * (float)projectile.direction, -2.5f, 0, new Color(255, Main.DiscoG, 0), 1f);
				Main.dust[num343].alpha = 200;
				Main.dust[num343].velocity *= 2.4f;
				Main.dust[num343].scale += Main.rand.NextFloat();
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
        	Player player = Main.player[projectile.owner];
        	player.AddBuff(mod.BuffType("Molten"), 360);
		}
    }
}