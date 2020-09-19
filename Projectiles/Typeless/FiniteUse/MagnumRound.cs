using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless.FiniteUse
{
    public class MagnumRound : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magnum Round");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.light = 0.5f;
            projectile.alpha = 255;
            projectile.extraUpdates = 10;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 1;
            aiType = ProjectileID.BulletHighVelocity;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            // Crits are extra powerful, dealing 2.5x damage instead of 2x.
            if (crit)
            {
                damage = (int)(damage * 1.25);
                knockback *= 1.25f;
            }

            if (target.Organic())
                damage += target.lifeMax / 25; //400 + 80 = 480 + (100000 / 25 = 4000) = 4480, if crit = 5600 = 5.6% of boss HP

            // Shots are hard capped at 6.6% of the entity's max health, meaning if you shoot a non-boss, you're an idiot.
            if (damage > target.lifeMax / 15 && CalamityPlayer.areThereAnyDamnBosses)
                damage = target.lifeMax / 15;
        }
    }
}
