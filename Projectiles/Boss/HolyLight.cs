using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyLight : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Light");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
			projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 200;
        }

        public override void AI()
        {
			if (projectile.ai[0] < 240f)
			{
				projectile.ai[0] += 1f;

				if (projectile.timeLeft < 160)
					projectile.timeLeft = 160;
			}

			bool expertMode = Main.expertMode;

			if (projectile.velocity.Length() < 16f)
				projectile.velocity *= 1.01f;

            projectile.ai[1] = Player.FindClosest(projectile.position, projectile.width, projectile.height);
			int num487 = (int)projectile.ai[1];
			float num491 = Vector2.Distance(Main.player[num487].Center, projectile.Center);

			if (num491 < 50f && !Main.player[num487].dead && projectile.position.X < Main.player[num487].position.X + Main.player[num487].width && projectile.position.X + projectile.width > Main.player[num487].position.X && projectile.position.Y < Main.player[num487].position.Y + Main.player[num487].height && projectile.position.Y + projectile.height > Main.player[num487].position.Y)
            {
                int num492 = expertMode ? 50 : 35;
                Main.player[num487].HealEffect(num492, false);
                Main.player[num487].statLife += num492;
                if (Main.player[num487].statLife > Main.player[num487].statLifeMax2)
                {
                    Main.player[num487].statLife = Main.player[num487].statLifeMax2;
                }
                NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, num487, num492);
                projectile.Kill();
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D value = Main.projectileTexture[projectile.type];
			Color baseColor = new Color(100, 255, 100, 255);
			Color color33 = baseColor * 0.5f;
			color33.A = 0;
			Vector2 vector28 = projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
			Color color34 = color33;
			Vector2 origin5 = value.Size() / 2f;
			Color color35 = color33 * 0.5f;
			float num162 = CalamityUtils.GetLerpValue(15f, 30f, projectile.timeLeft, clamped: true) * CalamityUtils.GetLerpValue(240f, 200f, projectile.timeLeft, clamped: true) * (1f + 0.2f * (float)Math.Cos(Main.GlobalTime % 30f / 0.5f * ((float)Math.PI * 2f) * 3f)) * 0.8f;
			Vector2 vector29 = new Vector2(0.5f, 1f) * num162;
			Vector2 vector30 = new Vector2(0.5f, 1f) * num162;
			color34 *= num162;
			color35 *= num162;

			int num163 = 0;
			Vector2 position3 = vector28 + projectile.velocity.SafeNormalize(Vector2.Zero) * CalamityUtils.GetLerpValue(0.5f, 1f, projectile.localAI[0] / 60f, clamped: true) * num163;

			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(value, position3, null, color34, (float)Math.PI / 2f, origin5, vector29, spriteEffects, 0);
			spriteBatch.Draw(value, position3, null, color34, 0f, origin5, vector30, spriteEffects, 0);
			spriteBatch.Draw(value, position3, null, color35, (float)Math.PI / 2f, origin5, vector29 * 0.6f, spriteEffects, 0);
			spriteBatch.Draw(value, position3, null, color35, 0f, origin5, vector30 * 0.6f, spriteEffects, 0);

			spriteBatch.Draw(value, position3, null, color34, MathHelper.PiOver4, origin5, vector29 * 0.6f, spriteEffects, 0);
			spriteBatch.Draw(value, position3, null, color34, MathHelper.PiOver4 * 3f, origin5, vector30 * 0.6f, spriteEffects, 0);
			spriteBatch.Draw(value, position3, null, color35, MathHelper.PiOver4, origin5, vector29 * 0.36f, spriteEffects, 0);
			spriteBatch.Draw(value, position3, null, color35, MathHelper.PiOver4 * 3f, origin5, vector30 * 0.36f, spriteEffects, 0);

			return false;
		}

		public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.Center);
            projectile.position.X = projectile.position.X + (projectile.width / 2);
            projectile.position.Y = projectile.position.Y + (projectile.height / 2);
            projectile.width = 50;
            projectile.height = 50;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 15; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 247, 0f, 0f, 100, default, 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 246, 0f, 0f, 100, default, 2f);
                Main.dust[num624].velocity *= 2f;
            }
        }
    }
}
