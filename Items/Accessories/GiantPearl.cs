using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class GiantPearl : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Pearl");
            Tooltip.SetDefault("You have a light aura around you\n" +
                "Enemies within the aura are slowed down\n" +
                "Does not work while a boss is alive\n" +
                "Provides a small amount of light in the abyss");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = ItemRarityID.Green;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.giantPearl = true;
            Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.45f, 0.8f, 0.8f);
        }
    }
}
