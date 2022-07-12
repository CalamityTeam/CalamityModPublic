using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.scale = 1.2f;
            Projectile.hostile = true;
            Projectile.coldDamage = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            Lighting.AddLight((int)((Projectile.position.X + (Projectile.width / 2)) / 16f), (int)((Projectile.position.Y + (Projectile.height / 2)) / 16f), 0f, 0.25f, 0.25f);

            if (Projectile.ai[0] == 0f)
            {
                if (Projectile.velocity.Length() < Projectile.ai[1])
                    Projectile.velocity *= 1.015f;

                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

                for (int num322 = 0; num322 < 2; num322++)
                {
                    int num323 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 92, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 0.6f);
                    Main.dust[num323].noGravity = true;
                    Dust dust = Main.dust[num323];
                    dust.velocity *= 0.3f;
                }
            }
            else if (Projectile.ai[0] == 1f)
            {
                if (Projectile.velocity.Length() < 10f)
                    Projectile.velocity *= (CalamityWorld.revenge || BossRushEvent.BossRushActive) ? 1.03f : 1.025f;

                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

                for (int num322 = 0; num322 < 2; num322++)
                {
                    int num323 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 92, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 0.6f);
                    Main.dust[num323].noGravity = true;
                    Dust dust = Main.dust[num323];
                    dust.velocity *= 0.3f;
                }
            }
            else if (Projectile.ai[0] == 2f)
            {
                Projectile.velocity.Y += 0.1f;

                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

                if (Projectile.velocity.Y > 6f)
                    Projectile.velocity.Y = 6f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, Projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27 with { Volume = SoundID.Item27.Volume * 0.25f }, Projectile.Center);
            for (int num373 = 0; num373 < 3; num373++)
            {
                int num374 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 76, 0f, 0f, 0, default, 1f);
                Main.dust[num374].noGravity = true;
                Main.dust[num374].noLight = true;
                Main.dust[num374].scale = 0.7f;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, 120, true);
            target.AddBuff(BuffID.Chilled, 60, true);
        }
    }
}
