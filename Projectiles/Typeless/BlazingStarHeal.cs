using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Typeless
{
    public class BlazingStarHeal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 4 == 0) //only once per 4 frames
                Lighting.AddLight(Projectile.Center, 0f, 0.6f, 0f);
            if (Projectile.timeLeft > 190)
                Projectile.velocity *= 1.1f;
            else if (Projectile.timeLeft <= 190)
                Projectile.velocity *= 0.99f;
            if (Projectile.timeLeft <= 160)
                Projectile.velocity = Vector2.Zero;
            
            int index = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            Player player = Main.player[index];
            if (Projectile.timeLeft > 190 || player is null || Main.player[Projectile.owner].team != player.team)
                return;

            float playerDist = Vector2.Distance(player.Center, Projectile.Center);
            if (!player.immune && playerDist < 50f && !player.dead && Projectile.position.X < player.position.X + player.width && Projectile.position.X + Projectile.width > player.position.X && Projectile.position.Y < player.position.Y + player.height && Projectile.position.Y + Projectile.height > player.position.Y)
            {
                int healAmt = Utils.Clamp((200 - Projectile.timeLeft) / 10, 1, 10); //min heal is 5, max heal is 10, achievable after 2 seconds
                player.HealEffect(healAmt, false);
                player.statLife += healAmt;
                if (player.statLife > player.statLifeMax2)
                {
                    player.statLife = player.statLifeMax2;
                }
                NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, index, healAmt);
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value = ModContent.Request<Texture2D>(Texture).Value;
            Color color33 = new Color(54, 209, 54, 0);
            Vector2 vector28 = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color color34 = color33;
            Vector2 origin5 = value.Size() / 2f;
            Color color35 = color33 * 0.5f;
            float num162 = Utils.GetLerpValue(15f, 30f, Projectile.timeLeft, clamped: true) * Utils.GetLerpValue(240f, 200f, Projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 30f / 0.5f * ((float)Math.PI * 2f) * 3f)) * 0.8f;
            Vector2 vector29 = new Vector2(0.5f, 1f) * num162;
            Vector2 vector30 = new Vector2(0.5f, 1f) * num162;
            color34 *= num162;
            color35 *= num162;

            int num163 = 0;
            Vector2 position3 = vector28 + Projectile.velocity.SafeNormalize(Vector2.Zero) * Utils.GetLerpValue(0.5f, 1f, Projectile.localAI[0] / 60f, clamped: true) * num163;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(value, position3, null, color34, (float)Math.PI / 2f, origin5, vector29, spriteEffects, 0);
            Main.EntitySpriteDraw(value, position3, null, color34, 0f, origin5, vector30, spriteEffects, 0);
            Main.EntitySpriteDraw(value, position3, null, color35, (float)Math.PI / 2f, origin5, vector29 * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(value, position3, null, color35, 0f, origin5, vector30 * 0.6f, spriteEffects, 0);

            Main.EntitySpriteDraw(value, position3, null, color34, MathHelper.PiOver4, origin5, vector29 * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(value, position3, null, color34, MathHelper.PiOver4 * 3f, origin5, vector30 * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(value, position3, null, color35, MathHelper.PiOver4, origin5, vector29 * 0.36f, spriteEffects, 0);
            Main.EntitySpriteDraw(value, position3, null, color35, MathHelper.PiOver4 * 3f, origin5, vector30 * 0.36f, spriteEffects, 0);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.position.X = Projectile.position.X + (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (Projectile.height / 2);
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            for (int num621 = 0; num621 < 5; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                Main.dust[num622].noGravity = true;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 8; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 247, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }
        }

        public override bool? CanDamage() => false;
    }
}
