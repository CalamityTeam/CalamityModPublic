using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ShockGrenadeExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const float radius = 100f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shock Grenade Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 320;
            projectile.height = 320;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft >= 8)
            {
                for (int i = 0; i < 50; i++)
                {
                    int dustType = 132;
                    if (Main.rand.NextBool())
                    {
                        dustType = 264;
                    }

                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 8;

                    int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, dustVelocity.X, dustVelocity.Y, 0, default, 0.75f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Electrified, 180);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, radius, targetHitbox);
    }
}
