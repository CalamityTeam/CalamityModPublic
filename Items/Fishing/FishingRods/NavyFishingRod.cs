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
            item.width = 24;
            item.height = 28;
            item.useAnimation = 8;
            item.useTime = 8;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.fishingPole = 20;
            item.shootSpeed = 13f;
            item.shoot = ModContent.ProjectileType<NavyBobber>();
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Navystone>(), 8);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
