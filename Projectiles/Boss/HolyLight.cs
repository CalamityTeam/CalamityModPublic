using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyLight : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 200;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.6f, 0f);

            if (Projectile.ai[0] < 240f)
            {
                Projectile.ai[0] += 1f;

                if (Projectile.timeLeft < 160)
                    Projectile.timeLeft = 160;
            }

            if (Projectile.velocity.Length() < 16f)
                Projectile.velocity *= 1.01f;

            int index = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
            Player player = Main.player[index];
            if (player is null)
                return;

            float playerDist = Vector2.Distance(player.Center, Projectile.Center);
            if (!player.immune && playerDist < 50f && !player.dead && Projectile.position.X < player.position.X + player.width && Projectile.position.X + Projectile.width > player.position.X && Projectile.position.Y < player.position.Y + player.height && Projectile.position.Y + Projectile.height > player.position.Y)
            {
                int healAmt = (int)Projectile.ai[1];
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
            Texture2D drawTexture = ModContent.Request<Texture2D>(Texture).Value;
            Color brightGreen = new Color(54, 209, 54, 0);
            Vector2 projDirection = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Vector2 halfTextureSize = drawTexture.Size() / 2f;
            Color halfBrightGreen = brightGreen * 0.5f;
            float timeLeftColorScale = Utils.GetLerpValue(15f, 30f, Projectile.timeLeft, clamped: true) * Utils.GetLerpValue(240f, 200f, Projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 30f / 0.5f * ((float)Math.PI * 2f) * 3f)) * 0.8f;
            Vector2 timeLeftDrawEffect = new Vector2(0.5f, 1f) * timeLeftColorScale;
            Vector2 timeLeftDrawEffect2 = new Vector2(0.5f, 1f) * timeLeftColorScale;
            brightGreen *= timeLeftColorScale;
            halfBrightGreen *= timeLeftColorScale;

            Vector2 position3 = projDirection + Projectile.velocity.SafeNormalize(Vector2.Zero) * Utils.GetLerpValue(0.5f, 1f, Projectile.localAI[0] / 60f, clamped: true) * 0;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(drawTexture, position3, null, brightGreen, (float)Math.PI / 2f, halfTextureSize, timeLeftDrawEffect, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, brightGreen, 0f, halfTextureSize, timeLeftDrawEffect2, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, halfBrightGreen, (float)Math.PI / 2f, halfTextureSize, timeLeftDrawEffect * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, halfBrightGreen, 0f, halfTextureSize, timeLeftDrawEffect2 * 0.6f, spriteEffects, 0);

            Main.EntitySpriteDraw(drawTexture, position3, null, brightGreen, MathHelper.PiOver4, halfTextureSize, timeLeftDrawEffect * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, brightGreen, MathHelper.PiOver4 * 3f, halfTextureSize, timeLeftDrawEffect2 * 0.6f, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, halfBrightGreen, MathHelper.PiOver4, halfTextureSize, timeLeftDrawEffect * 0.36f, spriteEffects, 0);
            Main.EntitySpriteDraw(drawTexture, position3, null, halfBrightGreen, MathHelper.PiOver4 * 3f, halfTextureSize, timeLeftDrawEffect2 * 0.36f, spriteEffects, 0);

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
                int holyYellow = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[holyYellow].velocity *= 3f;
                Main.dust[holyYellow].noGravity = true;
                if (Main.rand.NextBool())
                {
                    Main.dust[holyYellow].scale = 0.5f;
                    Main.dust[holyYellow].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 8; j++)
            {
                int holyYellow2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 247, 0f, 0f, 100, default, 3f);
                Main.dust[holyYellow2].noGravity = true;
                Main.dust[holyYellow2].velocity *= 5f;
                holyYellow2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[holyYellow2].velocity *= 2f;
                Main.dust[holyYellow2].noGravity = true;
            }
        }
    }
}
