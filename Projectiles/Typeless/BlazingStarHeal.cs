using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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
            Texture2D starTexture = ModContent.Request<Texture2D>(Texture).Value;
            Color healGreen = new Color(54, 209, 54, 0);
            Vector2 projPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Color healGreenDraw = healGreen;
            Vector2 colorSize = starTexture.Size() / 2f;
            Color lesserHealGreen = healGreen * 0.5f;
            float colorLerp = Utils.GetLerpValue(15f, 30f, Projectile.timeLeft, clamped: true) * Utils.GetLerpValue(240f, 200f, Projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 30f / 0.5f * ((float)Math.PI * 2f) * 3f)) * 0.8f;
            Vector2 scaledColorLerp1 = new Vector2(0.5f, 1f) * colorLerp;
            Vector2 scaledColorLerp2 = new Vector2(0.5f, 1f) * colorLerp;
            healGreenDraw *= colorLerp;
            lesserHealGreen *= colorLerp;

            Vector2 drawPos = projPos + Projectile.velocity.SafeNormalize(Vector2.Zero) * Utils.GetLerpValue(0.5f, 1f, Projectile.localAI[0] / 60f, clamped: true) * 0;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(starTexture, drawPos, null, healGreenDraw, (float)Math.PI / 2f, colorSize, scaledColorLerp1, spriteEffects, 0);
            Main.EntitySpriteDraw(starTexture, drawPos, null, healGreenDraw, 0f, colorSize, scaledColorLerp2, spriteEffects, 0);
            Main.EntitySpriteDraw(starTexture, drawPos, null, lesserHealGreen, (float)Math.PI / 2f, colorSize, scaledColorLerp1 * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(starTexture, drawPos, null, lesserHealGreen, 0f, colorSize, scaledColorLerp2 * 0.6f, spriteEffects, 0);

            Main.EntitySpriteDraw(starTexture, drawPos, null, healGreenDraw, MathHelper.PiOver4, colorSize, scaledColorLerp1 * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(starTexture, drawPos, null, healGreenDraw, MathHelper.PiOver4 * 3f, colorSize, scaledColorLerp2 * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(starTexture, drawPos, null, lesserHealGreen, MathHelper.PiOver4, colorSize, scaledColorLerp1 * 0.36f, spriteEffects, 0);
            Main.EntitySpriteDraw(starTexture, drawPos, null, lesserHealGreen, MathHelper.PiOver4 * 3f, colorSize, scaledColorLerp2 * 0.36f, spriteEffects, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.position.X = Projectile.position.X + (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (Projectile.height / 2);
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            for (int i = 0; i < 5; i++)
            {
                int duster = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[duster].velocity *= 3f;
                Main.dust[duster].noGravity = true;
                if (Main.rand.NextBool())
                {
                    Main.dust[duster].scale = 0.5f;
                    Main.dust[duster].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 8; j++)
            {
                int duster2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 247, 0f, 0f, 100, default, 3f);
                Main.dust[duster2].noGravity = true;
                Main.dust[duster2].velocity *= 5f;
                duster2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[duster2].velocity *= 2f;
                Main.dust[duster2].noGravity = true;
            }
        }

        public override bool? CanDamage() => false;
    }
}
