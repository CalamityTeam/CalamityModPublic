using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class FellerofEvergreens : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Feller of Evergreens");
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.knockBack = 5f;
            item.useTime = 17;
            item.useAnimation = 25;
            item.axe = 100 / 5;

            item.melee = true;
            item.width = 36;
            item.height = 36;
            item.scale = 1.5f;
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
            recipe.AddIngredient(ItemID.TungstenAxe);
            recipe.AddIngredient(ItemID.TungstenBar, 10);
            recipe.AddIngredient(ItemID.Wood, 15);
            recipe.anyWood = true;
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SilverAxe);
            recipe.AddIngredient(ItemID.SilverBar, 10);
            recipe.AddIngredient(ItemID.Wood, 15);
            recipe.anyWood = true;
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
