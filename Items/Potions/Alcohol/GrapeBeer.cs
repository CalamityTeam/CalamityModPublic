using CalamityMod.Buffs.Alcohol;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions.Alcohol
{
    public class GrapeBeer : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightRed;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.healLife = 100;
            Item.healMana = 100;
            Item.consumable = true;
            Item.potion = true;
            Item.value = Item.buyPrice(0, 0, 20, 0);
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(ModContent.BuffType<GrapeBeerBuff>(), 900);
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Grapes).
                AddIngredient<StarblightSoot>(5).
                AddTile(TileID.Kegs).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(5).
                AddIngredient<StarblightSoot>().
                AddTile(TileID.AlchemyTable).
                Register()
                .DisableDecraft();
        }
    }
}
