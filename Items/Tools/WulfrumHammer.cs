using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class WulfrumHammer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Hammer");
        }

        public override void SetDefaults()
        {
            item.damage = 7;
            item.knockBack = 5.5f;
            item.useTime = 11;
            item.useAnimation = 29;
            item.hammer = 45;
            item.tileBoost += 1;

            item.melee = true;
            item.width = 28;
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
            recipe.AddIngredient(ModContent.ItemType<WulfrumShard>(), 16);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
