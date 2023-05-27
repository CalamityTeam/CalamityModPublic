using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class GiantPearl : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.giantPearl = true;
            Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.45f, 0.8f, 0.8f);
        }
    }
}
