using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Melee
{
    public class AftershockRock : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rock");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 34;
            projectile.aiStyle = 14;
            projectile.friendly = true;
            projectile.penetrate = 6;
            projectile.extraUpdates = 1;
            projectile.melee = true;
            projectile.ignoreWater = true;
            aiType = ProjectileID.BoulderStaffOfEarth;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }
    }
}
