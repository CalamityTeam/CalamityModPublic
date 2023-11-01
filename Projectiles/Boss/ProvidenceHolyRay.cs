using System;
using System.IO;
using CalamityMod.NPCs;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class ProvidenceHolyRay : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            CooldownSlot = ImmunityCooldownID.Bosses;
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
            // Day mode by default but syncs with the boss
            if (CalamityGlobalNPC.holyBoss != -1)
            {
                if (Main.npc[CalamityGlobalNPC.holyBoss].active)
                    Projectile.maxPenetrate = (int)Main.npc[CalamityGlobalNPC.holyBoss].localAI[1];
            }
            else
                Projectile.maxPenetrate = (int)Providence.BossMode.Day;
                
            bool scissorLasers = CalamityWorld.revenge || (Projectile.maxPenetrate != (int)Providence.BossMode.Day);
            Vector2? vector78 = null;

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            if (Main.npc[(int)Projectile.ai[1]].active && (Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<Providence>() || Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<ProfanedGuardianCommander>()))
            {
                Vector2 laserOffset = Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<ProfanedGuardianCommander>() ? new Vector2(40f * Main.npc[(int)Projectile.ai[1]].direction, 20f) : Vector2.UnitY * 32f;
                Vector2 fireFrom = new Vector2(Main.npc[(int)Projectile.ai[1]].Center.X, Main.npc[(int)Projectile.ai[1]].Center.Y) + laserOffset;
                Projectile.position = fireFrom - new Vector2(Projectile.width, Projectile.height) / 2f;
            }
            else
                Projectile.Kill();

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            float projScale = Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<ProfanedGuardianCommander>() ? 0.66f : 1f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= (scissorLasers ? 100f : 180f))
            {
                Projectile.Kill();
                return;
            }

            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * MathHelper.Pi / (scissorLasers ? 100f : 180f)) * 10f * projScale;
            if (Projectile.scale > projScale)
                Projectile.scale = projScale;

            float projVelRotation = Projectile.velocity.ToRotation();
            projVelRotation += Projectile.ai[0];
            Projectile.rotation = projVelRotation - MathHelper.PiOver2;
            Projectile.velocity = projVelRotation.ToRotationVector2();

            float projWidth = Projectile.width;

            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
                samplingPoint = vector78.Value;

            float[] array3 = new float[3];
            Collision.LaserScan(samplingPoint, Projectile.velocity, projWidth * Projectile.scale, 2400f, array3);
            float rayLength = 0f;
            for (int i = 0; i < array3.Length; i++)
            {
                rayLength += array3[i];
            }
            rayLength /= 3f;

            // Fire laser through walls at max length if target cannot be seen
            if (!Collision.CanHitLine(Main.npc[(int)Projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)Projectile.ai[1]].target].Center, 1, 1))
                rayLength = 2400f;

            int dustType = ProvUtils.GetDustID(Projectile.maxPenetrate);
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], rayLength, 0.5f); // Length of laser, linear interpolation
            Vector2 dustRotation = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int j = 0; j < 2; j++)
            {
                float randDustDirection = Projectile.velocity.ToRotation() + (Main.rand.NextBool() ? -1f : 1f) * MathHelper.PiOver2;
                float randDustVel = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 finalDustVel = new Vector2((float)Math.Cos(randDustDirection) * randDustVel, (float)Math.Sin(randDustDirection) * randDustVel);
                int holyDust = Dust.NewDust(dustRotation, 0, 0, dustType, finalDustVel.X, finalDustVel.Y, 0, default, 1f);
                Main.dust[holyDust].noGravity = true;
                Main.dust[holyDust].scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 extraDustRotate = Projectile.velocity.RotatedBy(MathHelper.PiOver2, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                int extraHolyDust = Dust.NewDust(dustRotation + extraDustRotate - Vector2.One * 4f, 8, 8, dustType, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[extraHolyDust];
                dust.velocity *= 0.5f;
                Main.dust[extraHolyDust].velocity.Y = -Math.Abs(Main.dust[extraHolyDust].velocity.Y);
            }

            DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
                return false;

            bool dayTime = Projectile.maxPenetrate == (int)Providence.BossMode.Day;
            Texture2D texture2D19 = dayTime ? ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value : 
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/ProvidenceHolyRayNight", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D20 = dayTime ? ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayMid", AssetRequestMode.ImmediateLoad).Value : 
                ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayMidNight", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D21 = dayTime ? ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayEnd", AssetRequestMode.ImmediateLoad).Value : 
                ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayEndNight", AssetRequestMode.ImmediateLoad).Value;

            float rayDrawLength = Projectile.localAI[1]; //length of laser
            Color baseColor = ProvUtils.GetProjectileColor(Projectile.maxPenetrate, 0) * 0.9f;
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            Main.spriteBatch.Draw(texture2D19, vector, sourceRectangle2, baseColor, Projectile.rotation, texture2D19.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            rayDrawLength -= (texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
            Vector2 projCenter = Projectile.Center;
            projCenter += Projectile.velocity * Projectile.scale * texture2D19.Height / 2f;

            if (rayDrawLength > 0f)
            {
                float raySegment = 0f;
                Rectangle drawRectangle = new Rectangle(0, 36 * (Projectile.timeLeft / 3 % 4), texture2D20.Width, 36);
                while (raySegment + 1f < rayDrawLength)
                {
                    if (rayDrawLength - raySegment < drawRectangle.Height)
                        drawRectangle.Height = (int)(rayDrawLength - raySegment);

                    Main.spriteBatch.Draw(texture2D20, projCenter - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(drawRectangle), baseColor, Projectile.rotation, new Vector2(drawRectangle.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                    raySegment += drawRectangle.Height * Projectile.scale;
                    projCenter += Projectile.velocity * drawRectangle.Height * Projectile.scale;
                    drawRectangle.Y += 36;

                    if (drawRectangle.Y + drawRectangle.Height > texture2D20.Height)
                        drawRectangle.Y = 0;
                }
            }

            Vector2 vector2 = projCenter - Main.screenPosition;
            sourceRectangle2 = null;

            Main.spriteBatch.Draw(texture2D21, vector2, sourceRectangle2, baseColor, Projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0);

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
                return true;
    
            float useless = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref useless))
                return true;

            return false;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            // In GFB, "real damage" is replaced with negative healing
            if (Projectile.maxPenetrate >= (int)Providence.BossMode.Red)
                modifiers.SourceDamage *= 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // If the player is dodging, don't apply debuffs
            if ((info.Damage <= 0 && Projectile.maxPenetrate < (int)Providence.BossMode.Red) || target.creativeGodMode)
                return;

            ProvUtils.ApplyHitEffects(target, Projectile.maxPenetrate, 400, 20);
        }

        public override bool CanHitPlayer(Player target) => Projectile.scale >= 0.5f;
    }
}
