using System.IO;
using CalamityMod.NPCs.BrimstoneElemental;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneTargetRay : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.scale = 0.1f;
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

            if (Main.npc[(int)Projectile.ai[1]].active && Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<BrimstoneElemental>())
            {
                Vector2 fireFrom = new Vector2(Main.npc[(int)Projectile.ai[1]].Center.X + (Main.npc[(int)Projectile.ai[1]].spriteDirection > 0 ? 34f : -34f), Main.npc[(int)Projectile.ai[1]].Center.Y - 74f);
                Projectile.position = fireFrom - new Vector2(Projectile.width, Projectile.height) / 2f;
            }
            else
                Projectile.Kill();

            Vector2 laserVelocity = new Vector2(Main.npc[(int)Projectile.ai[1]].Calamity().newAI[1], Main.npc[(int)Projectile.ai[1]].Calamity().newAI[2]);
            float rotationVelocity = Projectile.ai[0] == 0f ? laserVelocity.ToRotation() : Projectile.velocity.ToRotation();
            Projectile.rotation = rotationVelocity - MathHelper.PiOver2;
            Projectile.velocity = rotationVelocity.ToRotationVector2();

            if (Projectile.ai[0] == 0f)
            {
                if (Main.npc[(int)Projectile.ai[1]].ai[1] >= 150f)
                {
                    Projectile.Kill();
                    return;
                }
            }
            else
            {
                if (Main.npc[(int)Projectile.ai[1]].ai[1] >= 180f)
                {
                    Projectile.Kill();
                    return;
                }
            }

            float projWidth = Projectile.width;

            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }

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
            {
                rayLength = 2400f;
            }

            float amount = 0.5f; //0.5f
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], rayLength, amount); //length of laser, linear interpolation

            DelegateMethods.v3_1 = new Vector3(0.9f, 0.3f, 0.3f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, DelegateMethods.CastLight);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture2D19 = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texture2D20 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BrimstoneRayMid", AssetRequestMode.ImmediateLoad).Value;
            Texture2D texture2D21 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BrimstoneRayEnd", AssetRequestMode.ImmediateLoad).Value;
            float rayDrawLength = Projectile.localAI[1]; //length of laser
            Color baseColor = new Color(255, 255, 255, 0) * 0.9f;
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            Main.EntitySpriteDraw(texture2D19, vector, sourceRectangle2, baseColor, Projectile.rotation, texture2D19.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            rayDrawLength -= (texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
            Vector2 projCenter = Projectile.Center;
            projCenter += Projectile.velocity * Projectile.scale * texture2D19.Height / 2f;
            if (rayDrawLength > 0f)
            {
                float raySegment = 0f;
                Rectangle drawRectangle = new Rectangle(0, 16 * (Projectile.timeLeft / 3 % 5), texture2D20.Width, 16);
                while (raySegment + 1f < rayDrawLength)
                {
                    if (rayDrawLength - raySegment < drawRectangle.Height)
                    {
                        drawRectangle.Height = (int)(rayDrawLength - raySegment);
                    }
                    Main.EntitySpriteDraw(texture2D20, projCenter - Main.screenPosition, new Rectangle?(drawRectangle), baseColor, Projectile.rotation, new Vector2(drawRectangle.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                    raySegment += drawRectangle.Height * Projectile.scale;
                    projCenter += Projectile.velocity * drawRectangle.Height * Projectile.scale;
                    drawRectangle.Y += 16;
                    if (drawRectangle.Y + drawRectangle.Height > texture2D20.Height)
                    {
                        drawRectangle.Y = 0;
                    }
                }
            }
            Vector2 vector2 = projCenter - Main.screenPosition;
            sourceRectangle2 = null;
            Main.EntitySpriteDraw(texture2D21, vector2, sourceRectangle2, baseColor, Projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0);
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
            float useless = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref useless))
            {
                return true;
            }
            return false;
        }
    }
}
