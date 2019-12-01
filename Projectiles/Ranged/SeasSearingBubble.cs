using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Ranged
{
    public class SeasSearingBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Searing Bubble");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 2;
            projectile.timeLeft = 480;
            projectile.ranged = true;
			projectile.extraUpdates = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
                if (projectile.alpha < 0)
                    projectile.alpha = 0;
            }
			if (projectile.timeLeft < 475)
			{
				for (int num105 = 0; num105 < 2; num105++)
				{
					float num99 = projectile.velocity.X / 3f * (float)num105;
					float num100 = projectile.velocity.Y / 3f * (float)num105;
					int num101 = 4;
					int num102 = Dust.NewDust(new Vector2(projectile.position.X + (float)num101, projectile.position.Y + (float)num101), projectile.width - num101 * 2, projectile.height - num101 * 2, 202, 0f, 0f, 150, new Color(60, Main.DiscoG, 190), 1.2f);
					Main.dust[num102].noGravity = true;
					Main.dust[num102].velocity *= 0.1f;
					Main.dust[num102].velocity += projectile.velocity * 0.1f;
					Dust expr_47FA_cp_0 = Main.dust[num102];
					expr_47FA_cp_0.position.X -= num99;
					Dust expr_4815_cp_0 = Main.dust[num102];
					expr_4815_cp_0.position.Y -= num100;
				}
				if (Main.rand.NextBool(10))
				{
					int num103 = 4;
					int num104 = Dust.NewDust(new Vector2(projectile.position.X + (float)num103, projectile.position.Y + (float)num103), projectile.width - num103 * 2, projectile.height - num103 * 2, 202, 0f, 0f, 150, new Color(60, Main.DiscoG, 190), 0.6f);
					Main.dust[num104].velocity *= 0.25f;
					Main.dust[num104].velocity += projectile.velocity * 0.5f;
				}
			}
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(60, Main.DiscoG, 190, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 96);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 202, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, new Color(60, Main.DiscoG, 190));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 96);
            target.AddBuff(BuffID.Wet, 240);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 240);

            int type = ModContent.ProjectileType<SeasSearingBubble>();
			if (projectile.ai[0] == 1f)
			{
				int numWaterBlasts = 2;
				int waterDamage = SeasSearing.BaseDamage / 2;
				float waterKB = 1f;
				Player owner = Main.player[projectile.owner];
				for (int i = 0; i < numWaterBlasts; ++i)
				{
					float startOffsetX = Main.rand.NextFloat(1000f, 1400f) * (Main.rand.NextBool() ? -1f : 1f);
					float startOffsetY = Main.rand.NextFloat(80f, 900f) * (Main.rand.NextBool() ? -1f : 1f);
					Vector2 startPos = new Vector2(target.Center.X + startOffsetX, target.Center.Y + startOffsetY);
					float dx = target.Center.X - startPos.X;
					float dy = target.Center.Y - startPos.Y;

					// Add some randomness / inaccuracy
					dx += Main.rand.NextFloat(-5f, 5f);
					dy += Main.rand.NextFloat(-5f, 5f);
					float speed = Main.rand.NextFloat(20f, 25f);
					float dist = (float)Math.Sqrt((double)(dx * dx + dy * dy));
					dist = speed / dist;
					dx *= dist;
					dy *= dist;
					Vector2 waterVel = new Vector2(dx, dy);
					float angle = Main.rand.NextFloat(MathHelper.TwoPi);
					if (projectile.owner == Main.myPlayer)
					{
						int idx = Projectile.NewProjectile(startPos, waterVel, type, waterDamage, waterKB, projectile.owner, 0f, 0f);
						Main.projectile[idx].rotation = angle;
						Main.projectile[idx].tileCollide = false;
						Main.projectile[idx].usesLocalNPCImmunity = true;
						Main.projectile[idx].localNPCHitCooldown = -1;
					}
				}
			}
        }
    }
}
