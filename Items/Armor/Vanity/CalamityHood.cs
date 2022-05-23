using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class CalamityHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Hood of Calamity");

            if (Main.netMode != NetmodeID.Server)
                ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 24;
            Item.rare = ItemRarityID.Lime;
            Item.vanity = true;
            Item.Calamity().donorItem = true;
            Item.value = Item.sellPrice(gold: 2);
        }
    }
}
