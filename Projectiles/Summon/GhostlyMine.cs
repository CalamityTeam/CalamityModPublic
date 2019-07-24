using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class GhostlyMine : ModProjectile
	{
		public bool start = true;
        public bool spawnDust = true;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mine");
		}
		
		public override void SetDefaults()
		{
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 900;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.minion = true;
        }

		public override void AI()
		{
			if (start)
			{
				projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage;
				projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionProjectileDamageValue = projectile.damage;
				Main.PlaySound(SoundID.Item20, projectile.position);
                projectile.ai[1] = projectile.ai[0];
                start = false;
			}
			if (Main.player[projectile.owner].minionDamage != projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue)
			{
				int damage2 = (int)(((float)projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionProjectileDamageValue /
					projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).spawnedPlayerMinionDamageValue) *
					Main.player[projectile.owner].minionDamage);
				projectile.damage = damage2;
			}
			Vector2 direction = Main.player[projectile.owner].Center - projectile.Center;
			direction.Normalize();
			direction *= 9f;
            Player player = Main.player[projectile.owner];
			double deg = (double)projectile.ai[1];
			double rad = deg * (Math.PI / 180);
			double dist = 550;
            projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
            projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;
            projectile.ai[1] += 1f; //1f
            if (spawnDust)
            {
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 15; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default(Color), 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num624].velocity *= 2f;
                }
                spawnDust = false;
            }
		}

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.timeLeft < 85)
            {
                byte b2 = (byte)(projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
            projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
            projectile.width = 150;
            projectile.height = 150;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            for (int num621 = 0; num621 < 30; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default(Color), 1.2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 60; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default(Color), 1.7f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 180, 0f, 0f, 100, default(Color), 1f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}