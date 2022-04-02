using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class AeroStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aero Stone");
            Tooltip.SetDefault("One of the ancient relics\n" +
                "Increases flight time, movement speed and jump speed by 10%");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(4, 8));
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 50;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = ItemRarityID.Green;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().aeroStone = true;
            player.moveSpeed += 0.1f;
            player.jumpSpeedBoost += 0.5f;
        }
    }
}
