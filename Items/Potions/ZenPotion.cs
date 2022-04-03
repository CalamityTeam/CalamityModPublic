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
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<Zen>();
            Item.buffTime = CalamityUtils.SecondsToFrames(900f);
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<PurifiedGel>(), 2).AddIngredient(ModContent.ItemType<EbonianGel>(), 2).AddIngredient(ItemID.PinkGel).AddIngredient(ItemID.Daybloom).AddTile(TileID.AlchemyTable).Register();
            CreateRecipe(1).AddIngredient(ItemID.BottledWater).AddIngredient(ModContent.ItemType<BloodOrb>(), 20).AddIngredient(ModContent.ItemType<PurifiedGel>(), 2).AddTile(TileID.AlchemyTable).Register();
        }
    }
}
