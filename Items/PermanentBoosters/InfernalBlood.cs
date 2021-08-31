using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public class InfernalBlood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infernal Blood");
            Tooltip.SetDefault("Permanently increases the duration of Rage Mode by 1 second\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 30;
            item.rare = ItemRarityID.Yellow;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item122;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player) => !player.Calamity().rageBoostTwo;

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = item.useTime;
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.rageBoostTwo = true;
            }
            return true;
        }
    }
}
