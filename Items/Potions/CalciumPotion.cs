using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class CalciumPotion : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Potions";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<CalciumBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(1200f);
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient(ItemID.BottledWater, 4).
                AddIngredient<AncientBoneDust>().
                AddTile(TileID.Bottles).
                Register();

            CreateRecipe(4).
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>().
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}
