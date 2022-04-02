using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class BansheeHookProj : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Banshee Hook");
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.melee = true;
            projectile.timeLeft = 90;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.hide = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
            projectile.alpha = 255;
        }

        public override SpearType SpearAiType => SpearType.GhastlyGlaiveSpear;
        public override float TravelSpeed => 22f;
        public override Action<Projectile> EffectBeforeReelback => (proj) =>
        {
            Projectile.NewProjectile(projectile.Center + projectile.velocity * 0.5f,
                                     projectile.velocity * 0.8f, ModContent.ProjectileType<BansheeHookScythe>(),
                                     projectile.damage, projectile.knockBack * 0.85f, projectile.owner, 0f, 0f);
        };
        public override void ExtraBehavior()
        {
            Player player = Main.player[projectile.owner];

            Vector2 playerRelativePoint = player.RotatedRelativePoint(player.MountedCenter);

            float itemAnimationCompletion = player.itemAnimation / (float)player.itemAnimationMax;
            float completionAsAngle = (1f - itemAnimationCompletion) * MathHelper.TwoPi;
            float startingVelocityRotation = projectile.velocity.ToRotation();
            float startingVelocitySpeed = projectile.velocity.Length();

            // The motion moves in an imaginary circle, but the cane does not because it relies on
            // its ai[0] X multiplier, giving it the "swiping" motion.
            Vector2 flatVelocity = Vector2.UnitX.RotatedBy(MathHelper.Pi + completionAsAngle) *
                new Vector2(startingVelocitySpeed, projectile.ai[0]);

            Vector2 destination = playerRelativePoint + flatVelocity.RotatedBy(startingVelocityRotation) +
                new Vector2(startingVelocitySpeed + TravelSpeed + 40f, 0f).RotatedBy(startingVelocityRotation);

            Vector2 directionTowardsEnd = player.SafeDirectionTo(destination, Vector2.UnitX * player.direction);
            Vector2 initalVelocity = projectile.velocity.SafeNormalize(Vector2.UnitY);

            float dustCount = 2f;
            int i = 0;
            while (i < dustCount)
            {
                Dust dust = Dust.NewDustDirect(projectile.Center, 14, 14, 60, 0f, 0f, 110, default, 1f);
                dust.velocity = player.SafeDirectionTo(dust.position) * 2f;
                dust.position = projectile.Center +
                    initalVelocity.RotatedBy(completionAsAngle * 2f + i / dustCount * MathHelper.TwoPi) * 10f;
                dust.scale = 1f + Main.rand.NextFloat(0.6f);
                dust.velocity += initalVelocity * 3f;
                dust.noGravity = true;
                i++;
            }
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(projectile.Center, 20, 20, 60, 0f, 0f, 110, default, 1f);
                dust.velocity = player.SafeDirectionTo(dust.position) * 2f;
                dust.position = projectile.Center + directionTowardsEnd * -110f;
                dust.scale = 0.45f + Main.rand.NextFloat(0.4f);
                dust.fadeIn = 0.7f + Main.rand.NextFloat(0.4f);
                dust.noGravity = true;
                dust.noLight = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawPosition = projectile.position + new Vector2(projectile.width, projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D alternateHookTexture = projectile.spriteDirection == -1 ? ModContent.GetTexture("CalamityMod/Projectiles/Melee/Spears/BansheeHookAlt") : Main.projectileTexture[projectile.type];
            Vector2 origin = new Vector2(projectile.spriteDirection == 1 ? alternateHookTexture.Width + 8f : -8f, -8f);
            spriteBatch.Draw(alternateHookTexture, drawPosition, null,
                new Color(255, 255, 255, 127), projectile.rotation,
                origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawPosition = projectile.position + new Vector2(projectile.width, projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture = projectile.spriteDirection == -1 ? ModContent.GetTexture("CalamityMod/Projectiles/Melee/Spears/BansheeHookAltGlow") : ModContent.GetTexture("CalamityMod/Projectiles/Melee/Spears/BansheeHookGlow");
            Vector2 origin = new Vector2(projectile.spriteDirection == 1 ? texture.Width - -8f : -8f, -8f); //-8 -8
            spriteBatch.Draw(texture, drawPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float angle = projectile.rotation - MathHelper.PiOver4 * Math.Sign(projectile.velocity.X) +
                (projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;

            float areaCheck = -95f;
            float reduntantVariable = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),
                targetHitbox.Size(), projectile.Center,
                projectile.Center + angle.ToRotationVector2() * areaCheck,
                (TravelSpeed + 1f) * projectile.scale, ref reduntantVariable))
                return true;
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(target.Center, Vector2.Zero,
                    ModContent.ProjectileType<BansheeHookBoom>(), (int)(damage * 0.25),
                    10f, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            }
        }
    }
}
