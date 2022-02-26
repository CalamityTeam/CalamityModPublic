using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class HolyWrathPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Holy Wrath Potion");
            Tooltip.SetDefault("Increases damage by 12% and your attacks inflict holy fire\n" +
                "While this potion's buff is active the Wrath Potion's buff is disabled");
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.height = 36;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Purple;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<HolyWrathBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(300f);
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WrathPotion);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.alchemy = true;
            recipe.SetResult(this);
            recipe.AddRecipe();
			// Blood orb recipes don't get the alchemy table effect
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 40);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
