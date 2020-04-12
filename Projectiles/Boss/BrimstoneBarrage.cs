using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneBarrage : ModProjectile
    {
		private bool start = true;
		private float startingPosX = 0f;
		private float startingPosY = 0f;
		private double distance = 0D;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Dart");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            cooldownSlot = 1;
        }

		public override void AI()
        {
			// Normal AI
			if (projectile.ai[0] < 2f)
			{
				if (projectile.velocity.Length() < (projectile.ai[1] == 0f ? 16f : 12f))
					projectile.velocity *= 1.01f;
			}

			// Special rotation for test AI
			else
			{
				if (start)
				{
					startingPosX = projectile.Center.X;
					startingPosY = projectile.Center.Y;
					start = false;
				}

				double deg = (double)projectile.ai[1];
				double rad = deg * (Math.PI / 180);
				distance += 2D;
				if (projectile.ai[0] == 2f)
				{
					projectile.position.X = startingPosX - (int)(Math.Cos(rad) * distance) - projectile.width / 2;
					projectile.position.Y = startingPosY - (int)(Math.Sin(rad) * distance) - projectile.height / 2;
				}
				else
				{
					projectile.position.X = startingPosX - (int)(Math.Sin(rad) * distance) - projectile.width / 2;
					projectile.position.Y = startingPosY - (int)(Math.Cos(rad) * distance) - projectile.height / 2;
				}

				projectile.ai[1] += 1f;
			}

			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;

			projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

            Lighting.AddLight(projectile.Center, 0.75f, 0f, 0f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 50, 50, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 180);

            if (projectile.ai[0] == 0f)
                target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
