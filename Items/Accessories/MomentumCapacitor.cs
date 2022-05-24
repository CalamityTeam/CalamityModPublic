using CalamityMod.CalPlayer;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Linq;

namespace CalamityMod.Items.Accessories
{
    public class MomentumCapacitor : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Momentum Capacitor");
            Tooltip.SetDefault("TOOLTIP LINE HERE\n" +
                               "Rogue projectiles that enter the field get a constant acceleration and 15% damage boost\n" +
                               "These boosts can only happen to a projectile once\n" +
                               "There can only be one field");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.MomentumCapacitatorHotkey.TooltipHotkeyString();
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");

            if (line != null)
                line.Text = "Press " + hotkey + " to consume 30% of your maximum stealth to create an energy field at the cursor position";
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.momentumCapacitor = true;
        }
    }
}
