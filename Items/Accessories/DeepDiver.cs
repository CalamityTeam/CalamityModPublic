using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.CalPlayer.Dashes;

namespace CalamityMod.Items.Accessories
{
    public class DeepDiver : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Deep Diver");
            Tooltip.SetDefault("15% increased damage, movement speed and +15 defense while underwater\n" +
                                "While underwater you gain the ability to dash great distances");
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 8;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.IsUnderwater())
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.deepDiver = true;
                modPlayer.DashID = DeepDiverDash.ID;
                player.dashType = 0;
            }
        }
    }
}
