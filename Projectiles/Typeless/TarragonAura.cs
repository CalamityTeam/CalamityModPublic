using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class TarragonAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 2;
        }

        // If the AI parameter isn't a valid NPC slot, it can hit anything. Otherwise it can only hit one NPC.
        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] < 0f || Projectile.ai[0] > 199f || Projectile.ai[0] == target.whoAmI)
                return null;
            return (bool?)false;
        }
    }
}
