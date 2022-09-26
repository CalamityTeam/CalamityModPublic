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
            Projectile.width = Projectile.height = 300;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        // localAI[0] = frame counter
        // localAI[1] = dust quantity
        public override void AI()
        {
            // Play sound on frame 1 and initialize dust quantity
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[1] = StartDustQuantity;
            }

            // Pure dust projectile
            DrawProjectile();

            // Increment frame counter
            Projectile.localAI[0] += 1f;
        }

        private void DrawProjectile()
        {
            // Taper down the dust amount for the last bit of the projectile's life
            if (Projectile.localAI[0] >= Lifetime - 15)
                Projectile.localAI[1] -= 1f;

            int dustCount = (int)Projectile.localAI[1];
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
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                Main.dust[idx].position.X = Projectile.Center.X + Main.rand.NextFloat(-10f, 10f);
                Main.dust[idx].position.Y = Projectile.Center.Y + Main.rand.NextFloat(-10f, 10f);
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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, ExplosionRadius, targetHitbox);
    }
}
