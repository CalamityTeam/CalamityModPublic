using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
using Terraria.ID;
using System.Collections.Generic;
using System;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class GalvanizingGlaiveProjectile : BaseSpearProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galvanizing Glaive");
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 90;
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
            projectile.hide = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.alpha = 255;
        }

        public override SpearType SpearAiType => SpearType.GhastlyGlaiveSpear;
        public override float TravelSpeed => 8f;
        public override Action<Projectile> EffectBeforeReelback => proj =>
        {
            Vector2 offset = projectile.velocity.SafeNormalize(Vector2.UnitY) * projectile.width;
            Projectile.NewProjectile(projectile.Center + offset, projectile.velocity.SafeNormalize(Vector2.UnitY) * 15f, ModContent.ProjectileType<GaussEnergy>(), (int)(projectile.damage * 0.4), 0f, projectile.owner);
        };

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawPosition = projectile.position + new Vector2(projectile.width, projectile.height) / 2f + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition;
            Texture2D texture = ModContent.GetTexture(Texture);
            Vector2 origin = texture.Size() * 0.5f;
            spriteBatch.Draw(texture, drawPosition, null, lightColor, projectile.rotation, origin, projectile.scale, projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            return false;
        }

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float angle = projectile.rotation - MathHelper.PiOver4 * (projectile.spriteDirection == -1).ToInt();
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + angle.ToRotationVector2() * -105f, projectile.width, ref _);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            float animationRatio = player.itemAnimation / (float)player.itemAnimationMax;
            DelegateMethods.f_1 = Utils.InverseLerp(0f, 0.4f, animationRatio, true) * Utils.InverseLerp(1f, 0.6f, animationRatio, true);
            DelegateMethods.c_1 = Color.White;
            List<Vector2> oldPositions = new List<Vector2>()
            {
                projectile.position
            };
            foreach (var oldPosition in projectile.oldPos)
            {
                if (oldPosition != Vector2.Zero)
                    oldPositions.Add(oldPosition);
            }
            for (int i = 0; i < oldPositions.Count - 2; i += 2)
            {
                Vector2 offset = projectile.Size * 0.5f + projectile.Size.RotatedBy(projectile.velocity.ToRotation() - MathHelper.PiOver4) * 0.4f;
                Vector2 start = oldPositions[i] + offset - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY;
                Vector2 end = oldPositions[i + 2] + offset - Main.screenPosition + Vector2.UnitY * projectile.gfxOffY;
                Utils.DrawLaser(spriteBatch, ModContent.GetTexture("CalamityMod/Projectiles/LightningProj"), start, end, new Vector2(0.2f), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(12))
                Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<GaussFlux>(), damage, 0f, projectile.owner, 0f, target.whoAmI);
        }
    }
}
