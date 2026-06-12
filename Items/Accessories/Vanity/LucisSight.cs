using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Vanity
{
    [AutoloadEquip(EquipType.Face)]
    public class LucisSight : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Face);
                ArmorIDs.Face.Sets.DrawInFaceUnderHairLayer[equipSlot] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.value = Item.buyPrice(gold: 10); // Sold by Steampunker
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
            Item.vanity = true;
            Item.Calamity().donorItem = true;
        }
    }
}
