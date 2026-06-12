using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BatholithBangle : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static int cooldown = 600;
        public static int damage = 50;
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 28;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.batholithBangle = true;
            modPlayer.batholithBangleVisual = !hideVisual;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlackGlassBand>().
                AddRecipeGroup("Boss2Material", 15).
                AddIngredient(ItemID.Granite, 45).
                AddIngredient(ItemID.Amber, 3).
                AddIngredient(ItemID.FallenStar, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
