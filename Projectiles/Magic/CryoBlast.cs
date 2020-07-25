using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class CryoBlast : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 35;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 4;
            projectile.timeLeft = 600;
			projectile.coldDamage = true;
        }

        public override void AI()
        {
            if (projectile.scale <= 3.6f)
            {
                projectile.scale *= 1.01f;
				projectile.position = projectile.Center;
                projectile.width = (int)(35f * projectile.scale);
                projectile.height = (int)(35f * projectile.scale);
				projectile.Center = projectile.position;
            }

			if (projectile.timeLeft < 53)
				projectile.alpha += 5;

            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            if (projectile.frameCounter++ % 4 == 3)
            {
                projectile.frame++;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0.5f);

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
				int ice = Dust.NewDust(projectile.position, projectile.width, projectile.height, 66, 0f, 0f, 100, default, projectile.scale * 0.5f);
				Main.dust[ice].noGravity = true;
				Main.dust[ice].velocity *= 0f;
				int snow = Dust.NewDust(projectile.position, projectile.width, projectile.height, 185, 0f, 0f, 100, default, projectile.scale * 0.5f);
				Main.dust[snow].noGravity = true;
				Main.dust[snow].velocity *= 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			if (projectile.timeLeft > 599)
				return false;

			CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GlacialState>(), 360);
            target.AddBuff(BuffID.Frostburn, 360);
        }
    }
}
