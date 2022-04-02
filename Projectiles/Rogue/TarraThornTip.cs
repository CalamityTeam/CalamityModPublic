using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class TarraThornTip : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Magic/NettleTip";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.aiStyle = 4;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            aiType = ProjectileID.NettleBurstEnd;
        }
    }
}
