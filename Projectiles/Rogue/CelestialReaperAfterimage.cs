using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class CelestialReaperAfterimage : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/CelestialReaper";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Reaper");
        }

        public override void SetDefaults()
        {
            projectile.width = 66;
            projectile.height = 76;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 51;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }
        public override void AI()
        {
            projectile.rotation += MathHelper.ToRadians(30f); // Buzzsaw scythe.
            NPC target = projectile.position.ClosestNPCAt(640f);
            if (target != null)
            {
                projectile.velocity = (projectile.velocity * 20f + projectile.DirectionTo(target.Center) * 20f) / 21f;
            }
            projectile.alpha += 5;
        }
    }
}
