using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class CadencePotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cadance Potion");
            Tooltip.SetDefault("Gives the cadance buff which reduces spawn rates\n" +
                               "Increases life regen and increases max life by 25%\n" +
                               "Increases heart pickup range\n" +
                                "While this potion's buff is active, Regeneration Potion and Lifeforce Potion buffs are disabled");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 38;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.Orange;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.rare = ItemRarityID.LightRed;
            item.consumable = true;
            item.buffType = ModContent.BuffType<Cadence>();
            item.buffTime = CalamityUtils.SecondsToFrames(480f);
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LovePotion);
            recipe.AddIngredient(ItemID.HeartreachPotion);
            recipe.AddIngredient(ItemID.LifeforcePotion);
            recipe.AddIngredient(ItemID.RegenerationPotion);
            recipe.AddIngredient(ItemID.CalmingPotion);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 40);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
