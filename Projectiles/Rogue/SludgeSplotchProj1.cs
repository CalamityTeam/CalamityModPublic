using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SludgeSplotchProj1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sludge Splotch");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.1f;
            if (projectile.velocity.Y >= 16f)
            {
                projectile.velocity.Y = 16f;
            }

            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);

            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 191, 0f, 0f, 225, new Color(255, 255, 255), 3);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
                Main.dust[dust].velocity = Main.dust[dust].velocity * 0.25f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.Calamity().stealthStrike)
            {
                projectile.ai[0] = 0;
                projectile.ai[1] = 0;

                if (projectile.velocity.X != oldVelocity.X)
                {
                    if (oldVelocity.X < 0)
                    {
                        projectile.ai[0] = 1;
                    }
                    if (oldVelocity.X > 0)
                    {
                        projectile.ai[0] = -1;
                    }
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    if (oldVelocity.Y < 0)
                    {
                        projectile.ai[1] = 1;
                    }
                    if (oldVelocity.Y > 0)
                    {
                        projectile.ai[1] = -1;
                    }
                }
            }

            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 9, 2, 0);
            projectile.Kill();
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Slow, 120);
            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 9, 2, 0);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Slow, 120);
            Main.PlaySound(SoundID.NPCKilled, (int)projectile.position.X, (int)projectile.position.Y, 9, 2, 0);
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.Calamity().stealthStrike)
            {
                int sparkCount = Main.rand.Next(3, 6);
                for (int i = 0; i < sparkCount; i++)
                {
                    int sparkScatter = 1;
                    Vector2 sparkVelocity = new Vector2(Main.rand.NextFloat(-sparkScatter, sparkScatter), Main.rand.NextFloat(-sparkScatter - 2, sparkScatter + 2));

                    sparkVelocity.Normalize();
                    sparkVelocity *= 7;

                    Projectile.NewProjectile(projectile.Center, sparkVelocity, ModContent.ProjectileType<SludgeSplotchProj2>(), 7, 0, projectile.owner, 0, 0);
                }
            }

            int numDust = 20;
            int dustType = 191;
            float spread = 3f;
            for (int i = 0; i < numDust; i++)
            {
                Vector2 velocity = projectile.velocity + new Vector2(Main.rand.NextFloat(-spread, spread), Main.rand.NextFloat(-spread, spread));

                int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, velocity.X, velocity.Y, 175, default, 3f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
