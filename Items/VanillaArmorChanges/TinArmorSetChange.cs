using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class TinArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.TinHelmet;

        public override int? BodyPieceID => ItemID.TinChainmail;

        public override int? LegPieceID => ItemID.TinGreaves;

        public override string ArmorSetName => "Tin";

        public const int MiningSpeedPercentSetBonus = 10;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += CalamityGlobalItem.MiningSpeedString(MiningSpeedPercentSetBonus);
        }

        public override void ApplyArmorSetBonus(Player player) => player.pickSpeed -= MiningSpeedPercentSetBonus * 0.01f;
    }
}
