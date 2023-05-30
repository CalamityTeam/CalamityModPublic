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
            Item.createTile = ModContent.TileType<Tiles.Abyss.SulphurousSand>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 9999;
            Item.ammo = AmmoID.Sand;
            Item.shoot = ModContent.ProjectileType<SulphurousSandBallGun>();
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
