using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class CrumblingPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crumbling Potion");
            Tooltip.SetDefault("Increases melee and rogue critical strike chance by 5%\n" +
                "Melee and rogue attacks break enemy armor");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.LightRed;
            item.consumable = true;
            item.buffType = ModContent.BuffType<ArmorCrumbling>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 5);
            recipe.AddIngredient(ModContent.ItemType<AncientBoneDust>());
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.alchemy = true;
            recipe.SetResult(this, 5);
            recipe.AddRecipe();
            // Blood Orb recipes don't get the Alchemy Table effect
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 20);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
