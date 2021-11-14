using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class WulfrumPickaxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Pickaxe");
        }

        public override void SetDefaults()
        {
            item.damage = 5;
            item.knockBack = 2f;
            item.useTime = 8;
            item.useAnimation = 16;
            item.pick = 40;
            item.tileBoost += 1;

            item.melee = true;
            item.width = 46;
            item.height = 38;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WulfrumShard>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
