using CalamityMod.World;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class MoltenArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.TinHelmet;

        public override int? BodyPieceID => ItemID.TinChainmail;

        public override int? LegPieceID => ItemID.TinGreaves;

        public override string ArmorSetName => "Molten";

        public const int MiningSpeedPercentSetBonus = 10;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            string extraLine = "\n20% extra true melee damage\nGrants immunity to fire blocks and temporary immunity to lava";
            if (CalamityWorld.death)
                extraLine += CalamityGlobalItem.BothProtectionLine;
            setBonusText += extraLine;
        }

        // TODO - The death mode immuntities are hardcoded in the misc player effects file. Perhaps make it into a generalized bool?
        public override void ApplyArmorSetBonus(Player player)
        {
            player.fireWalk = true;
            player.lavaMax += 300;
        }
    }
}
