using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class CragmawBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        // How long this laser can exist before it is deleted.
        public const int Lifetime = 120;

        // Pretty self explanatory
        private const float maximumLength = 1200f;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = Lifetime;
        }

        // Netcode for sending and receiving shit
        // localAI[0] is the timer, localAI[1] is the laser length

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
            NPC body = Main.npc[(int)Projectile.ai[1]];
            if (!body.active)
                Projectile.Kill();

            // Sometimes the beam would readjust to some other, weird angle. This fixes that.
            if (Projectile.velocity != -Vector2.UnitY)
            {
                Projectile.velocity = -Vector2.UnitY;
                Projectile.netUpdate = true;
            }

            if (Main.npc[(int)Projectile.ai[1]].active)
            {
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Top + new Vector2(0f, 4f);
            }

            // How fat the laser is
            float laserSize = 1.6f;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= Lifetime)
            {
                Projectile.Kill();
                return;
            }

            // Causes the effect where the laser appears to expand/contract at the beginning and end of its life
            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * MathHelper.Pi / Lifetime) * 10f * laserSize;
            if (Projectile.scale > laserSize)
            {
                Projectile.scale = laserSize;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            Vector2 samplingPoint = Projectile.Center;

            float[] samples = new float[3];

            float determinedLength = 0f;
            Collision.LaserScan(samplingPoint, Projectile.velocity, Projectile.width * Projectile.scale, maximumLength, samples);
            for (int i = 0; i < samples.Length; i++)
            {
                determinedLength += samples[i];
            }
            determinedLength /= samples.Length;

            determinedLength = MathHelper.Clamp(determinedLength, maximumLength * 0.5f, maximumLength);

            float lerpDelta = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], determinedLength, lerpDelta);
            Vector2 beamEndPosiiton = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 6f);

            // Release acid along the beam periodically.
            if (Projectile.timeLeft % 20 == 19 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 acidSpawnPosition = Projectile.Center + Projectile.velocity * Projectile.localAI[1] * Main.rand.NextFloat(0.1f, 0.8f);
                if (!Main.player[Player.FindClosest(acidSpawnPosition, 1, 1)].WithinRange(acidSpawnPosition, 300f))
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), acidSpawnPosition, Main.rand.NextVector2CircularEdge(8f, 8f), ModContent.ProjectileType<CragmawAcidDrop>(), Projectile.damage / 2, 0f);
            }

            if (WorldGen.SolidTile((int)(beamEndPosiiton.X / 16), (int)(beamEndPosiiton.Y / 16)))
            {
                for (int i = 0; i < 4; i++)
                {
                    float theta = Projectile.velocity.ToRotation() + Main.rand.NextBool().ToDirectionInt() * MathHelper.PiOver2;
                    float speed = (float)Main.rand.NextDouble() * 2f + 2f;
                    Vector2 velocity = theta.ToRotationVector2() * speed;
                    Dust dust = Dust.NewDustDirect(beamEndPosiiton, 0, 0, (int)CalamityDusts.SulfurousSeaAcid, velocity.X, velocity.Y, 0, default, 1f);
                    dust.noGravity = true;
                    dust.scale = 2.1f;
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustPerfect(beamEndPosiiton, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(55f)).RotatedBy(Projectile.rotation);
                    dust.noGravity = true;
                    dust.scale = 1.8f;
                }
                for (int i = 0; i < 16; i++)
                {
                    Dust dust = Dust.NewDustPerfect(beamEndPosiiton, (int)CalamityDusts.SulfurousSeaAcid);
                    dust.velocity = (i / 16f * MathHelper.TwoPi).ToRotationVector2() * 3f;
                    dust.noGravity = true;
                    dust.scale = 2.4f;
                }
            }
            // Draw acid green light across the laser
            DelegateMethods.v3_1 = new Vector3(0.62f, 0.94f, 0.38f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D laserTailTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/CragmawMireBeamBegin", AssetRequestMode.ImmediateLoad).Value;
            Texture2D laserBodyTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/CragmawMireBeamMid", AssetRequestMode.ImmediateLoad).Value;
            Texture2D laserHeadTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/CragmawMireBeamEnd", AssetRequestMode.ImmediateLoad).Value;
            float laserLength = Projectile.localAI[1];
            Color drawColor = new Color(1f, 1f, 1f) * 0.9f;

            // Laser tail logic

            Main.spriteBatch.Draw(laserTailTexture, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.rotation, laserTailTexture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            // Laser body logic

            laserLength -= (laserTailTexture.Height / 2 + laserHeadTexture.Height) * Projectile.scale;
            Vector2 centerDelta = Projectile.Center;
            centerDelta += Projectile.velocity * Projectile.scale * (float)laserTailTexture.Height / 2f;
            if (laserLength > 0f)
            {
                float laserLengthDelta = 0f;
                Rectangle sourceRectangle = new Rectangle(0, 16 * (Projectile.timeLeft / 3 % 5), laserBodyTexture.Width, 16);
                while (laserLengthDelta + 1f < laserLength)
                {
                    if (laserLength - laserLengthDelta < sourceRectangle.Height)
                    {
                        sourceRectangle.Height = (int)(laserLength - laserLengthDelta);
                    }
                    Main.spriteBatch.Draw(laserBodyTexture, centerDelta - Main.screenPosition, new Rectangle?(sourceRectangle), drawColor, Projectile.rotation, new Vector2(sourceRectangle.Width / 2f, 0f), Projectile.scale, SpriteEffects.None, 0);
                    laserLengthDelta += sourceRectangle.Height * Projectile.scale;
                    centerDelta += Projectile.velocity * sourceRectangle.Height * Projectile.scale;
                    sourceRectangle.Y += 16;
                    if (sourceRectangle.Y + sourceRectangle.Height > laserBodyTexture.Height)
                    {
                        sourceRectangle.Y = 0;
                    }
                }
            }

            // Laser head logic

            Main.spriteBatch.Draw(laserHeadTexture, centerDelta - Main.screenPosition, null, drawColor, Projectile.rotation, laserHeadTexture.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Projectile.localAI[1], (float)Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float value = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref value))
            {
                return true;
            }
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }
    }
}
