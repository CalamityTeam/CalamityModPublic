using CalamityMod.Items.Placeables.Crags;
using CalamityMod.Tiles.Pylons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Pylons
{
    public class CragsPylon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CragsPylonTile>());

            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScorchedRemains>(15).
                AddIngredient<ScorchedBone>(10).
                AddIngredient(ItemID.Hellstone, 15).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
