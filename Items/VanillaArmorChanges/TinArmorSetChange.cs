using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class TinArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.TinHelmet;

        public override int? BodyPieceID => ItemID.TinChainmail;

        public override int? LegPieceID => ItemID.TinGreaves;

        public override string ArmorSetName => "Tin";

        public const float ArmorPen = 5.0f;
        public const int LifeRegen = 1;
        public const int MiningSpeedPercentSetBonus = 25;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("+5 armor penetration, +1 life regen\n");
            sb.Append(CalamityGlobalItem.MiningSpeedString(MiningSpeedPercentSetBonus));
            setBonusText += sb.ToString();
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.GetArmorPenetration<GenericDamageClass>() += ArmorPen;
            player.lifeRegen += LifeRegen;
            player.pickSpeed -= MiningSpeedPercentSetBonus * 0.01f;
        }
    }
}
