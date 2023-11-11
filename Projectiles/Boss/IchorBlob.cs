using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class IchorBlob : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 56;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
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
            if (Projectile.position.Y > Projectile.ai[1] - 48f)
                Projectile.tileCollide = true;

            // Deal no damage and increment the variable used to kill the projectile after 15 seconds
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 900f)
            {
                Projectile.localAI[0] += 10f;
                Projectile.damage = 0;
            }

            // Kill the projectile 26 frames after it stops dealing damage
            if (Projectile.localAI[0] > 255f)
            {
                Projectile.Kill();
                Projectile.localAI[0] = 255f;
            }

            // Add yellow light based on alpha
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.2f / 255f, (255 - Projectile.alpha) * 0.16f / 255f, (255 - Projectile.alpha) * 0.04f / 255f);

            // Adjust projectile visibility based on the kill timer
            Projectile.alpha = (int)(100.0 + Projectile.localAI[0] * 0.7);

            if (Projectile.velocity.Y != 0f && Projectile.ai[0] == 0f)
            {
                // Rotate based on velocity, only do this here, because it's falling
                Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) - MathHelper.PiOver2;

                Projectile.frameCounter++;
                if (Projectile.frameCounter > 6)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame > 1)
                    Projectile.frame = 0;
            }
            else
            {
                // Prevent sliding
                Projectile.velocity.X = 0f;

                // Do not animate falling frames
                Projectile.ai[0] = 1f;

                if (Projectile.frame < 2)
                {
                    // Set frame to blob and frame counter to 0
                    Projectile.frame = 2;
                    Projectile.frameCounter = 0;

                    // Play squish sound
                    SoundEngine.PlaySound(SoundID.NPCDeath21, Projectile.Center);

                    // Emit dust
                    Vector2 dustRotation = (Projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
                    Vector2 dustVelocity = dustRotation * Projectile.velocity.Length();
                    for (int i = 0; i < 10; i++)
                    {
                        int ichorDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 170, 0f, 0f, 200, default, 1.6f);
                        Dust dust = Main.dust[ichorDust];
                        dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                        dust.noGravity = true;
                        dust.velocity.Y -= 2f;
                        dust.velocity *= 3f;
                        dust.velocity += dustVelocity * Main.rand.NextFloat();
                        ichorDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 170, 0f, 0f, 100, default, 0.8f);
                        dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                        dust.velocity.Y -= 2f;
                        dust.velocity *= 2f;
                        dust.noGravity = true;
                        dust.fadeIn = 1f;
                        dust.velocity += dustVelocity * Main.rand.NextFloat();
                    }
                    for (int j = 0; j < 5; j++)
                    {
                        int ichorDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 170, 0f, 0f, 0, default, 2f);
                        Dust dust = Main.dust[ichorDust2];
                        dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                        dust.noGravity = true;
                        dust.velocity.Y -= 2f;
                        dust.velocity *= 0.5f;
                        dust.velocity += dustVelocity * (0.6f + 0.6f * Main.rand.NextFloat());
                    }
                }

                Projectile.rotation = 0f;
                Projectile.gfxOffY = 4f;

                Projectile.frameCounter++;
                if (Projectile.frameCounter > 6)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame > 5)
                    Projectile.frame = 5;
            }

            // Do velocity code after the frame code, to avoid messing anything up
            // Reduce x velocity every frame
            Projectile.velocity.X *= 0.995f;

            // Stop falling if water or lava is hit
            if (Projectile.wet || Projectile.lavaWet)
            {
                Projectile.velocity.Y = 0f;
            }
            else
            {
                // Fall
                Projectile.velocity.Y += 0.1f;
                if (Projectile.velocity.Y > 6f)
                    Projectile.velocity.Y = 6f;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 16f, targetHitbox);

        public override bool CanHitPlayer(Player target) => Projectile.localAI[1] <= 900f && Projectile.localAI[1] > 120f;

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.localAI[1] > 900f)
            {
                byte b2 = (byte)((26f - (Projectile.localAI[1] - 900f)) * 10f);
                byte a2 = (byte)(Projectile.alpha * (b2 / 255f));
                return new Color(b2, b2, b2, a2);
            }
            return new Color(255, 255, 255, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            if (Projectile.localAI[1] <= 900f && Projectile.localAI[1] > 120f)
                target.AddBuff(BuffID.Ichor, 240);
        }
    }
}
