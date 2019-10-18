using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Popo : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Scarf and Hat");
            Tooltip.SetDefault("Transforms the holder into a snowman\n" +
                "Don't let the demons steal your nose");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 30;
            item.accessory = true;
            item.value = 1000000;
            item.rare = 5;
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
