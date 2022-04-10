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
            Item.width = 20;
            Item.height = 20;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.Yellow;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item122;
            Item.consumable = true;  // Not researchable, only drops one time.
        }

        public override bool CanUseItem(Player player) => !player.Calamity().rageBoostTwo;

        public override bool? UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = Item.useTime;
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.rageBoostTwo = true;
            }
            return true;
        }
    }
}
