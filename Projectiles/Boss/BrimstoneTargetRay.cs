using CalamityMod.NPCs.BrimstoneElemental;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneTargetRay : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Target Ray");
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

            float num805 = 3f; //3f
            float num806 = Projectile.width;

            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }

            float[] array3 = new float[(int)num805];
            Collision.LaserScan(samplingPoint, Projectile.velocity, num806 * Projectile.scale, 2400f, array3);
            float num807 = 0f;
            for (int num808 = 0; num808 < array3.Length; num808++)
            {
                num807 += array3[num808];
            }
            num807 /= num805;

            // Fire laser through walls at max length if target cannot be seen
            if (!Collision.CanHitLine(Main.npc[(int)Projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)Projectile.ai[1]].target].Center, 1, 1))
            {
                num807 = 2400f;
            }

            float amount = 0.5f; //0.5f
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num807, amount); //length of laser, linear interpolation

            DelegateMethods.v3_1 = new Vector3(0.9f, 0.3f, 0.3f);
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture2D19 = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D texture2D20 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BrimstoneRayMid");
            Texture2D texture2D21 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BrimstoneRayEnd");
            float num223 = Projectile.localAI[1]; //length of laser
            Color color44 = new Color(255, 255, 255, 0) * 0.9f;
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            spriteBatch.Draw(texture2D19, vector, sourceRectangle2, color44, Projectile.rotation, texture2D19.Size() / 2f, Projectile.scale, SpriteEffects.None, 0f);
            num223 -= (texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
            Vector2 value20 = Projectile.Center;
            value20 += Projectile.velocity * Projectile.scale * texture2D19.Height / 2f;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = new Rectangle(0, 16 * (Projectile.timeLeft / 3 % 5), texture2D20.Width, 16);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }
                    spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, new Rectangle?(rectangle7), color44, Projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0f);
                    num224 += rectangle7.Height * Projectile.scale;
                    value20 += Projectile.velocity * rectangle7.Height * Projectile.scale;
                    rectangle7.Y += 16;
                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            Vector2 vector2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;
            spriteBatch.Draw(texture2D21, vector2, sourceRectangle2, color44, Projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit * Projectile.localAI[1], Projectile.width * Projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
