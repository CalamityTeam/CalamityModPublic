using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class BoundingPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bounding Potion");
            Tooltip.SetDefault("Grants 5% increased jump speed, 25 extra blocks of fall damage resistance, and increased jump height");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<BoundingBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(300f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.Frog);
            recipe.AddIngredient(ItemID.Vine);
            recipe.AddTile(TileID.Bottles);
            recipe.SetResult(this, 3);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 20);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
