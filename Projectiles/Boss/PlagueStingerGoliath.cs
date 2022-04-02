using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
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
            projectile.scale = 1.5f;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            if (projectile.position.Y > projectile.ai[1])
                projectile.tileCollide = true;

            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

            int num123 = Player.FindClosest(projectile.Center, 1, 1);
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] < (60f + projectile.ai[0] * 0.5f) && projectile.localAI[0] > 30f)
            {
                float scaleFactor2 = projectile.velocity.Length();
                Vector2 vector17 = Main.player[num123].Center - projectile.Center;
                vector17.Normalize();
                vector17 *= scaleFactor2;
                projectile.velocity = (projectile.velocity * 24f + vector17) / 25f;
                projectile.velocity.Normalize();
                projectile.velocity *= scaleFactor2;
            }

            if (projectile.velocity.Length() < (16f + projectile.ai[0] * 0.04f))
                projectile.velocity *= 1.02f;
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
            Vector2 vector11 = new Vector2(Main.projectileTexture[projectile.type].Width / 2, Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type] / 2);
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2(ModContent.GetTexture("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Width, ModContent.GetTexture("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Height / Main.projFrames[projectile.type]) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + projectile.gfxOffY);
            Color color = new Color(127 - projectile.alpha, 127 - projectile.alpha, 127 - projectile.alpha, 0).MultiplyRGBA(Color.Red);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow"), vector,
                null, color, projectile.rotation, vector11, projectile.scale, spriteEffects, 0f);
        }
    }
}
