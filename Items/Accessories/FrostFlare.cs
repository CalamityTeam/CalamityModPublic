using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class FrostFlare : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frost Flare");
            Tooltip.SetDefault("All melee attacks and projectiles inflict frostburn\n" +
                "Immunity to frostburn, chilled, and frozen\n" +
                "Resistant to cold attacks and +1 life regen\n" +
                "Being above 75% life grants the player 10% increased damage\n" +
                "Being below 25% life grants the player 10 defense and 15% increased max movement speed and acceleration\n" +
				"Provides heat and cold protection in Death Mode\n" +
                "Revengeance drop");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 10));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.lifeRegen = 1;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.rare = 5;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.frostFlare = true;
        }
    }
}
