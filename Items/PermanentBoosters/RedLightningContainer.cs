using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public class RedLightningContainer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Red Lightning Container");
            Tooltip.SetDefault("Permanently makes Rage Mode do 15% (50% in Death Mode) more damage\n" +
                "Revengeance drop");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 6));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = 4;
            item.UseSound = SoundID.Item122;
            item.consumable = true;
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 12;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.rageBoostThree)
            {
                return false;
            }
            return true;
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = item.useTime;
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.rageBoostThree = true;
            }
            return true;
        }
    }
}
