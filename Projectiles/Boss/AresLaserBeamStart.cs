using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;

namespace CalamityMod.Projectiles.Boss
{
    public class AresLaserBeamStart : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        private const int maxFrames = 5;
        private int frameDrawn = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(frameDrawn);
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            frameDrawn = reader.ReadInt32();
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Vector2? vector78 = null;

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            // Kill laser to prevent bullshit hits when Ares enters passive and immune phase
            bool killLaser = false;
            if (CalamityGlobalNPC.draedonExoMechPrime != -1)
            {
                if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
                {
                    killLaser = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[1] == (float)AresBody.SecondaryPhase.PassiveAndImmune ||
                        Main.npc[CalamityGlobalNPC.draedonExoMechPrime].Calamity().newAI[0] == (float)AresBody.Phase.Deathrays;
                }
            }

            if (Main.npc[(int)Projectile.ai[1]].active && Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<AresLaserCannon>() && !killLaser)
            {
                float offset = 84f;
                float offset2 = 16f;
                Vector2 fireFrom = Main.npc[(int)Projectile.ai[1]].Calamity().newAI[3] == 0f ? new Vector2(Main.npc[(int)Projectile.ai[1]].Center.X - offset2 * Main.npc[(int)Projectile.ai[1]].direction, Main.npc[(int)Projectile.ai[1]].Center.Y + offset) : new Vector2(Main.npc[(int)Projectile.ai[1]].Center.X + offset * Main.npc[(int)Projectile.ai[1]].direction, Main.npc[(int)Projectile.ai[1]].Center.Y + offset2);
                Projectile.position = fireFrom - new Vector2(Projectile.width, Projectile.height) / 2f;
            }
            else
                Projectile.Kill();

            float projScale = 1f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 60f)
            {
                Projectile.Kill();
                return;
            }

            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * (float)Math.PI / 60f) * 10f * projScale;
            if (Projectile.scale > projScale)
                Projectile.scale = projScale;

            float projVelRotation = Projectile.velocity.ToRotation();
            Projectile.rotation = projVelRotation - MathHelper.PiOver2;
            Projectile.velocity = projVelRotation.ToRotationVector2();

            float projWidth = Projectile.width;

            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
                samplingPoint = vector78.Value;

            float[] array3 = new float[3];
            Collision.LaserScan(samplingPoint, Projectile.velocity, projWidth * Projectile.scale, 2400f, array3);
            float laserLength = 0f;
            for (int j = 0; j < array3.Length; j++)
            {
                laserLength += array3[j];
            }
            laserLength /= 3f;

            // Fire laser through walls at max length if target cannot be seen
            if (!Collision.CanHitLine(Main.npc[(int)Projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)Projectile.ai[1]].target].Center, 1, 1))
            {
                laserLength = 2400f;
            }

            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], laserLength, amount); //length of laser, linear interpolation

            // Spawn dust at the end of the beam
            int dustType = (int)CalamityDusts.Brimstone;
            Vector2 dustPos = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int i = 0; i < 2; i++)
            {
                float dustRot = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                float dustVelMult = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 dustVel = new Vector2((float)Math.Cos(dustRot) * dustVelMult, (float)Math.Sin(dustRot) * dustVelMult);
                int dust = Dust.NewDust(dustPos, 0, 0, dustType, dustVel.X, dustVel.Y, 0, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 dustRot = Projectile.velocity.RotatedBy(MathHelper.PiOver2, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                int dust = Dust.NewDust(dustPos + dustRot - Vector2.One * 4f, 8, 8, dustType, 0f, 0f, 100, default, 1.5f);
                Main.dust[dust].velocity *= 0.5f;
                Main.dust[dust].velocity.Y = -Math.Abs(Main.dust[dust].velocity.Y);
            }

            DelegateMethods.v3_1 = new Vector3(0.9f, 0.3f, 0.3f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
                return false;

            Texture2D beamStart = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D beamMiddle = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/AresLaserBeamMiddle", AssetRequestMode.ImmediateLoad).Value;
            Texture2D beamEnd = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/AresLaserBeamEnd", AssetRequestMode.ImmediateLoad).Value;

            float drawLength = Projectile.localAI[1];
            Color color = new Color(250, 250, 250, 100);

            if (Projectile.localAI[0] % 5f == 0f)
            {
                frameDrawn++;
                if (frameDrawn >= maxFrames)
                    frameDrawn = 0;
            }

            // Draw start of beam
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle = new Rectangle(0, beamStart.Height / maxFrames * frameDrawn, beamStart.Width, beamStart.Height / maxFrames);
            Main.EntitySpriteDraw(beamStart, vector, sourceRectangle, color, Projectile.rotation, new Vector2(beamStart.Width, beamStart.Height / maxFrames) / 2f, Projectile.scale, SpriteEffects.None, 0);

            // Draw middle of beam
            drawLength -= (beamStart.Height / maxFrames / 2 + beamEnd.Height / maxFrames) * Projectile.scale;
            Vector2 center = Projectile.Center;
            center += Projectile.velocity * Projectile.scale * beamStart.Height / maxFrames / 2f;
            if (drawLength > 0f)
            {
                float i = 0f;
                int middleFrameDrawn = frameDrawn;
                while (i + 1f < drawLength)
                {
                    Rectangle rectangle = new Rectangle(0, beamMiddle.Height / maxFrames * middleFrameDrawn, beamMiddle.Width, beamMiddle.Height / maxFrames);

                    if (drawLength - i < rectangle.Height)
                        rectangle.Height = (int)(drawLength - i);

                    Main.EntitySpriteDraw(beamMiddle, center - Main.screenPosition, rectangle, color, Projectile.rotation, new Vector2(rectangle.Width / 2f, 0f), Projectile.scale, SpriteEffects.None, 0);

                    middleFrameDrawn++;
                    if (middleFrameDrawn >= maxFrames)
                        middleFrameDrawn = 0;

                    i += rectangle.Height * Projectile.scale;
                    center += Projectile.velocity * rectangle.Height * Projectile.scale;

                    rectangle.Y += beamMiddle.Height / maxFrames;
                    if (rectangle.Y + rectangle.Height > beamMiddle.Height / maxFrames)
                        rectangle.Y = 0;
                }
            }

            // Draw end of beam
            Vector2 vector2 = center - Main.screenPosition;
            sourceRectangle = new Rectangle(0, beamEnd.Height / maxFrames * frameDrawn, beamEnd.Width, beamEnd.Height / maxFrames);
            Main.EntitySpriteDraw(beamEnd, vector2, sourceRectangle, color, Projectile.rotation, new Vector2(beamEnd.Width, beamEnd.Height / maxFrames) / 2f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 30f * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 150);
        }

        public override bool CanHitPlayer(Player target) => Projectile.scale >= 0.5f;
    }
}
