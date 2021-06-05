using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class AtaxiaSubligar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydrothermic Subligar");
            Tooltip.SetDefault("3% increased critical strike chance\n" +
                "15% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 18, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.meleeCrit += 3;
            player.magicCrit += 3;
            player.rangedCrit += 3;
            player.Calamity().throwingCrit += 3;
            player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CruptixBar>(), 10);
			recipe.AddIngredient(ItemID.HellstoneBar, 5);
			recipe.AddIngredient(ModContent.ItemType<CoreofChaos>(), 2);
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
