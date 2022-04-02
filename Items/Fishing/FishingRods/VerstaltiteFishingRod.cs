using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
namespace CalamityMod.Items.Fishing.FishingRods
{
    public class VerstaltiteFishingRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Verstaltite Fishing Rod");
            Tooltip.SetDefault("Increased fishing power when in the tundra\n" +
                "The ancient alloy's prismatic qualities are perfect for attracting fish");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.useAnimation = 8;
            item.useTime = 8;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.fishingPole = 35;
            item.shootSpeed = 15f;
            item.shoot = ModContent.ProjectileType<VerstaltiteBobber>();
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
