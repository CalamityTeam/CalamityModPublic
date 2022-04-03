using CalamityMod.CalPlayer;
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
            Item.width = 26;
            Item.height = 30;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.Calamity().devItem = true;
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
