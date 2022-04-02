using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Dusts;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneRay : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Ray");
        }

        public override void SetDefaults()
        {
            projectile.Calamity().canBreakPlayerDefense = true;
            projectile.width = 10;
            projectile.height = 10;
            projectile.hostile = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 600;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            Vector2? vector78 = null;

            if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
                projectile.velocity = -Vector2.UnitY;

            if (Main.npc[(int)projectile.ai[1]].active && Main.npc[(int)projectile.ai[1]].type == ModContent.NPCType<BrimstoneElemental>())
            {
                Vector2 fireFrom = new Vector2(Main.npc[(int)projectile.ai[1]].Center.X + (Main.npc[(int)projectile.ai[1]].spriteDirection > 0 ? 34f : -34f), Main.npc[(int)projectile.ai[1]].Center.Y - 74f);
                projectile.position = fireFrom - new Vector2(projectile.width, projectile.height) / 2f;
            }
            else
                projectile.Kill();

            float num801 = 1f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] >= 45f)
            {
                projectile.Kill();
                return;
            }

            projectile.scale = (float)Math.Sin(projectile.localAI[0] * (float)Math.PI / 45f) * 10f * num801;
            if (projectile.scale > num801)
                projectile.scale = num801;

            float num804 = projectile.velocity.ToRotation();

            // Rotating beam for test AI
            if (projectile.ai[0] > 0f)
                num804 += Main.npc[(int)projectile.ai[1]].spriteDirection * MathHelper.TwoPi / 360f;

            projectile.rotation = num804 - MathHelper.PiOver2;
            projectile.velocity = num804.ToRotationVector2();

            float num805 = 3f; //3f
            float num806 = projectile.width;

            Vector2 samplingPoint = projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }

            float[] array3 = new float[(int)num805];
            Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 2400f, array3);
            float num807 = 0f;
            for (int num808 = 0; num808 < array3.Length; num808++)
            {
                num807 += array3[num808];
            }
            num807 /= num805;

            // Fire laser through walls at max length if target cannot be seen
            if (!Collision.CanHitLine(Main.npc[(int)projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)projectile.ai[1]].target].Center, 1, 1))
            {
                num807 = 2400f;
            }

            float amount = 0.5f; //0.5f
            projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num807, amount); //length of laser, linear interpolation
            Vector2 vector79 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14f);

            // Fire brimstone darts along the laser
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            if (Main.npc[(int)projectile.ai[1]].ai[1] == 210f && projectile.owner == Main.myPlayer)
            {
                Vector2 velocity = projectile.velocity;
                velocity.Normalize();
                float distanceBetweenProjectiles = malice ? 72f : 144f;
                Vector2 fireFrom = new Vector2(Main.npc[(int)projectile.ai[1]].Center.X + (Main.npc[(int)projectile.ai[1]].spriteDirection > 0 ? 34f : -34f), Main.npc[(int)projectile.ai[1]].Center.Y - 74f) + velocity * distanceBetweenProjectiles;
                int projectileAmt = (int)(projectile.localAI[1] / distanceBetweenProjectiles);
                int type = ModContent.ProjectileType<BrimstoneBarrage>();
                int damage = projectile.GetProjectileDamage(ModContent.NPCType<BrimstoneElemental>());
                for (int i = 0; i < projectileAmt; i++)
                {
                    int totalProjectiles = 2;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    for (int j = 0; j < totalProjectiles; j++)
                    {
                        Vector2 projVelocity = projectile.velocity.RotatedBy(radians * j + MathHelper.PiOver2);
                        int proj = Projectile.NewProjectile(fireFrom, projVelocity, type, damage, 0f, Main.myPlayer, death ? 2f : 1f, 0f);
                        Main.projectile[proj].tileCollide = true;
                    }
                    fireFrom += velocity * distanceBetweenProjectiles;
                }
            }

            for (int num809 = 0; num809 < 2; num809++)
            {
                float num810 = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 vector80 = new Vector2((float)Math.Cos(num810) * num811, (float)Math.Sin(num810) * num811);
                int num812 = Dust.NewDust(vector79, 0, 0, (int)CalamityDusts.Brimstone, vector80.X, vector80.Y, 0, default, 1f);
                Main.dust[num812].noGravity = true;
                Main.dust[num812].scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 value29 = projectile.velocity.RotatedBy(MathHelper.PiOver2, default) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
                int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[num813];
                dust.velocity *= 0.5f;
                Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
            }

            DelegateMethods.v3_1 = new Vector3(0.9f, 0.3f, 0.3f);
            Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            Texture2D texture2D19 = Main.projectileTexture[projectile.type];
            Texture2D texture2D20 = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/BrimstoneRayMid");
            Texture2D texture2D21 = ModContent.GetTexture("CalamityMod/ExtraTextures/Lasers/BrimstoneRayEnd");
            float num223 = projectile.localAI[1]; //length of laser
            Color color44 = new Color(255, 255, 255, 0) * 0.9f;
            Vector2 vector = projectile.Center - Main.screenPosition;
            Rectangle? sourceRectangle2 = null;
            spriteBatch.Draw(texture2D19, vector, sourceRectangle2, color44, projectile.rotation, texture2D19.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            num223 -= (texture2D19.Height / 2 + texture2D21.Height) * projectile.scale;
            Vector2 value20 = projectile.Center;
            value20 += projectile.velocity * projectile.scale * texture2D19.Height / 2f;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = new Rectangle(0, 0, texture2D20.Width, texture2D20.Height);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }
                    spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, new Rectangle?(rectangle7), color44, projectile.rotation, new Vector2(rectangle7.Width / 2, 0f), projectile.scale, SpriteEffects.None, 0f);
                    num224 += rectangle7.Height * projectile.scale;
                    value20 += projectile.velocity * rectangle7.Height * projectile.scale;
                    rectangle7.Y += texture2D20.Height;
                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            Vector2 vector2 = value20 - Main.screenPosition;
            sourceRectangle2 = null;
            spriteBatch.Draw(texture2D21, vector2, sourceRectangle2, color44, projectile.rotation, texture2D21.Frame(1, 1, 0, 0).Top(), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = projectile.velocity;
            Utils.PlotTileLine(projectile.Center, projectile.Center + unit * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CutTiles));
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], 22f * projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
        }

        public override bool CanHitPlayer(Player target) => projectile.scale >= 0.5f;

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            target.Calamity().lastProjectileHit = projectile;
        }
    }
}
