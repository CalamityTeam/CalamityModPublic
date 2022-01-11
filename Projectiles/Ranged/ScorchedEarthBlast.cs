using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ScorchedEarthBlast : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private static int Lifetime = 20;
        private static float ExplosionRadius = 150f;
        private static float StartDustQuantity = 36f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Earth Scorcher");
        }

        public override void SetDefaults()
        {
            // Width and height don't actually do anything because the explosion uses custom collision
            projectile.width = projectile.height = 300;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = Lifetime;

            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

        // localAI[0] = frame counter
        // localAI[1] = dust quantity
        public override void AI()
        {
            // Play sound on frame 1 and initialize dust quantity
            if (projectile.localAI[0] == 0f)
            {
                projectile.localAI[1] = StartDustQuantity;
            }

            // Pure dust projectile
            DrawProjectile();

            // Increment frame counter
            projectile.localAI[0] += 1f;
        }

        private void DrawProjectile()
        {
            // Taper down the dust amount for the last bit of the projectile's life
            if (projectile.localAI[0] >= Lifetime - 15)
                projectile.localAI[1] -= 1f;

            int dustCount = (int)projectile.localAI[1];
            for (int i = 0; i < dustCount; ++i)
            {
                int dustType = Main.rand.NextBool() ? 6 : 244;
                float scale = Main.rand.NextFloat(2.0f, 2.5f);
                float randX = Main.rand.NextFloat(-30f, 30f);
                float randY = Main.rand.NextFloat(-30f, 30f);
                float randVelocity = Main.rand.NextFloat(5f, 19f);
                float speed = (float)Math.Sqrt((double)(randX * randX + randY * randY));
                speed = randVelocity / speed;
                randX *= speed;
                randY *= speed;
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                Main.dust[idx].position.X = projectile.Center.X + Main.rand.NextFloat(-10f, 10f);
                Main.dust[idx].position.Y = projectile.Center.Y + Main.rand.NextFloat(-10f, 10f);
                Main.dust[idx].velocity.X = randX;
                Main.dust[idx].velocity.Y = randY;
                Main.dust[idx].scale = scale;
                Main.dust[idx].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 300);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, ExplosionRadius, targetHitbox);
    }
}
