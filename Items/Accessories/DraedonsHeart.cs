using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.World;

namespace CalamityMod.Items.Accessories
{
    public class DraedonsHeart : ModItem
    {
        private const double ContactDamageReduction = 0.15D;

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Draedon's Heart");
            Tooltip.SetDefault("15% reduced contact damage from enemies\n" +
                "Reduces defense damage taken by 50%\n" + "Replaces Adrenaline with the Nanomachines meter\n" +
                "Unlike Adrenaline, you lose no Nanomachines when you take damage, but it stops generating for 4 seconds\n" +
                "With full Nanomachines, press & to heal 300 health over 2 seconds\n" +
                "While healing, wings are disabled and your mobility is crippled\n" +
                "'Nanomachines, son.'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 11));
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 68;
            Item.accessory = true;
            Item.defense = 48;
            Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.draedonsHeart = true;
            modPlayer.contactDamageReduction += ContactDamageReduction;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            // The 3rd tooltip line "Replaces Adrenaline..." is replaced on Normal or Expert
            TooltipLine nanomachineMeterLine = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");
            if (nanomachineMeterLine != null && !CalamityWorld.revenge)
                nanomachineMeterLine.Text = "Adds the Nanomachines meter";

            // The 4th tooltip line "Unlike Adrenaline..." is replaced on Normal or Expert
            TooltipLine doesntStopOnDamageLine = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip3");
            if (doesntStopOnDamageLine != null && !CalamityWorld.revenge)
                doesntStopOnDamageLine.Text = "Nanomachines generates over time when fighting bosses\nTaking damage stops its generation for 4 seconds";

            // The 5th tooltip line "With full Nanomachines" has the & replaced with the hotkey.
            string adrenKey = CalamityKeybinds.AdrenalineHotKey.TooltipHotkeyString();
            TooltipLine hotkeyLine = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip4");
            if (hotkeyLine != null)
            {
                string tooltipWithHotkey = hotkeyLine.Text.Replace("&", adrenKey);
                hotkeyLine.Text = tooltipWithHotkey;
            }
        }
    }
}
