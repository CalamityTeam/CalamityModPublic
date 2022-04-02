using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class CeaselessHungerPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ceaseless Hunger Potion");
            Tooltip.SetDefault("Causes you to suck up all items in the world");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<CeaselessHunger>();
            item.buffTime = CalamityUtils.SecondsToFrames(10f);
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 4);
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>());
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.alchemy = true;
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
            // Blood orb recipes don't get the Alchemy table effect
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 4);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 20);
            recipe.AddIngredient(ModContent.ItemType<DarkPlasma>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
        }
    }
}
