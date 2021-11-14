using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class GreatbayPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greatbay Pickaxe");
            Tooltip.SetDefault("Can mine Demonite, Crimtane and Meteorite");
        }

        public override void SetDefaults()
        {
            item.damage = 9;
            item.knockBack = 2f;
            item.useTime = 8;
            item.useAnimation = 16;
            item.pick = 55;

            item.melee = true;
            item.width = 44;
            item.height = 44;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VictideBar>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
