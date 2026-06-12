using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class PalladiumArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.PalladiumHelmet;

        public override int? BodyPieceID => ItemID.PalladiumBreastplate;

        public override int? LegPieceID => ItemID.PalladiumLeggings;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.PalladiumHeadgear, ItemID.PalladiumMask };

        public override string ArmorSetName => "Palladium";

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = $"{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }
    }
}
