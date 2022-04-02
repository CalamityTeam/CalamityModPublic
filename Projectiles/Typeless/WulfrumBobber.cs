using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using CalamityMod.Items.Fishing.FishingRods;
namespace CalamityMod.Projectiles.Typeless
{
	public class WulfrumBobber : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wulfrum Bobber");
		}

		public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.height = 14;
			projectile.aiStyle = 61;
			projectile.bobber = true;
			projectile.penetrate = -1;
		}

		public override bool PreDrawExtras(SpriteBatch spriteBatch)
		{
			return projectile.DrawFishingLine(ModContent.ItemType<WulfrumRod>(), new Color(200, 200, 200, 100), 38, 28f);
		}
	}
}
