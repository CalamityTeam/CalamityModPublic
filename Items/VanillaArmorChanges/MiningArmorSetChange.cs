using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class MiningArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.MiningHelmet;
        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.UltrabrightHelmet };

        public override int? BodyPieceID => ItemID.MiningShirt;

        public override int? LegPieceID => ItemID.MiningPants;

        public override string ArmorSetName => "Mining";

        public const float MiningSpeed = 0.4f;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = setBonusText.Replace("30%", "40%");
			setBonusText += "\nMining ore will sometimes have additional yield";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            // Remove the vanilla +30% pick speed and add (subtract?) the new pick speed value at the same time
            player.pickSpeed -= MiningSpeed + 0.3f;
			player.Calamity().miningSet = true;
        }
    }
}
