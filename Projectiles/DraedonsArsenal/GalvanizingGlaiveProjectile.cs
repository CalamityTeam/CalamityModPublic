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
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 90;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.alpha = 255;
        }

        public override SpearType SpearAiType => SpearType.GhastlyGlaiveSpear;
        public override float TravelSpeed => 8f;
        public override Action<Projectile> EffectBeforeReelback => proj =>
        {
            Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitY) * Projectile.width;
            Projectile.NewProjectile(Projectile.Center + offset, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 15f, ModContent.ProjectileType<GaussEnergy>(), (int)(Projectile.damage * 0.4), 0f, Projectile.owner);
        };

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Texture2D texture = ModContent.Request<Texture2D>(Texture);
            Vector2 origin = texture.Size() * 0.5f;
            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, Projectile.rotation, origin, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float angle = Projectile.rotation - MathHelper.PiOver4 * (Projectile.spriteDirection == -1).ToInt();
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + angle.ToRotationVector2() * -105f, Projectile.width, ref _);
        }

        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            float animationRatio = player.itemAnimation / (float)player.itemAnimationMax;
            DelegateMethods.f_1 = Utils.InverseLerp(0f, 0.4f, animationRatio, true) * Utils.InverseLerp(1f, 0.6f, animationRatio, true);
            DelegateMethods.c_1 = Color.White;
            List<Vector2> oldPositions = new List<Vector2>()
            {
                Projectile.position
            };
            foreach (var oldPosition in Projectile.oldPos)
            {
                if (oldPosition != Vector2.Zero)
                    oldPositions.Add(oldPosition);
            }
            for (int i = 0; i < oldPositions.Count - 2; i += 2)
            {
                Vector2 offset = Projectile.Size * 0.5f + Projectile.Size.RotatedBy(Projectile.velocity.ToRotation() - MathHelper.PiOver4) * 0.4f;
                Vector2 start = oldPositions[i] + offset - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Vector2 end = oldPositions[i + 2] + offset - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                Utils.DrawLaser(spriteBatch, ModContent.Request<Texture2D>("CalamityMod/Projectiles/LightningProj"), start, end, new Vector2(0.2f), new Utils.LaserLineFraming(DelegateMethods.LightningLaserDraw));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(12))
                Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<GaussFlux>(), damage, 0f, Projectile.owner, 0f, target.whoAmI);
        }
    }
}
