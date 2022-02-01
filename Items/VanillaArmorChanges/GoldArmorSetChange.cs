using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class GoldArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.GoldHelmet;

        public override int? BodyPieceID => ItemID.GoldChainmail;

        public override int? LegPieceID => ItemID.GoldGreaves;

        public override string ArmorSetName => "Gold";

        public const int MiningSpeedPercentSetBonus = 45;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += CalamityGlobalItem.MiningSpeedString(MiningSpeedPercentSetBonus);
        }

        public override void ApplyArmorSetBonus(Player player) => player.pickSpeed -= MiningSpeedPercentSetBonus * 0.01f;
    }
}
