using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class SupremeManaPotion : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 9999;
            Item.healMana = 400;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(0, 6, 50, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(15).
                AddIngredient(ItemID.SuperManaPotion, 15).
                AddIngredient<Polterplasm>().
                AddTile(TileID.Bottles).
                Register()
                .DisableDecraft();
        }
    }
}
