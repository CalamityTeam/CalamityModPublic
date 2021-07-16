using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public class MushroomPlasmaRoot : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mushroom Plasma Root");
            Tooltip.SetDefault("Permanently increases the duration of Rage Mode by 1 second\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 30;
            item.rare = ItemRarityID.Green;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item122;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player) => !player.Calamity().rageBoostOne;

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = item.useTime;
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.rageBoostOne = true;
            }
            return true;
        }
    }
}
