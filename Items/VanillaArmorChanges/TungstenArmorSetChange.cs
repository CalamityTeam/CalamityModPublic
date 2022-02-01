using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class TungstenArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.TungstenHelmet;

        public override int? BodyPieceID => ItemID.TungstenChainmail;

        public override int? LegPieceID => ItemID.TungstenGreaves;

        public override string ArmorSetName => "Tungsten";

        public const int MiningSpeedPercentSetBonus = 30;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += CalamityGlobalItem.MiningSpeedString(MiningSpeedPercentSetBonus);
        }

        public override void ApplyArmorSetBonus(Player player) => player.pickSpeed -= MiningSpeedPercentSetBonus * 0.01f;
    }
}
