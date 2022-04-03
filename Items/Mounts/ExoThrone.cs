using System.Collections.Generic;
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
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string hotkey = CalamityMod.ExoChairSpeedupHotkey.TooltipHotkeyString();
            string hotkey2 = CalamityMod.ExoChairSlowdownHotkey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.Mod == "Terraria" && line2.Name == "Tooltip1")
                    line2.text = $"Hold {hotkey} while sitting in the throne to move much faster";
                if (line2.Mod == "Terraria" && line2.Name == "Tooltip2")
                    line2.text = $"And hold {hotkey2} to move much slower";
            }
        }
    }
}
