using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Buffs;
namespace CalamityMod.Items.Potions
{
    public class TitanScalePotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Scale Potion");
            Tooltip.SetDefault("Increases knockback, defense by 5, and damage reduction by 5%");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 999;
            item.rare = 3;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<TitanScale>();
            item.buffTime = 18000;
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TitanPotion);
            recipe.AddIngredient(ItemID.BeetleHusk);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(null, "BloodOrb", 10);
            recipe.AddIngredient(ItemID.BeetleHusk);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
