using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class OrichalcumArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.OrichalcumHelmet;

        public override int? BodyPieceID => ItemID.OrichalcumBreastplate;

        public override int? LegPieceID => ItemID.OrichalcumLeggings;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.OrichalcumHeadgear, ItemID.OrichalcumMask };

        public override string ArmorSetName => "Orichalcum";

        public const int ChestplateCritChanceBoost = 4;

        public override void ApplyBodyPieceEffect(Player player) => player.GetCritChance<GenericDamageClass>() += ChestplateCritChanceBoost;
    }
}
