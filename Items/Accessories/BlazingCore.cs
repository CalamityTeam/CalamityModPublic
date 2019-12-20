using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BlazingCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Core");
            Tooltip.SetDefault("The searing core of the profaned goddess\n" +
                               "10% damage reduction \n" +
                               "Being hit creates a miniature sun that lingers, dealing damage to nearby enemies\n" +
                               "The sun will slowly drag enemies into it\n" +
                               "Only one sun can be active at once");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 6));
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 46;
            item.accessory = true;
            item.expert = true;
            item.rare = 9;
            item.value = Item.buyPrice(1, 20, 0, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.blazingCore = true;
        }
    }
}
