using CalamityMod.Projectiles.BaseProjectiles;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Projectiles.Melee
{
	public class MourningstarFlail : BaseWhipProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mourningstar");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Daybreak, 300);
            // Note: This is being left as the solar explosion projectile for now, since the weapon is in a good balancing position right now.
            // If it is to be rebalanced, consider replacing it with the FuckYou projectile (which does not mess with I-frames)
            if (projectile.localAI[1] <= 0f && projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, ProjectileID.SolarWhipSwordExplosion, projectile.damage, knockback, projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
            }
            projectile.localAI[1] = 4f;
        }
    }
}
