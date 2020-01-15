using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class JellyChargedBattery : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jelly-Charged Battery");
            Tooltip.SetDefault("+1 max minions and 10% minion damage\n" +
							   "Minion attacks spawn orbs of energy and inflict Electrified");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.accessory = true;
            item.rare = 2;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.Calamity().voltaicJelly = true;
			player.Calamity().jellyChargedBattery = true;
            player.maxMinions ++;
            player.minionDamage += 0.07f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WulfrumBattery>());
            recipe.AddIngredient(ModContent.ItemType<VoltaicJelly>());
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
