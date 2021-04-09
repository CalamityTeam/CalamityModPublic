using CalamityMod.CalPlayer;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Vanity
{
    public class Popo : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Scarf and Hat");
            Tooltip.SetDefault("Don't let the demons steal your nose\n" +
                "Transforms the holder into a snowman");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 30;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.Calamity().devItem = true;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (CalamityWorld.death)
            {
                foreach (TooltipLine line2 in list)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                    {
                        line2.text = "Transforms the holder into a snowman\n" +
                        "Provides heat and cold protection in Death Mode";
                    }
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.snowman = true;
            if (hideVisual)
            {
                modPlayer.snowmanHide = true;
            }
        }
    }

    public class PopoHead : EquipTexture
    {
        public override bool DrawHead()
        {
            return false;
        }
    }

    public class PopoNoselessHead : EquipTexture
    {
        public override bool DrawHead()
        {
            return false;
        }
    }

    public class PopoBody : EquipTexture
    {
        public override bool DrawBody()
        {
            return false;
        }
    }

    public class PopoLegs : EquipTexture
    {
        public override bool DrawLegs()
        {
            return false;
        }
    }
}
