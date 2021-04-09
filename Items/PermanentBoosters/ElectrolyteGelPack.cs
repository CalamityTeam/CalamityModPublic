using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public class ElectrolyteGelPack : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electrolyte Gel Pack");
            Tooltip.SetDefault("Permanently increases Adrenaline Mode damage by 15% and damage reduction by 5%\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 30;
            item.rare = ItemRarityID.LightRed;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item122;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.adrenalineBoostOne)
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
                modPlayer.adrenalineBoostOne = true;
            }
            return true;
        }
    }
}
