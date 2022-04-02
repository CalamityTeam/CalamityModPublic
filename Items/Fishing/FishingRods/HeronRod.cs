using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
namespace CalamityMod.Items.Fishing.FishingRods
{
    public class HeronRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heron Rod");
            Tooltip.SetDefault("Increased fishing power in space.\n" + //John Steinbeck quote but fish instead of snake
                "A silent head and beak lanced down and plucked it out by the head,\n" +
                "and the beak swallowed the little fish while its tail waved frantically.");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.useAnimation = 8;
            item.useTime = 8;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.fishingPole = 25;
            item.shootSpeed = 14.5f;
            item.shoot = ModContent.ProjectileType<HeronBobber>();
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 7);
            recipe.AddIngredient(ItemID.SunplateBlock, 5);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
