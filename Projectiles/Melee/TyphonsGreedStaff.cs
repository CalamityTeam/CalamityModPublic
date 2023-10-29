using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TyphonsGreedStaff : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<TyphonsGreed>();
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
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override void AI()
        {
            float spinTimer = 50f;
            float rotationFactor = 2f;
            float scaleFactor = 20f;
            Player player = Main.player[Projectile.owner];
            float rotationSpeed = -0.7853982f;
            Vector2 actualPosition = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 rotationPoint = Vector2.Zero;

            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            Lighting.AddLight(player.Center, 0f, 0.2f, 1.45f);
            int rotationVel = Math.Sign(Projectile.velocity.X);
            Projectile.velocity = new Vector2((float)rotationVel, 0f);
            if (Projectile.ai[0] == 0f)
            {
                Projectile.rotation = new Vector2((float)rotationVel, -player.gravDir).ToRotation() + rotationSpeed + 3.14159274f;
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
            float arg_5DB_0 = Projectile.ai[0] / spinTimer;
            Projectile.ai[0] += 1f;
            Projectile.rotation += 6.28318548f * rotationFactor / spinTimer * (float)rotationVel;
            bool isUsing = Projectile.ai[0] == (float)(int)(spinTimer / 2f);
            if (Projectile.ai[0] >= spinTimer || (isUsing && !player.controlUseItem))
            {
                Projectile.Kill();
                player.reuseDelay = 2;
            }
            else if (isUsing)
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
            float rotateDirection = Projectile.rotation - 0.7853982f * (float)rotationVel;
            rotationPoint = (rotateDirection + ((rotationVel == -1) ? 3.14159274f : 0f)).ToRotationVector2() * (Projectile.ai[0] / spinTimer) * scaleFactor;
            Vector2 dustSpawn = Projectile.Center + (rotateDirection + ((rotationVel == -1) ? 3.14159274f : 0f)).ToRotationVector2() * 30f;
            Vector2 staffTipDirection = rotateDirection.ToRotationVector2();
            Vector2 tipDustDirection = staffTipDirection.RotatedBy((double)(1.57079637f * (float)Projectile.spriteDirection), default);
            if (Main.rand.NextBool())
            {
                Dust staffDust = Dust.NewDustDirect(dustSpawn - new Vector2(5f), 10, 10, 33, player.velocity.X, player.velocity.Y, 150, default, 1f);
                staffDust.velocity = Projectile.SafeDirectionTo(staffDust.position) * 0.1f + staffDust.velocity * 0.1f;
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
                    Dust staffTipDust = Dust.NewDustDirect(Projectile.position, 0, 0, 186, 0f, 0f, 100, default, 1f);
                    staffTipDust.position = Projectile.Center + staffTipDirection * (60f + Main.rand.NextFloat() * 20f) * scaleFactor3;
                    staffTipDust.velocity = tipDustDirection * (4f + 4f * Main.rand.NextFloat()) * scaleFactor3 * scaleFactor2;
                    staffTipDust.noGravity = true;
                    staffTipDust.noLight = true;
                    staffTipDust.scale = 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        staffTipDust.noGravity = false;
                    }
                }
            }
            Projectile.position = actualPosition - Projectile.Size / 2f;
            Projectile.position += rotationPoint;
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
                    var source = Projectile.GetSource_FromThis();
                    Projectile bubble = CalamityUtils.ProjectileBarrage(source, Projectile.Center, player.Center, Main.rand.NextBool(), 800f, 800f, 0f, 800f, 10f, ModContent.ProjectileType<TyphonsGreedBubble>(), (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Projectile.owner, true);
                    bubble.ai[1] = Main.rand.NextFloat() + 0.5f;
                }
            }
        }

        public override void CutTiles()
        {
            float cutRadius = 60f;
            float f = Projectile.rotation - 0.7853982f * (float)Math.Sign(Projectile.velocity.X);
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(Projectile.Center + f.ToRotationVector2() * -cutRadius, Projectile.Center + f.ToRotationVector2() * cutRadius, (float)Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float f = Projectile.rotation - 0.7853982f * (float)Math.Sign(Projectile.velocity.X);
            float rotationFactor = 0f;
            float collisionRadius = 110f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + f.ToRotationVector2() * -collisionRadius, Projectile.Center + f.ToRotationVector2() * collisionRadius, 23f * Projectile.scale, ref rotationFactor))
            {
                return true;
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
    }
}
