using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ChaosAmulet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaos Amulet");
            Tooltip.SetDefault("Spelunker effect and increased life regen");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.lifeRegen = 2;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.findTreasure = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EssenceofChaos>(), 7);
            recipe.AddIngredient(ItemID.SpelunkerPotion, 7);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
