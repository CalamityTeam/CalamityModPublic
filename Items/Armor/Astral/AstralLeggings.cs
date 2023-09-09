using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Astral
{
    [AutoloadEquip(EquipType.Legs)]
    public class AstralLeggings : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.Hardmode";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.defense = 21;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.1f;
            player.findTreasure = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstralBar>(10).
                AddIngredient(ItemID.MeteoriteBar, 8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
