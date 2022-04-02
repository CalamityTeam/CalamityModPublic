using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class HeartofDarkness : ModItem
    {
        // The percentage of a full Rage bar that is gained every second with Heart of Darkness equipped.
        public const float RagePerSecond = 0.01f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart of Darkness");
            Tooltip.SetDefault("You constantly gain rage over time\n" +
                "Rage does not fade away when out of combat\n" +
                "Revengeance drop");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 4));
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.heartOfDarkness = true;
        }
    }
}
