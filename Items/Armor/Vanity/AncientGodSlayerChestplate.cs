using CalamityMod.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Body)]
    public class AncientGodSlayerChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Ancient God Slayer Chestplate");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 20;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.vanity = true;
            Item.value = Item.buyPrice(0, 60, 0, 0);
        }
    }
}
