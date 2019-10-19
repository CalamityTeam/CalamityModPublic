using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Items.Potions
{
    public class TriumphPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Triumph Potion");
            Tooltip.SetDefault("Enemy contact damage is reduced, the lower their health the more it is reduced");
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
            item.buffType = ModContent.BuffType<TriumphBuff>();
            item.buffTime = 7200;
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<StormlionMandible>());
            recipe.AddIngredient(ModContent.ItemType<VictoryShard>(), 3);
            recipe.AddTile(TileID.Bottles);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 30);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
