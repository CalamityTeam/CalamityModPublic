using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class DirectStrike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nondescript Damaging Entity");
        }

        public override void SetDefaults()
        {
            projectile.width = 2;
            projectile.height = 2;
            projectile.penetrate = 1;
            projectile.extraUpdates = 0;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.timeLeft = 2;
        }

        // Can only strike the given NPC slot (assuming it's a valid identifier)
        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.ai[0] < 0f || projectile.ai[0] > 199f)
                return null;
            return (projectile.ai[0] == target.whoAmI) ? null : false;
        }
    }
}
