using CalamityMod.Projectiles.Typeless;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class SulphurousSand : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Placeables";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SulphurousShale>();
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Abyss.SulphurousSand>());
            Item.ammo = AmmoID.Sand;
            Item.notAmmo = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Walls.SulphurousSandWall>(4).
                AddTile(TileID.WorkBenches).
                Register();
        }
    }
}
