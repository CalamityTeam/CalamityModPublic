using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class AdamantiteThrowingAxeProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Adamantite Throwing Axe");
		}

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 2;
            projectile.timeLeft = 600;
            aiType = 3;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}

        public override void Kill(int timeLeft)
        {
        	if (Main.rand.Next(2) == 0)
        	{
        		Item.NewItem((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height, mod.ItemType("AdamantiteThrowingAxe"));
        	}
        }
    }
}
