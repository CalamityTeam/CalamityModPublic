using CalamityMod.Rarities;
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class ExoThrone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Box");
            Tooltip.SetDefault("Materializes a quite cozy and extremely nimble flying Exo throne\n" +
                "Replaced\n" +
                "Also replaced\n" +
                "A comfortable gamer is a dangerous gamer");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item94;
            Item.noMelee = true;
            Item.mountType = ModContent.MountType<DraedonGamerChairMount>();

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityKeybinds.ExoChairSpeedupHotkey.TooltipHotkeyString();
            string hotkey2 = CalamityKeybinds.ExoChairSlowdownHotkey.TooltipHotkeyString();

            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip1");
            TooltipLine line2 = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip2");

            if (line != null)
                line.Text = $"Hold {hotkey} while sitting in the throne to move much faster";

            if (line2 != null)
                line2.Text = $"And hold {hotkey2} to move much slower";
        }
    }
}
