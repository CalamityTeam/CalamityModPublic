using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<PurifiedGel>(2).
                AddIngredient<EbonianGel>(2).
                AddIngredient(ItemID.PinkGel).
                AddIngredient(ItemID.Daybloom).
                AddTile(TileID.AlchemyTable).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(20).
                AddIngredient<PurifiedGel>(2).
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}
