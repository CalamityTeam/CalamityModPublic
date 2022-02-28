using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneHellblast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Hellblast");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 40;
            projectile.height = 40;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 255;
            cooldownSlot = 1;
		}

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter >= 10)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
                projectile.frame = 0;

			Lighting.AddLight(projectile.Center, 0.9f * projectile.Opacity, 0f, 0f);

			if (projectile.timeLeft < 51)
				projectile.Opacity -= 0.02f;

            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = 1f;
                Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 20);
            }

            if (projectile.velocity.Length() < 18f)
                projectile.velocity *= 1.03f;

            if (projectile.velocity.X < 0f)
            {
                projectile.spriteDirection = -1;
                projectile.rotation = (float)Math.Atan2(-projectile.velocity.Y, -projectile.velocity.X);
            }
            else
            {
                projectile.spriteDirection = 1;
                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			lightColor.R = (byte)(255 * projectile.Opacity);
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

		public override bool CanHitPlayer(Player target) => projectile.timeLeft >= 51;

		public override void OnHitPlayer(Player target, int damage, bool crit)
        {
			if (projectile.timeLeft < 51)
				return;

			if (projectile.ai[0] == 0f)
			{
				target.AddBuff(ModContent.BuffType<AbyssalFlames>(), 180);
				target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120);
			}
			else
				target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120);
		}

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.position.X, (int)projectile.position.Y, 20);
            for (int dust = 0; dust <= 5; dust++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f);
            }
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
