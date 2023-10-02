using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class BansheeHookProj : BaseSpearProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<BansheeHook>();
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 90;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.alpha = 255;
        }

        public override SpearType SpearAiType => SpearType.GhastlyGlaiveSpear;
        public override float TravelSpeed => 22f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), 
                                     Projectile.Center + Projectile.velocity * 0.5f,
                                     Projectile.velocity * 0.8f, ModContent.ProjectileType<BansheeHookScythe>(),
                                     Projectile.damage, Projectile.knockBack * 0.85f, Projectile.owner, 0f, 0f);
        };
        public override void ExtraBehavior()
        {
            Player player = Main.player[Projectile.owner];

            Vector2 playerRelativePoint = player.RotatedRelativePoint(player.MountedCenter);

            float itemAnimationCompletion = player.itemAnimation / (float)player.itemAnimationMax;
            float completionAsAngle = (1f - itemAnimationCompletion) * MathHelper.TwoPi;
            float startingVelocityRotation = Projectile.velocity.ToRotation();
            float startingVelocitySpeed = Projectile.velocity.Length();

            // The motion moves in an imaginary circle, but the cane does not because it relies on
            // its ai[0] X multiplier, giving it the "swiping" motion.
            Vector2 flatVelocity = Vector2.UnitX.RotatedBy(MathHelper.Pi + completionAsAngle) *
                new Vector2(startingVelocitySpeed, Projectile.ai[0]);

            Vector2 destination = playerRelativePoint + flatVelocity.RotatedBy(startingVelocityRotation) +
                new Vector2(startingVelocitySpeed + TravelSpeed + 40f, 0f).RotatedBy(startingVelocityRotation);

            Vector2 directionTowardsEnd = player.SafeDirectionTo(destination, Vector2.UnitX * player.direction);
            Vector2 initalVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);

            float dustCount = 2f;
            int i = 0;
            while (i < dustCount)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 14, 14, 60, 0f, 0f, 110, default, 1f);
                dust.velocity = player.SafeDirectionTo(dust.position) * 2f;
                dust.position = Projectile.Center +
                    initalVelocity.RotatedBy(completionAsAngle * 2f + i / dustCount * MathHelper.TwoPi) * 10f;
                dust.scale = 1f + Main.rand.NextFloat(0.6f);
                dust.velocity += initalVelocity * 3f;
                dust.noGravity = true;
                i++;
            }
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 20, 20, 60, 0f, 0f, 110, default, 1f);
                dust.velocity = player.SafeDirectionTo(dust.position) * 2f;
                dust.position = Projectile.Center + directionTowardsEnd * -110f;
                dust.scale = 0.45f + Main.rand.NextFloat(0.4f);
                dust.fadeIn = 0.7f + Main.rand.NextFloat(0.4f);
                dust.noGravity = true;
                dust.noLight = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D alternateHookTexture = Projectile.spriteDirection == -1 ? ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Spears/BansheeHookAlt").Value : ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = new Vector2(Projectile.spriteDirection == 1 ? alternateHookTexture.Width + 8f : -8f, -8f);
            Main.EntitySpriteDraw(alternateHookTexture, drawPosition, null,
                new Color(255, 255, 255, 127), Projectile.rotation,
                origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture = Projectile.spriteDirection == -1 ? ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Spears/BansheeHookAltGlow").Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Spears/BansheeHookGlow").Value;
            Vector2 origin = new Vector2(Projectile.spriteDirection == 1 ? texture.Width - -8f : -8f, -8f); //-8 -8
            Main.EntitySpriteDraw(texture, drawPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float angle = Projectile.rotation - MathHelper.PiOver4 * Math.Sign(Projectile.velocity.X) +
                (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;

            float areaCheck = -95f;
            float reduntantVariable = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),
                targetHitbox.Size(), Projectile.Center,
                Projectile.Center + angle.ToRotationVector2() * areaCheck,
                (TravelSpeed + 1f) * Projectile.scale, ref reduntantVariable))
                return true;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), 
                    target.Center, Vector2.Zero,
                    ModContent.ProjectileType<BansheeHookBoom>(), (int)(hit.Damage * 0.25),
                    10f, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            }
        }
    }
}
