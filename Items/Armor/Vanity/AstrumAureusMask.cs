using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Armor.Vanity
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("AureusMask")]
    public class AstrumAureusMask : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Armor";
        public override void SetStaticDefaults()
        {

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
