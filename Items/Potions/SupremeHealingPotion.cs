using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class SupremeHealingPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supreme Healing Potion");
            SacrificeTotal = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.healLife = 250;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.potion = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(0, 6, 50, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).
                AddIngredient(ItemID.SuperHealingPotion, 4).
                AddIngredient<Bloodstone>(3).
                AddTile(TileID.Bottles).
                Register();
        }
    }
}
