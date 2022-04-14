using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories.Vanity
{
    public class Popo : ModItem
    {
        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Head, "CalamityMod/Items/Accessories/Vanity/Popo_Head");
                //Mod.AddEquipTexture(new EquipTexture(), "PopoNoseless", EquipType.Head, "CalamityMod/Items/Accessories/Vanity/PopoNoseless_Head");
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Body, "CalamityMod/Items/Accessories/Vanity/Popo_Body");
                Mod.AddEquipTexture(new EquipTexture(), this, EquipType.Legs, "CalamityMod/Items/Accessories/Vanity/Popo_Legs");
            }
        }

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
}
