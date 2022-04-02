using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class MechanicalBarracuda : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Barracuda");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.aiStyle = 39;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.ranged = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 5;
        }
    }
}
