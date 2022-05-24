using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Boss
{
    public class ScavengerLaser : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Dart");
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
                Projectile.frame = 0;

            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.alpha < 40)
            {
                int num805 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 244, -Projectile.velocity.X / 3f, -Projectile.velocity.Y / 3f, 150, Color.Transparent, 0.6f);
                Main.dust[num805].noGravity = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.ai[1] == -1f)
            {
                if (Projectile.velocity.Length() < 18f)
                    Projectile.velocity *= 1.05f;
                else
                    Projectile.tileCollide = true;

                return;
            }

            if (Projectile.ai[0] == 0f)
            {
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] >= 45f)
                {
                    Projectile.localAI[0] = 0f;
                    Projectile.ai[0] = 1f;
                    Projectile.ai[1] = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
                    Projectile.netUpdate = true;
                }

                Projectile.velocity.X = Projectile.velocity.RotatedBy(0.0, default).X;
                Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X, -6f, 6f);

                Projectile.velocity.Y = Projectile.velocity.Y - 0.08f;
                if (Projectile.velocity.Y > 0f)
                    Projectile.velocity.Y = Projectile.velocity.Y - 0.2f;
                if (Projectile.velocity.Y < -7f)
                    Projectile.velocity.Y = -7f;
            }
            else if (Projectile.ai[0] == 1f)
            {
                if (Main.player[(int)Projectile.ai[1]].Center.Y > Projectile.Center.Y + 90f)
                {
                    Projectile.ai[0] = 2f;
                    Projectile.netUpdate = true;
                }

                Projectile.velocity.X = Projectile.velocity.RotatedBy(0.0, default).X;
                Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X, -6f, 6f);

                Projectile.velocity.Y = Projectile.velocity.Y - 0.08f;
                if (Projectile.velocity.Y > 0f)
                    Projectile.velocity.Y = Projectile.velocity.Y - 0.2f;
                if (Projectile.velocity.Y < -7f)
                    Projectile.velocity.Y = -7f;
            }
            else if (Projectile.ai[0] == 2f)
            {
                if (Main.player[(int)Projectile.ai[1]].Center.Y < Projectile.Center.Y)
                    Projectile.tileCollide = true;

                float speed = revenge ? 9f : 7.5f;

                Vector2 vector70 = Main.player[(int)Projectile.ai[1]].Center - Projectile.Center;
                if (vector70.Length() < 20f)
                {
                    Projectile.Kill();
                    return;
                }

                vector70.Normalize();
                vector70 *= 14f;
                vector70 = Vector2.Lerp(Projectile.velocity, vector70, 0.6f);
                if (vector70.Y < speed)
                    vector70.Y = speed;

                float num804 = 0.4f;
                if (Projectile.velocity.X < vector70.X)
                {
                    Projectile.velocity.X = Projectile.velocity.X + num804;
                    if (Projectile.velocity.X < 0f && vector70.X > 0f)
                        Projectile.velocity.X = Projectile.velocity.X + num804;
                }
                else if (Projectile.velocity.X > vector70.X)
                {
                    Projectile.velocity.X = Projectile.velocity.X - num804;
                    if (Projectile.velocity.X > 0f && vector70.X < 0f)
                        Projectile.velocity.X = Projectile.velocity.X - num804;
                }

                if (Projectile.velocity.Y < vector70.Y)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + num804;
                    if (Projectile.velocity.Y < 0f && vector70.Y > 0f)
                        Projectile.velocity.Y = Projectile.velocity.Y + num804;
                }
                else if (Projectile.velocity.Y > vector70.Y)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - num804;
                    if (Projectile.velocity.Y > 0f && vector70.Y < 0f)
                        Projectile.velocity.Y = Projectile.velocity.Y - num804;
                }
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 50, 50, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(CommonNPCSounds.GetZombieSound(103), Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            for (int num193 = 0; num193 < 2; num193++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1.5f);
            }
            for (int num194 = 0; num194 < 20; num194++)
            {
                int num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 0, default, 2.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
                num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1.5f);
                Main.dust[num195].velocity *= 2f;
                Main.dust[num195].noGravity = true;
            }
            Projectile.Damage();
        }
    }
}
