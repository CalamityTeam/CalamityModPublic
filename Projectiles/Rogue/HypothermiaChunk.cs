using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Projectiles.Rogue
{
	public class HypothermiaChunk : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ice Chunk");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 80;
            projectile.extraUpdates = 3;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            //Rotation
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            Lighting.AddLight(projectile.Center, 0.1f, 0f, 0.5f);
            if (projectile.ai[0] < 0.2f)
            {
                projectile.ai[0] += 0.1f;
            }
            else
            {
                projectile.tileCollide = true;
            }

            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 191, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.8f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i <= 3; i++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 191, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.8f);
            }
            if (projectile.owner == Main.myPlayer)
            {
				for (int i = 0; i < Main.rand.Next(1,4); i++)
				{
					Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
					int shard = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<HypothermiaShard>(), (int)(projectile.damage * 0.33f), projectile.knockBack * 0.75f, Main.myPlayer, Main.rand.Next(4), 1f);
				}
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
		}

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 300);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
