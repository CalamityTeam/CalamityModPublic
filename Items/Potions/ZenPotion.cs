using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class ZenPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Zen Potion");
            Tooltip.SetDefault("Vastly decreases enemy spawn rate");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.consumable = true;
            item.buffType = ModContent.BuffType<Zen>();
            item.buffTime = CalamityUtils.SecondsToFrames(900f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.alchemy = true;
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 2);
            recipe.AddIngredient(ModContent.ItemType<EbonianGel>(), 2);
            recipe.AddIngredient(ItemID.PinkGel);
            recipe.AddIngredient(ItemID.Daybloom);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();

            // Alternative recipe is not "alchemy" because none of the blood orb recipes are
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 20);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 2);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
