using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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

        public override bool NeedsToCreateSetBonusTextManually => true;

        public override string ArmorSetName => "Eskimo";

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = "Multiplies all cold-based debuff damage by 1.25\n" +
                "Cold enemies will deal reduced contact damage to the player\n" +
                "Provides immunity to the Frostburn and Glacial State debuffs";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().eskimoSet = true;
            player.buffImmune[BuffID.Frostburn] = true;
            player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
        }
    }
}
