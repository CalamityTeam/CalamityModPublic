using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Projectiles.Typeless;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
	public class SerpentsBite : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Serpent's Bite");
            Tooltip.SetDefault(@"Reach: 28.125
Launch Velocity: 18
Pull Velocity: 14");
		}

		public override void SetDefaults()
		{
			// Instead of copying these values, we can clone and modify the ones we want to copy
			item.CloneDefaults(ItemID.AmethystHook);
			item.shootSpeed = 18f; // how quickly the hook is shot.
			item.shoot = ProjectileType<SerpentsBiteHook>();
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
			item.width = 30;
			item.height = 32;
		}
	}
}
