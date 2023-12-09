using System;
using System.IO;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MiniGuardianHolyRay : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Boss/ProvidenceHolyRay";
        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3; 
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
            
            Vector2? vector78 = null;

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            Player owner = Main.player[Projectile.owner];
            
            if (owner.active && !owner.dead)
            {
                Vector2 fireFrom = new Vector2(owner.Center.X, owner.Center.Y);
                Projectile.position = fireFrom - new Vector2(Projectile.width, Projectile.height) / 2f;
                Projectile.damage = (int)Owner.GetTotalDamage<SummonDamageClass>().ApplyTo(Projectile.originalDamage);
                Projectile.damage = Owner.ApplyArmorAccDamageBonusesTo(Projectile.damage);
            }
            else
                Projectile.Kill();
            
            
            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            float num801 = 0.66f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 150f)
            {
                Projectile.Kill();
                return;
            }

            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * MathHelper.Pi / 150f) * 10f * num801;
            if (Projectile.scale > num801)
                Projectile.scale = num801;

            float num804 = Projectile.velocity.ToRotation();
            num804 += Projectile.ai[0];
            Projectile.rotation = num804 - MathHelper.PiOver2;
            Projectile.velocity = num804.ToRotationVector2();

            float num805 = 3f;
            float num806 = Projectile.width;

            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
                samplingPoint = vector78.Value;

            float[] array3 = new float[(int)num805];
            Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 2400f, array3);
            float num807 = 0f;
            for (int num808 = 0; num808 < array3.Length; num808++)
            {
                num807 += array3[num808];
            }
            num807 /= num805;

            // Fire laser through walls at max length
            num807 = 2400f;

            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            int dustType = ProvUtils.GetDustID(pscState);
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount); // Length of laser, linear interpolation
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            for (int num809 = 0; num809 < 2; num809++)
            {
                float num810 = Projectile.velocity.ToRotation() + (Main.rand.NextBool() ? -1f : 1f) * MathHelper.PiOver2;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos(num810) * num811, (float)Math.Sin(num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, dustType, vector80.X, vector80.Y, 0, default, 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = Projectile.velocity.RotatedBy(MathHelper.PiOver2, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, dustType, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }

            DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
                return false;

            bool dayTime = Main.dayTime;
            Texture2D texture2D19 = dayTime ? ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value : 
                ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/ProvidenceHolyRayNight", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D20 = dayTime ? ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayMid", AssetRequestMode.ImmediateLoad).Value : 
                ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayMidNight", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D21 = dayTime ? ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayEnd", AssetRequestMode.ImmediateLoad).Value : 
                ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayEndNight", AssetRequestMode.ImmediateLoad).Value;

            float num223 = Projectile.localAI[1]; //length of laser
            int pscState = (int)(Main.dayTime ? Providence.BossMode.Day : Providence.BossMode.Night);
            Color color44 = ProvUtils.GetProjectileColor(pscState, 0) * 0.9f;
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            Main.spriteBatch.Draw(texture2D19, vector, sourceRectangle2, color44, Projectile.rotation, texture2D19.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            num223 -= (texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
            Vector2 value20 = Projectile.Center;
            value20 += Projectile.velocity * Projectile.scale * texture2D19.Height / 2f;

            if (num223 > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = new Rectangle(0, 36 * (Projectile.timeLeft / 3 % 4), texture2D20.Width, 36);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < rectangle7.Height)
                        rectangle7.Height = (int)(num223 - num224);

                    Main.spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle7), color44, Projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                    num224 += rectangle7.Height * Projectile.scale;
                    value20 += Projectile.velocity * rectangle7.Height * Projectile.scale;
                    rectangle7.Y += 36;

                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                        rectangle7.Y = 0;
                }
            }

            Vector2 vector2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;

            Main.spriteBatch.Draw(texture2D21, vector2, sourceRectangle2, color44, Projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0);

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

            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref num6))
                return true;

            return false;
        }

        public override bool? CanDamage() => Projectile.scale >= 0.5f;
    }
}
