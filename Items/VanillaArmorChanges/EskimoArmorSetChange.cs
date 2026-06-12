using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class EskimoArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.EskimoHood;

        public override int? BodyPieceID => ItemID.EskimoCoat;

        public override int? LegPieceID => ItemID.EskimoPants;

        // The normal and Pink Eskimo set can be mixed and matched.
        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.PinkEskimoHood };

        public override int[] AlternativeBodyPieceIDs => new int[] { ItemID.PinkEskimoCoat };

        public override int[] AlternativeLegPieceIDs => new int[] { ItemID.PinkEskimoPants };

        public override string ArmorSetName => "Eskimo";

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = $"{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().eskimoSet = true;
            player.Calamity().ColdDebuffMultiplier += 0.50f;

            player.buffImmune[BuffID.Frostburn] = true;
        }
    }
}
