using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class TitanScalePotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Scale Potion");
            Tooltip.SetDefault("Increases knockback, defense by 5 and damage reduction by 5%\n" +
				"Increases defense by 25 and damage reduction by 10% for a few seconds after a true melee strike");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 34;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Yellow;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<TitanScale>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TitanPotion, 4);
            recipe.AddIngredient(ItemID.BeetleHusk);
            recipe.AddIngredient(ModContent.ItemType<CoralskinFoolfish>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.alchemy = true;
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
			// Alch table effect not on blood orb recipes
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 4);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 40);
            recipe.AddIngredient(ItemID.BeetleHusk);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
        }
    }
}
