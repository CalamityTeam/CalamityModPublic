using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Aftershock : ModProjectile
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
            projectile.melee = true;
            projectile.ignoreWater = true;
            aiType = 261;
        }
    }
}
