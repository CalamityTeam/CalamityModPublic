using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Buffs;
namespace CalamityMod.Projectiles
{
    public class PlagueStingerGoliath : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Homing Stinger");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (projectile.position.Y > projectile.ai[1])
                projectile.tileCollide = true;

            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;

            int num123 = (int)Player.FindClosest(projectile.Center, 1, 1);
            projectile.ai[0] += 1f;
            if (projectile.ai[0] < 110f && projectile.ai[0] > 30f)
            {
                float scaleFactor2 = projectile.velocity.Length();
                Vector2 vector17 = Main.player[num123].Center - projectile.Center;
                vector17.Normalize();
                vector17 *= scaleFactor2;
                projectile.velocity = (projectile.velocity * 24f + vector17) / 25f;
                projectile.velocity.Normalize();
                projectile.velocity *= scaleFactor2;
            }
            if (projectile.velocity.Length() < 18f)
            {
                projectile.velocity *= 1.02f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(projectile.Center.X, projectile.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.projectileTexture[projectile.type].Width / 2), (float)(Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Width, (float)(ModContent.GetTexture("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Height / Main.projFrames[projectile.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + projectile.gfxOffY);
            Color color = new Color(127 - projectile.alpha, 127 - projectile.alpha, 127 - projectile.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Red);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow"), vector,
                null, color, projectile.rotation, vector11, 1f, spriteEffects, 0f);
        }
    }
}
