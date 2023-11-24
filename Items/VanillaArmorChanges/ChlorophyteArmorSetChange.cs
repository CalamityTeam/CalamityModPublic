using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class ChlorophyteArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.ChlorophyteHeadgear;

        public override int? BodyPieceID => ItemID.ChlorophytePlateMail;

        public override int? LegPieceID => ItemID.ChlorophyteGreaves;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.ChlorophyteMask, ItemID.ChlorophyteHelmet };

        public override string ArmorSetName => "Chlorophyte";

        public const int AmountToHealPerPulse = 10;

        public const int PulseReleaseRate = 300;

        public const int DelayBetweenHeals = 270;

        public const int BaseDamageToEnemies = 250;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = $"{CalamityUtils.GetText($"Vanilla.Armor.SetBonus.{ArmorSetName}").Format(PulseReleaseRate / 60f, BaseDamageToEnemies, AmountToHealPerPulse, DelayBetweenHeals / 60f)}";
        }
    }
}
