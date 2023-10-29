using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.NPCs.BrimstoneElemental;
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
    public class BrimstoneRay : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 10;
            Projectile.height = 10;
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
            Vector2? vector78 = null;

            if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero)
                Projectile.velocity = -Vector2.UnitY;

            if (Main.npc[(int)Projectile.ai[1]].active && Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<BrimstoneElemental>())
            {
                if (Main.npc[(int)Projectile.ai[1]].ai[0] == 5)
                {
                    Vector2 fireFrom = new Vector2(Main.npc[(int)Projectile.ai[1]].Center.X + (Main.npc[(int)Projectile.ai[1]].spriteDirection > 0 ? 34f : -34f), Main.npc[(int)Projectile.ai[1]].Center.Y - 74f);
                    Projectile.position = fireFrom - new Vector2(Projectile.width, Projectile.height) / 2f;
                }
            }
            else
                Projectile.Kill();

            float projScale = 1f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 45f)
            {
                Projectile.Kill();
                return;
            }

            Projectile.scale = (float)Math.Sin(Projectile.localAI[0] * (float)Math.PI / 45f) * 10f * projScale;
            if (Projectile.scale > projScale)
                Projectile.scale = projScale;

            float projVelRotation = Projectile.velocity.ToRotation();

            // Rotating beam for test AI
            if (Projectile.ai[0] > 0f)
                projVelRotation += Main.npc[(int)Projectile.ai[1]].spriteDirection * MathHelper.TwoPi / 360f;

            Projectile.rotation = projVelRotation - MathHelper.PiOver2;
            Projectile.velocity = projVelRotation.ToRotationVector2();

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

            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], rayLength, 0.5f); //length of laser, linear interpolation
            Vector2 dustSpawnPos = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);

            // Fire brimstone darts along the laser
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            if (Main.npc[(int)Projectile.ai[1]].ai[1] == 210f && Projectile.owner == Main.myPlayer && Main.npc[(int)Projectile.ai[1]].ai[0] == 5)
            {
                Vector2 velocity = Projectile.velocity;
                velocity.Normalize();
                float distanceBetweenProjectiles = Main.zenithWorld ? 360 : bossRush ? 72f : 144f;
                Vector2 fireFrom = new Vector2(Main.npc[(int)Projectile.ai[1]].Center.X + (Main.npc[(int)Projectile.ai[1]].spriteDirection > 0 ? 34f : -34f), Main.npc[(int)Projectile.ai[1]].Center.Y - 74f) + velocity * distanceBetweenProjectiles;
                int projectileAmt = (int)(Projectile.localAI[1] / distanceBetweenProjectiles);
                int type = ModContent.ProjectileType<BrimstoneBarrage>();
                int damage = Projectile.GetProjectileDamage(ModContent.NPCType<BrimstoneElemental>());
                for (int i = 0; i < projectileAmt; i++)
                {
                    int totalProjectiles = 2;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    for (int j = 0; j < totalProjectiles; j++)
                    {
                        Vector2 projVelocity = Projectile.velocity.RotatedBy(radians * j + MathHelper.PiOver2);
                        int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), fireFrom, projVelocity, type, damage, 0f, Main.myPlayer, death ? 2f : 1f, 0f);
                        Main.projectile[proj].tileCollide = true;
                        if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                            Main.projectile[proj].extraUpdates += 1;
                    }
                    fireFrom += velocity * distanceBetweenProjectiles;
                }
            }

            for (int j = 0; j < 2; j++)
            {
                float dustRotation = Projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * MathHelper.PiOver2;
                float randomFloatOffset = (float)Main.rand.NextDouble() * 2f + 2f;
                Vector2 dustDirection = new Vector2((float)Math.Cos(dustRotation) * randomFloatOffset, (float)Math.Sin(dustRotation) * randomFloatOffset);
                int brimDust = Dust.NewDust(dustSpawnPos, 0, 0, (int)CalamityDusts.Brimstone, dustDirection.X, dustDirection.Y, 0, default, 1f);
                Main.dust[brimDust].noGravity = true;
                Main.dust[brimDust].scale = 1.7f;
            }

            if (Main.rand.NextBool(5))
            {
                Vector2 extraDustSpawn = Projectile.velocity.RotatedBy(MathHelper.PiOver2, default) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width;
                int extraBrimDust = Dust.NewDust(dustSpawnPos + extraDustSpawn - Vector2.One * 4f, 8, 8, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1.5f);
                Dust dust = Main.dust[extraBrimDust];
                dust.velocity *= 0.5f;
                Main.dust[extraBrimDust].velocity.Y = -Math.Abs(Main.dust[extraBrimDust].velocity.Y);
            }

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
                Rectangle drawRectangle = new Rectangle(0, 0, texture2D20.Width, texture2D20.Height);
                while (raySegment + 1f < rayDrawLength)
                {
                    if (rayDrawLength - raySegment < drawRectangle.Height)
                    {
                        drawRectangle.Height = (int)(rayDrawLength - raySegment);
                    }
                    Main.EntitySpriteDraw(texture2D20, projCenter - Main.screenPosition, new Rectangle?(drawRectangle), baseColor, Projectile.rotation, new Vector2(drawRectangle.Width / 2, 0f), Projectile.scale, SpriteEffects.None, 0);
                    raySegment += drawRectangle.Height * Projectile.scale;
                    projCenter += Projectile.velocity * drawRectangle.Height * Projectile.scale;
                    drawRectangle.Y += texture2D20.Height;
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

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 150);
        }

        public override bool CanHitPlayer(Player target) => Projectile.scale >= 0.5f;
    }
}
