using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ExorcismShockwave : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static float radius = 100;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exorcism Shockwave");
        }

        public override void SetDefaults()
        {
            Projectile.width = (int)radius * 2;
            Projectile.height = (int)radius * 2;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 9;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft >= 5)
            {
                Projectile.alpha = (int)((1 - Projectile.ai[0]) * 255f);

                // Blast wave should be brighter the higher the charge of the projectile when it is killed
                int numDust = (int)(40 * Projectile.ai[0]) + 10;

                for (int i = 0; i < numDust; i++)
                {
                    // Dust Type
                    int dustToUse = Main.rand.Next(0, 3);
                    int dustType = 0;
                    switch (dustToUse)
                    {
                        case 0:
                            dustType = 175;
                            break;
                        case 1:
                            dustType = 229;
                            break;
                        case 2:
                            dustType = 263;
                            break;
                    }

                    // Shockwave
                    Vector2 circleVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    circleVelocity.Normalize();
                    circleVelocity *= radius / 10f;

                    int circle = Dust.NewDust(Projectile.Center, 1, 1, dustType, circleVelocity.X, circleVelocity.Y, 0, default, 1.5f);
                    Main.dust[circle].noGravity = true;
                }

                // Cross
                Vector2 dustLeft = (new Vector2(-1, 0)).RotatedBy(Projectile.rotation);
                Vector2 dustRight = (new Vector2(1, 0)).RotatedBy(Projectile.rotation);
                Vector2 dustUp = (new Vector2(0, -1)).RotatedBy(Projectile.rotation);
                Vector2 dustDown = (new Vector2(0, 1) * 2f).RotatedBy(Projectile.rotation);

                int dustScale = 7;
                Vector2 dustPos = Projectile.Center - new Vector2((dustScale - 1) * 0.5f, (dustScale - 1) * 0.5f);

                float minSpeed = 0f;
                float maxSpeed = 5f;
                float minScale = 1.9f;
                float maxScale = 2.1f;
                int dustCount = (int)(5 * Projectile.ai[0]);

                for (int i = 0; i < dustCount; i++)
                {
                    // Dust Type
                    int dustToUse = Main.rand.Next(0, 3);
                    int dustType = 0;
                    switch (dustToUse)
                    {
                        case 0:
                            dustType = 175;
                            break;
                        case 1:
                            dustType = 229;
                            break;
                        case 2:
                            dustType = 263;
                            break;
                    }

                    int left = Dust.NewDust(dustPos, dustScale, dustScale, dustType, 0f, 0f);
                    Main.dust[left].noGravity = true;
                    Main.dust[left].velocity = dustLeft * Main.rand.NextFloat(minSpeed, maxSpeed);
                    Main.dust[left].scale = Main.rand.NextFloat(minScale, maxScale);

                    int right = Dust.NewDust(dustPos, dustScale, dustScale, dustType, 0f, 0f);
                    Main.dust[right].noGravity = true;
                    Main.dust[right].velocity = dustRight * Main.rand.NextFloat(minSpeed, maxSpeed);
                    Main.dust[right].scale = Main.rand.NextFloat(minScale, maxScale);

                    int up = Dust.NewDust(dustPos, dustScale, dustScale, dustType, 0f, 0f);
                    Main.dust[up].noGravity = true;
                    Main.dust[up].velocity = dustUp * Main.rand.NextFloat(minSpeed, maxSpeed);
                    Main.dust[up].scale = Main.rand.NextFloat(minScale, maxScale);

                    int down = Dust.NewDust(dustPos, dustScale, dustScale, dustType, 0f, 0f);
                    Main.dust[down].noGravity = true;
                    Main.dust[down].velocity = dustDown * Main.rand.NextFloat(minSpeed, maxSpeed);
                    Main.dust[down].scale = Main.rand.NextFloat(minScale, maxScale);
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            float dist1 = Vector2.Distance(Projectile.Center, target.Hitbox.TopLeft());
            float dist2 = Vector2.Distance(Projectile.Center, target.Hitbox.TopRight());
            float dist3 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomLeft());
            float dist4 = Vector2.Distance(Projectile.Center, target.Hitbox.BottomRight());

            float damageScale = dist1;
            if (dist2 < damageScale)
                damageScale = dist2;
            if (dist3 < damageScale)
                damageScale = dist3;
            if (dist4 < damageScale)
                damageScale = dist4;
            damageScale /= radius;
            damageScale = 1f - damageScale;

            damage = (int)(damage * damageScale);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, radius, targetHitbox);
    }
}
