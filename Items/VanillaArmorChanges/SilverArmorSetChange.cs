using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class SilverArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.SilverHelmet;

        public override int? BodyPieceID => ItemID.SilverChainmail;

        public override int? LegPieceID => ItemID.SilverGreaves;

        public override string ArmorSetName => "Silver";

        public const int MiningSpeedPercentSetBonus = 35;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += CalamityGlobalItem.MiningSpeedString(MiningSpeedPercentSetBonus);
        }

        public override void ApplyArmorSetBonus(Player player) => player.pickSpeed -= MiningSpeedPercentSetBonus * 0.01f;
    }
}
