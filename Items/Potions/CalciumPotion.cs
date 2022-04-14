using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Potions
{
    public class CalciumPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
            DisplayName.SetDefault("Calcium Potion");
            Tooltip.SetDefault("Grants immunity to fall damage");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.EatFood;
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
