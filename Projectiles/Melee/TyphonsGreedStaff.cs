using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Melee
{
    public class TyphonsGreedStaff : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Typhon's Greed");
        }

        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 110;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.hide = true;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
        }

        public override void AI()
        {
            float num = 50f;
            float num2 = 2f;
            float scaleFactor = 20f;
            Player player = Main.player[Projectile.owner];
            float num3 = -0.7853982f;
            Vector2 value = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 value2 = Vector2.Zero;
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            Lighting.AddLight(player.Center, 0f, 0.2f, 1.45f);
            int num9 = Math.Sign(Projectile.velocity.X);
            Projectile.velocity = new Vector2((float)num9, 0f);
            if (Projectile.ai[0] == 0f)
            {
                Projectile.rotation = new Vector2((float)num9, -player.gravDir).ToRotation() + num3 + 3.14159274f;
                if (Projectile.velocity.X < 0f)
                {
                    Projectile.rotation -= 1.57079637f;
                }
            }
            Projectile.alpha -= 128;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            float arg_5DB_0 = Projectile.ai[0] / num;
            float num10 = 1f;
            Projectile.ai[0] += num10;
            Projectile.rotation += 6.28318548f * num2 / num * (float)num9;
            bool flag2 = Projectile.ai[0] == (float)(int)(num / 2f);
            if (Projectile.ai[0] >= num || (flag2 && !player.controlUseItem))
            {
                Projectile.Kill();
                player.reuseDelay = 2;
            }
            else if (flag2)
            {
                int expectedDirection = (player.SafeDirectionTo(Main.MouseWorld).X > 0f).ToDirectionInt();
                if (expectedDirection != Projectile.velocity.X)
                {
                    player.ChangeDir(expectedDirection);
                    Projectile.velocity = Vector2.UnitX * expectedDirection;
                    Projectile.rotation -= MathHelper.Pi;
                    Projectile.netUpdate = true;
                }
            }
            float num12 = Projectile.rotation - 0.7853982f * (float)num9;
            value2 = (num12 + ((num9 == -1) ? 3.14159274f : 0f)).ToRotationVector2() * (Projectile.ai[0] / num) * scaleFactor;
            Vector2 value3 = Projectile.Center + (num12 + ((num9 == -1) ? 3.14159274f : 0f)).ToRotationVector2() * 30f;
            Vector2 vector2 = num12.ToRotationVector2();
            Vector2 value4 = vector2.RotatedBy((double)(1.57079637f * (float)Projectile.spriteDirection), default);
            if (Main.rand.NextBool(2))
            {
                Dust dust3 = Dust.NewDustDirect(value3 - new Vector2(5f), 10, 10, 33, player.velocity.X, player.velocity.Y, 150, default, 1f);
                dust3.velocity = Projectile.SafeDirectionTo(dust3.position) * 0.1f + dust3.velocity * 0.1f;
            }
            for (int j = 0; j < 4; j++)
            {
                float scaleFactor2 = 1f;
                float scaleFactor3 = 1f;
                switch (j)
                {
                    case 1:
                        scaleFactor3 = -1f;
                        break;
                    case 2:
                        scaleFactor3 = 1.25f;
                        scaleFactor2 = 0.5f;
                        break;
                    case 3:
                        scaleFactor3 = -1.25f;
                        scaleFactor2 = 0.5f;
                        break;
                }
                if (Main.rand.Next(6) != 0)
                {
                    Dust dust4 = Dust.NewDustDirect(Projectile.position, 0, 0, 186, 0f, 0f, 100, default, 1f);
                    dust4.position = Projectile.Center + vector2 * (60f + Main.rand.NextFloat() * 20f) * scaleFactor3;
                    dust4.velocity = value4 * (4f + 4f * Main.rand.NextFloat()) * scaleFactor3 * scaleFactor2;
                    dust4.noGravity = true;
                    dust4.noLight = true;
                    dust4.scale = 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        dust4.noGravity = false;
                    }
                }
            }
            Projectile.position = value - Projectile.Size / 2f;
            Projectile.position += value2;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = MathHelper.WrapAngle(Projectile.rotation);
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] % 12f == 0f)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    var source = Projectile.GetProjectileSource_FromThis();
                    Projectile bubble = CalamityUtils.ProjectileBarrage(source, Projectile.Center, player.Center, Main.rand.NextBool(), 800f, 800f, 0f, 800f, 10f, ModContent.ProjectileType<TyphonsGreedBubble>(), (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Projectile.owner, true);
                    bubble.ai[1] = Main.rand.NextFloat() + 0.5f;
                }
            }
        }

        public override void CutTiles()
        {
            float num5 = 60f;
            float f = Projectile.rotation - 0.7853982f * (float)Math.Sign(Projectile.velocity.X);
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(Projectile.Center + f.ToRotationVector2() * -num5, Projectile.Center + f.ToRotationVector2() * num5, (float)Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float f = Projectile.rotation - 0.7853982f * (float)Math.Sign(Projectile.velocity.X);
            float num2 = 0f;
            float num3 = 110f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + f.ToRotationVector2() * -num3, Projectile.Center + f.ToRotationVector2() * num3, 23f * Projectile.scale, ref num2))
            {
                return true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
            target.immune[Projectile.owner] = 6;
        }
    }
}
