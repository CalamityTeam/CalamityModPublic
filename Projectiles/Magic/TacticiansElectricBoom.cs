using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class TacticiansElectricBoom : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private const float radius = 50f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Explosion");
        }

        public override void SetDefaults()
        {
            projectile.width = 100;
            projectile.height = 100;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
            projectile.magic = true;
        }

        public override void AI()
        {
            if (projectile.timeLeft >= 8)
            {
                for (int i = 0; i < 30; i++)
                {
                    int dustType = Main.rand.NextBool() ? 132 : 264;

                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                    dustVelocity.Normalize();
                    dustVelocity *= 6;

                    int dust = Dust.NewDust(projectile.Center, 1, 1, dustType, dustVelocity.X, dustVelocity.Y, 0, default, 0.75f);
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.Electrified, 180);

        public override void OnHitPvp(Player target, int damage, bool crit) => target.AddBuff(BuffID.Electrified, 180);

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(projectile.Center, radius, targetHitbox);
    }
}
