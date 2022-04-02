using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class ShockblastRound : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Round");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.light = 0.5f;
            projectile.extraUpdates = 3;
            aiType = ProjectileID.Bullet;
			projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			if (projectile.owner == Main.myPlayer)
			{
				int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<Shockblast>(), projectile.damage, 0f, projectile.owner, 0f, projectile.ai[1]);
				Main.projectile[proj].scale = (projectile.ai[1] * 0.5f) + 1f;
			}
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool PreAI()
        {
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * projectile.direction;

			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				for (int num136 = 0; num136 < 2; num136++)
				{
					Vector2 dspeed = -projectile.velocity * Main.rand.NextFloat(0.5f, 0.7f);
					float x2 = projectile.Center.X - projectile.velocity.X / 10f * num136;
					float y2 = projectile.Center.Y - projectile.velocity.Y / 10f * num136;
					int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 185, 0f, 0f, 0, default, 1f);
					Main.dust[num137].alpha = projectile.alpha;
					Main.dust[num137].position.X = x2;
					Main.dust[num137].position.Y = y2;
					Main.dust[num137].velocity = dspeed;
					Main.dust[num137].noGravity = true;
				}
			}

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.owner == Main.myPlayer)
			{
				int proj = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<Shockblast>(), projectile.damage, 0f, projectile.owner, 0f, projectile.ai[1]);
				Main.projectile[proj].scale = (projectile.ai[1] * 0.5f) + 1f;
			}

            if (Main.player[projectile.owner].moonLeech)
                return;

            float healAmt = damage * 0.05f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

			if (healAmt > CalamityMod.lifeStealCap)
				healAmt = CalamityMod.lifeStealCap;

			CalamityGlobalProjectile.SpawnLifeStealProjectile(projectile, Main.player[projectile.owner], healAmt, ModContent.ProjectileType<TransfusionTrail>(), 1200f, 3f);
        }
    }
}
