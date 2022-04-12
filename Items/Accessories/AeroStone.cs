using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class AeroStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aero Stone");
            Tooltip.SetDefault("One of the ancient relics\n" +
                "Increases flight time, movement speed and jump speed by 10%");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 8));
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 50;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().aeroStone = true;
            player.moveSpeed += 0.1f;
            player.jumpSpeedBoost += 0.5f;
        }
    }
}
