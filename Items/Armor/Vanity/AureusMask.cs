using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    public class AureusMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Astrum Aureus Mask");

            if (Main.netMode != NetmodeID.Server)
                ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.Blue;
            Item.vanity = true;
        }
    }
}
