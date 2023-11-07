using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class ScavengerLaser : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
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
                int laserDust = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 244, -Projectile.velocity.X / 3f, -Projectile.velocity.Y / 3f, 150, Color.Transparent, 0.6f);
                Main.dust[laserDust].noGravity = true;
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

                Vector2 playerDistance = Main.player[(int)Projectile.ai[1]].Center - Projectile.Center;
                if (playerDistance.Length() < 20f)
                {
                    Projectile.Kill();
                    return;
                }

                playerDistance.Normalize();
                playerDistance *= 14f;
                playerDistance = Vector2.Lerp(Projectile.velocity, playerDistance, 0.6f);
                if (playerDistance.Y < speed)
                    playerDistance.Y = speed;

                float projVelocityMult = 0.4f;
                if (Projectile.velocity.X < playerDistance.X)
                {
                    Projectile.velocity.X = Projectile.velocity.X + projVelocityMult;
                    if (Projectile.velocity.X < 0f && playerDistance.X > 0f)
                        Projectile.velocity.X = Projectile.velocity.X + projVelocityMult;
                }
                else if (Projectile.velocity.X > playerDistance.X)
                {
                    Projectile.velocity.X = Projectile.velocity.X - projVelocityMult;
                    if (Projectile.velocity.X > 0f && playerDistance.X < 0f)
                        Projectile.velocity.X = Projectile.velocity.X - projVelocityMult;
                }

                if (Projectile.velocity.Y < playerDistance.Y)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + projVelocityMult;
                    if (Projectile.velocity.Y < 0f && playerDistance.Y > 0f)
                        Projectile.velocity.Y = Projectile.velocity.Y + projVelocityMult;
                }
                else if (Projectile.velocity.Y > playerDistance.Y)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - projVelocityMult;
                    if (Projectile.velocity.Y > 0f && playerDistance.Y < 0f)
                        Projectile.velocity.Y = Projectile.velocity.Y - projVelocityMult;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

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

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Zombie103, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1.5f);
            }
            for (int j = 0; j < 20; j++)
            {
                int killDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 0, default, 2.5f);
                Main.dust[killDust].noGravity = true;
                Main.dust[killDust].velocity *= 3f;
                killDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 1.5f);
                Main.dust[killDust].velocity *= 2f;
                Main.dust[killDust].noGravity = true;
            }
            Projectile.Damage();
        }
    }
}
