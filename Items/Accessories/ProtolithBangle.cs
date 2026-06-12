using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ProtolithBangle : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public static int cooldown = 420;
        public static int damage = 30;
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.protolithBangle = true;
            modPlayer.protolithBangleVisual = !hideVisual;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlackGlassBand>().
                AddRecipeGroup("Boss2Material", 15).
                AddIngredient(ItemID.Marble, 45).
                AddIngredient(ItemID.Ruby, 3).
                AddIngredient(ItemID.Lens, 5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
