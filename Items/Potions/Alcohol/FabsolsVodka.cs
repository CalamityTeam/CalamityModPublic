using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class FabsolsVodka : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fabsol's Vodka");
            Tooltip.SetDefault("Boosts all damage stats by 8% but lowers defense by 10%\n" +
                               "Increases immune time after being struck\n" +
							   "This magical liquor is highly sought by those with a refined palate");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = ItemRarityID.LightRed;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<FabsolVodkaBuff>();
            item.buffTime = CalamityUtils.SecondsToFrames(900f);
            item.value = Item.buyPrice(0, 2, 60, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Ale);
            recipe.AddIngredient(ItemID.PixieDust, 10);
            recipe.AddIngredient(ItemID.CrystalShard, 5);
            recipe.AddIngredient(ItemID.UnicornHorn);
            recipe.AddTile(TileID.Kegs);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Ale);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 40);
            recipe.AddIngredient(ItemID.CrystalShard);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
