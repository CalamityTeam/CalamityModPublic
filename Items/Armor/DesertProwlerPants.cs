using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class DesertProwlerPants : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Prowler Pants");
            Tooltip.SetDefault("10% increased movement speed and immunity to the Mighty Wind debuff");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
            item.defense = 2;
        }

        public override void UpdateEquip(Player player)
        {
            player.buffImmune[BuffID.WindPushed] = true;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DesertFeather>());
            recipe.AddIngredient(ItemID.Silk, 5);
            recipe.AddTile(TileID.Loom);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
