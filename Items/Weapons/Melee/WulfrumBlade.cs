using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Melee
{
    public class WulfrumBlade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Blade");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.damage = 12;
            item.melee = true;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.useTime = 20;
            item.useTurn = true;
            item.knockBack = 3.75f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 54;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.rare = 1;
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
