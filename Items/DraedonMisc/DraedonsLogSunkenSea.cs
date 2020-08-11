using CalamityMod.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    public class DraedonsLogSunkenSea : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon's Log - Study on Sunken Aquatic Life");
            Tooltip.SetDefault("Click to view its contents");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DraedonRust;
            item.useAnimation = item.useTime = 20;
            item.useStyle = ItemUseStyleID.HoldingUp;
        }

        public override bool UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                PopupGUIManager.FlipActivityOfGUIWithType(typeof(DraedonLogSunkenSeaGUI));
            return true;
        }
    }
}
