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

            int playerTracker = Player.FindClosest(Projectile.Center, 1, 1);
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] < (60f + Projectile.ai[0] * 0.5f) && Projectile.localAI[0] > 30f)
            {
                float projVelocity = Projectile.velocity.Length();
                Vector2 playerDirection = Main.player[playerTracker].Center - Projectile.Center;
                playerDirection.Normalize();
                playerDirection *= projVelocity;
                Projectile.velocity = (Projectile.velocity * 24f + playerDirection) / 25f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= projVelocity;
            }

            if (Projectile.velocity.Length() < (16f + Projectile.ai[0] * 0.04f))
                Projectile.velocity *= 1.02f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Plague>(), 90);
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 center = new Vector2(Projectile.Center.X, Projectile.Center.Y);
            Vector2 textureArea = new Vector2(ModContent.Request<Texture2D>(Texture).Value.Width / 2, ModContent.Request<Texture2D>(Texture).Value.Height / 2);
            Vector2 drawArea = center - Main.screenPosition;
            drawArea -= new Vector2(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Value.Width, ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Value.Height) * 1f / 2f;
            drawArea += textureArea * 1f + new Vector2(0f, 4f + Projectile.gfxOffY);
            Color whiteColor = Color.White;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/PlagueStingerGoliathGlow").Value, drawArea,
                null, whiteColor, Projectile.rotation, textureArea, Projectile.scale, SpriteEffects.None, 0);
        }
    }
}
