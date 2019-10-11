using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.RareVariants
{
    public class DeepDiver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Diver");
            Tooltip.SetDefault("15% increased damage, defense, and movement speed when underwater\n" +
                                "While underwater you gain the ability to dash great distances");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = 2;
            item.defense = 2;
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 22;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.deepDiver = true;
                modPlayer.dashMod = 5;
            }
        }
    }
}
