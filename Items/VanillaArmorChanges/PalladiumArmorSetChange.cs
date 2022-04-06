using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class PalladiumArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.PalladiumHelmet;

        public override int? BodyPieceID => ItemID.PalladiumBreastplate;

        public override int? LegPieceID => ItemID.PalladiumLeggings;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.PalladiumHeadgear, ItemID.PalladiumMask };

        public override string ArmorSetName => "Palladium";

        public const int ChestplateDamagePercentageBoost = 4;
        public const int LeggingsDamagePercentageBoost = 5;

        public override void ApplyBodyPieceEffect(Player player)
        {
            player.GetDamage<GenericDamageClass>() += ChestplateDamagePercentageBoost * 0.01f;
        }

        public override void ApplyLegPieceEffect(Player player)
        {
            player.GetDamage<GenericDamageClass>() += LeggingsDamagePercentageBoost * 0.01f;
        }
    }
}
