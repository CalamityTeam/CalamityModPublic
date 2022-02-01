using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class CopperArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.CopperHelmet;

        public override int? BodyPieceID => ItemID.CopperChainmail;

        public override int? LegPieceID => ItemID.CopperGreaves;

        public override string ArmorSetName => "Copper";

        public const int MiningSpeedPercentSetBonus = 15;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += CalamityGlobalItem.MiningSpeedString(MiningSpeedPercentSetBonus);
        }

        public override void ApplyArmorSetBonus(Player player) => player.pickSpeed -= MiningSpeedPercentSetBonus * 0.01f;
    }
}
