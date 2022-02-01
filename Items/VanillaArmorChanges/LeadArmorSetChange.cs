using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class LeadArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.LeadHelmet;

        public override int? BodyPieceID => ItemID.LeadChainmail;

        public override int? LegPieceID => ItemID.LeadGreaves;

        public override string ArmorSetName => "Lead";

        public const int MiningSpeedPercentSetBonus = 20;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += CalamityGlobalItem.MiningSpeedString(MiningSpeedPercentSetBonus);
        }

        public override void ApplyArmorSetBonus(Player player) => player.pickSpeed -= MiningSpeedPercentSetBonus * 0.01f;
    }
}
