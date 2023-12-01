using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class FrostMist : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.Opacity = 0f;
            Projectile.coldDamage = true;
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
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            if (Projectile.ai[1] == 0f)
            {
                for (int i = 0; i < 5; i++)
                {
                    int mistDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 187, 0f, 0f, 100, default, 2f);
                    Main.dust[mistDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[mistDust].scale = 0.5f;
                        Main.dust[mistDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                Projectile.ai[1] = 1f;
            }

            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] == 16f)
            {
                Projectile.localAI[1] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 dustRotation = Vector2.UnitX * -Projectile.width / 2f;
                    dustRotation += -Vector2.UnitY.RotatedBy(l * MathHelper.Pi / 6f) * new Vector2(8f, 16f);
                    dustRotation = dustRotation.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                    int extraMistDust = Dust.NewDust(Projectile.Center, 0, 0, 187, 0f, 0f, 160, default, 1f);
                    Main.dust[extraMistDust].scale = 1.1f;
                    Main.dust[extraMistDust].noGravity = true;
                    Main.dust[extraMistDust].position = Projectile.Center + dustRotation;
                    Main.dust[extraMistDust].velocity = Projectile.velocity * 0.1f;
                    Main.dust[extraMistDust].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[extraMistDust].position) * 1.25f;
                }
            }

            if (Projectile.timeLeft < 30)
                Projectile.Opacity = MathHelper.Clamp(Projectile.timeLeft / 30f, 0f, 1f);
            else
                Projectile.Opacity = MathHelper.Clamp(1f - ((Projectile.timeLeft - 270) / 30f), 0f, 1f);

            int playerTracker = Player.FindClosest(Projectile.Center, 1, 1);
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] < 140f && Projectile.ai[1] > 60f)
            {
                float projVelocityMult = Projectile.velocity.Length();
                Vector2 playerDistance = Main.player[playerTracker].Center - Projectile.Center;
                playerDistance.Normalize();
                playerDistance *= projVelocityMult;
                Projectile.velocity = (Projectile.velocity * 24f + playerDistance) / 25f;
                Projectile.velocity.Normalize();
                Projectile.velocity *= projVelocityMult;
            }
            if (Projectile.velocity.Length() < 12f)
                Projectile.velocity *= 1.02f;

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, 0f, 0.35f * Projectile.Opacity, 0.35f * Projectile.Opacity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool CanHitPlayer(Player target) => Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0 || Projectile.Opacity != 1f)
                return;

            target.AddBuff(BuffID.Frostburn, 180, true);
            target.AddBuff(BuffID.Chilled, 90, true);
        }
    }
}
