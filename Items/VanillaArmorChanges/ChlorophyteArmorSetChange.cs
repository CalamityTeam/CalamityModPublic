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
            setBonusText = $"Summons a powerful leaf crystal to shoot pulses of life every {PulseReleaseRate / 60f} seconds\n" +
                $"The pulses do a base damage of {BaseDamageToEnemies} to enemies within its range\n" +
                $"The pulses also provide a {AmountToHealPerPulse} health boost to you and all players on your team\n" +
                $"Players healed by pulses cannot be healed by another pulse until {DelayBetweenHeals / 60f} seconds have passed\n" +
                "Both the health boost and the damage scale based on your strongest class";
        }
    }
}
