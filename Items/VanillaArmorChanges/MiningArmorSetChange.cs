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

        public const float MiningSpeed = 0.2f;
        public const int BonusOreChance = 4;
        public const int CooldownMin = 180;
        public const int CooldownMax = 360;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = setBonusText.Replace("10%", "20%");
			setBonusText += "\nMining ore will sometimes have additional yield";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            // Remove the vanilla +30% pick speed and add (subtract?) the new pick speed value at the same time
            player.pickSpeed -= MiningSpeed + 0.1f;
			player.Calamity().miningSet = true;
        }
    }
}
