using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class VernalSoil : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 10;
        }

        public override void SetDefaults() => Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.VernalSoil>());

        public override void AddRecipes()
        {
            CreateRecipe(25).
                AddIngredient(ItemID.MudBlock, 25).
                AddIngredient(ItemID.JungleSpores).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
