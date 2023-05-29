using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Body)]
    [LegacyName("ApronOfAffection")]
    public class AcesApronOfAffection : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;

            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Blue;
            Item.Calamity().donorItem = true;

            Item.vanity = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Robe).
                AddIngredient(ItemID.LovePotion, 10).
                AddIngredient(ItemID.LifeCrystal).
                AddTile(TileID.Loom).
                Register();
        }
    }
}
