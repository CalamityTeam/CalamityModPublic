using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class PlagueStingerGoliath : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.scale = 1.5f;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
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

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 center = new Vector2(Projectile.Center.X, Projectile.Center.Y);
            Vector2 vector11 = new Vector2(ModContent.Request<Texture2D>(Texture).Value.Width / 2, ModContent.Request<Texture2D>(Texture).Value.Height / 2);
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Value.Width, ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Value.Height) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 4f + Projectile.gfxOffY);
            Color color = Color.White;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Value, vector,
                null, color, Projectile.rotation, vector11, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}
