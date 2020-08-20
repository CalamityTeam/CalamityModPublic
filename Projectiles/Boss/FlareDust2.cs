using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class FlareDust2 : ModProjectile
    {
		private bool start = true;
		private Vector2 velocity = Vector2.Zero;

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flare Bomb");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.scale = 1.5f;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 840;
            cooldownSlot = 1;
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(start);
			writer.WriteVector2(velocity);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			start = reader.ReadBoolean();
			velocity = reader.ReadVector2();
		}

		public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter > 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }

            if (projectile.ai[0] == 1f)
            {
				if (projectile.timeLeft < 630)
				{
					if (projectile.velocity.Length() < 6f)
					{
						projectile.velocity *= 1.025f;
					}
					else
					{
						if (start)
						{
							velocity = projectile.velocity;
							start = false;
						}

						projectile.ai[1] += 0.1f;

						float amplitude = 2f;

						float wavyVelocity = (float)Math.Cos(projectile.ai[1]);

						projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity) * amplitude;
					}
				}
            }
            else
            {
				if (projectile.timeLeft < 420)
				{
					if (projectile.velocity.Length() < 12f)
					{
						projectile.velocity *= 1.05f;
					}
					else
					{
						if (start)
						{
							velocity = projectile.velocity;
							start = false;
						}

						projectile.ai[1] += 0.1f;

						float amplitude = 2f;

						float wavyVelocity = (float)Math.Sin(projectile.ai[1]);

						projectile.velocity = velocity + new Vector2(wavyVelocity, wavyVelocity) * amplitude;
					}
				}
            }

            Lighting.AddLight(projectile.Center, 0.5f, 0.25f, 0f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 53, projectile.alpha);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item14, projectile.position);
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 48;
            projectile.position.X = projectile.position.X - (projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (projectile.height / 2);
            projectile.Damage();
            for (int num621 = 0; num621 < 2; num621++)
            {
                int num622 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 4; num623++)
            {
                int num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 2f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 244, 0f, 0f, 100, default, 1f);
                Main.dust[num624].velocity *= 2f;
            }
			CalamityUtils.ExplosionGores(projectile.Center, 3);
        }

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(ModContent.BuffType<LethalLavaBurn>(), 180);
		}

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
        {
			target.Calamity().lastProjectileHit = projectile;
		}
    }
}
