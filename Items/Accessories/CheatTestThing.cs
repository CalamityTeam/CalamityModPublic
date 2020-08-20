using CalamityMod.CalPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CheatTestThing : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("lul");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity16BuyPrice;
            item.Calamity().postMoonLordRarity = 16;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            bool canUse = (player.name == "Fabsol" || player.name == "Totalbiscuit" || player.name == "TotalBiscuit" || player.name == "Total Biscuit" || player.name == "Total biscuit") && player.townNPCs <= 1;
            if (canUse)
            {
                modPlayer.lol = true;
            }
            else if (!player.immune)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " isn't worthy."), 1000.0, 0, false);
            }
        }
    }
}
