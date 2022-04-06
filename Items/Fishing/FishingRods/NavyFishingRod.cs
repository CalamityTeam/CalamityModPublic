using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Fishing.FishingRods
{
    public class NavyFishingRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Navy Fishing Rod");
            Tooltip.SetDefault("While held, slowly electrifies nearby enemies.\n" +
                "The sea is a city.\n" + //Life of Pi ref Ch.59
                "Just below are highways, boulevards, streets and roundabouts bustling with submarine traffic.");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 20;
            Item.shootSpeed = 13f;
            Item.shoot = ModContent.ProjectileType<NavyBobber>();
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaPrism>(5).
                AddIngredient<Navystone>(8).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
