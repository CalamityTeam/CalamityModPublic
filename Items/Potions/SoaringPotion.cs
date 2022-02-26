using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class SoaringPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soaring Potion");
            Tooltip.SetDefault("Increases flight time and horizontal flight speed by 10%\n" +
				"Restores a fraction of your wing flight time after a true melee strike\n" +
				"The amount of flight time restored scales with your melee stats and weapon swing speed");
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
            item.buffType = ModContent.BuffType<Soaring>();
            item.buffTime = CalamityUtils.SecondsToFrames(360f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.SoulofFlight);
            recipe.AddIngredient(ModContent.ItemType<SunkenSailfish>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.alchemy = true;
            recipe.SetResult(this);
            recipe.AddRecipe();
			// Blood orb recipes don't get the alchemy table effect
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 30);
            recipe.AddIngredient(ItemID.SoulofFlight);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
