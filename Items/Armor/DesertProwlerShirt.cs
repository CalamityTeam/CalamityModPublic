using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class DesertProwlerShirt : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Prowler Shirt");
            Tooltip.SetDefault("5% increased ranged critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 5;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertFeather>(3).
                AddIngredient(ItemID.Silk, 10).
                AddTile(TileID.Loom).
                Register();
        }
    }
}
