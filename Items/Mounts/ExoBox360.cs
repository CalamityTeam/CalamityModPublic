using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Mounts
{
    public class ExoBox360 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("ExoBox 360");
            Tooltip.SetDefault("Creates a rideable flying chair\n" +
                "Replaced\n");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 34;
            item.useTime = item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item94;
            item.noMelee = true;
            item.mountType = ModContent.MountType<DraedonGamerChairMount>();

            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.ExoChairSpeedupHotkey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    line2.text = $"Press {hotkey} when on the chair to move much faster";
            }
        }
    }
}
