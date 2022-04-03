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
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.scale = 1.5f;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.Calamity().affectedByMaliceModeVelocityMultiplier = true;
        }

        public override void AI()
        {
            if (Projectile.position.Y > Projectile.ai[1])
                Projectile.tileCollide = true;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            int num123 = Player.FindClosest(Projectile.Center, 1, 1);
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] < (60f + Projectile.ai[0] * 0.5f) && Projectile.localAI[0] > 30f)
            {
                float scaleFactor2 = Projectile.velocity.Length();
                Vector2 vector17 = Main.player[num123].Center - Projectile.Center;
                vector17.Normalize();
                vector17 *= scaleFactor2;
                Projectile.velocity = (Projectile.velocity * 24f + vector17) / 25f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= scaleFactor2;
            }

            if (Projectile.velocity.Length() < (16f + Projectile.ai[0] * 0.04f))
                Projectile.velocity *= 1.02f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(Projectile.Center.X, Projectile.Center.Y);
            Vector2 vector11 = new Vector2(Main.projectileTexture[Projectile.type].Width / 2, Main.projectileTexture[Projectile.type].Height / Main.projFrames[Projectile.type] / 2);
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Width, ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Height / Main.projFrames[Projectile.type]) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + Projectile.gfxOffY);
            Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow"), vector,
                null, color, Projectile.rotation, vector11, Projectile.scale, spriteEffects, 0f);
        }
    }
}
