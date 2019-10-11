using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class MoonstoneCrown : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moonstone Crown");
            Tooltip.SetDefault("15% increased rogue projectile velocity\n" +
                "Stealth strikes summon lunar flares on enemy hits\n" +
                "Rogue projectiles very occasionally summon moon sigils behind them");
        }

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
            item.value = Item.buyPrice(0, 18, 0, 0);
            item.Calamity().postMoonLordRarity = 12;
            item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.throwingVelocity += 0.15f;
            modPlayer.moonCrown = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("FeatherCrown"));
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(mod.ItemType("GalacticaSingularity"), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
