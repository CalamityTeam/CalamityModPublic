using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class RevivifyPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Revivify Potion");
            Tooltip.SetDefault("Causes enemy attacks to heal you for a fraction of their damage");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 36;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
            item.consumable = true;
			item.buffType = ModContent.BuffType<Revivify>();
			item.buffTime = CalamityUtils.SecondsToFrames(180f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HolyWater, 5);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 20);
            recipe.AddIngredient(ItemID.CrystalShard, 5);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 3);
            recipe.AddIngredient(ModContent.ItemType<ScarredAngelfish>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.alchemy = true;
            recipe.SetResult(this, 5);
            recipe.AddRecipe();
			// Blood orb recipes no alch table effect
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HolyWater, 5);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 100);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this, 5);
            recipe.AddRecipe();
        }
    }
}
