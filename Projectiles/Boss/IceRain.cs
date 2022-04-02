using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class IceRain : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Rain");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 300;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Lighting.AddLight((int)((projectile.position.X + (projectile.width / 2)) / 16f), (int)((projectile.position.Y + (projectile.height / 2)) / 16f), 0f, 0.25f, 0.25f);

            if (projectile.ai[0] == 0f)
            {
                if (projectile.velocity.Length() < projectile.ai[1])
                    projectile.velocity *= 1.015f;

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                for (int num322 = 0; num322 < 2; num322++)
                {
                    int num323 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 92, projectile.velocity.X, projectile.velocity.Y, 50, default, 0.6f);
                    Main.dust[num323].noGravity = true;
                    Dust dust = Main.dust[num323];
                    dust.velocity *= 0.3f;
                }
            }
            else if (projectile.ai[0] == 1f)
            {
                projectile.aiStyle = 1;

                for (int num322 = 0; num322 < 2; num322++)
                {
                    int num323 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 92, projectile.velocity.X, projectile.velocity.Y, 50, default, 0.6f);
                    Main.dust[num323].noGravity = true;
                    Dust dust = Main.dust[num323];
                    dust.velocity *= 0.3f;
                }
            }
            else if (projectile.ai[0] == 2f)
            {
                projectile.velocity.Y += 0.1f;

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

                if (projectile.velocity.Y > 6f)
                    projectile.velocity.Y = 6f;
            }

            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.01f;
                projectile.alpha -= 50;
                if (projectile.alpha <= 0)
                {
                    projectile.localAI[0] = 1f;
                    projectile.alpha = 0;
                }
            }
            else
            {
                projectile.scale -= 0.01f;
                projectile.alpha += 50;
                if (projectile.alpha >= 255)
                {
                    projectile.localAI[0] = 0f;
                    projectile.alpha = 255;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Main.dayTime ? new Color(50, 50, 255, projectile.alpha) : new Color(255, 255, 255, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, (int)projectile.Center.X, (int)projectile.Center.Y, 27, 0.25f);
            for (int num373 = 0; num373 < 3; num373++)
            {
                int num374 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 76, 0f, 0f, 0, default, 1f);
                Main.dust[num374].noGravity = true;
                Main.dust[num374].noLight = true;
                Main.dust[num374].scale = 0.7f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 60, true);
            target.AddBuff(BuffID.Chilled, 30, true);
        }
    }
}
