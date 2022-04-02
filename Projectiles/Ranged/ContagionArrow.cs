using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ContagionArrow : ModProjectile
    {
        private int addBallTimer = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Contagion Arrow");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.arrow = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.ranged = true;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            addBallTimer--;
            if (addBallTimer <= 0)
            {
                if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<ContagionBall>()] < 100)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ContagionBall>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                }
                addBallTimer = 10;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.15f / 255f, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0f / 255f);
            if (projectile.ai[0] <= 60f)
            {
                projectile.ai[0] += 1f;
                return;
            }
            projectile.velocity.Y = projectile.velocity.Y + 0.075f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 240);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
