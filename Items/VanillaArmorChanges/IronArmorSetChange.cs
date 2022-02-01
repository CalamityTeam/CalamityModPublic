using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class IronArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.IronHelmet;

        public override int? BodyPieceID => ItemID.IronChainmail;

        public override int? LegPieceID => ItemID.IronGreaves;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.AncientIronHelmet };

        public override string ArmorSetName => "Iron";

        public const int MiningSpeedPercentSetBonus = 25;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += CalamityGlobalItem.MiningSpeedString(MiningSpeedPercentSetBonus);
        }

        public override void ApplyArmorSetBonus(Player player) => player.pickSpeed -= MiningSpeedPercentSetBonus * 0.01f;
    }
}
