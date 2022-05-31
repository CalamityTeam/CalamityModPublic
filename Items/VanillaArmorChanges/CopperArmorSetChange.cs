using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class CopperArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.CopperHelmet;

        public override int? BodyPieceID => ItemID.CopperChainmail;

        public override int? LegPieceID => ItemID.CopperGreaves;

        public override string ArmorSetName => "Copper";

        public const float FlatDamage = 2.0f;
        public const float MoveSpeed = 0.1f;
        public const int MiningSpeedPercentSetBonus = 25;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("+2 flat damage to all attacks, +10% movement speed\n");
            sb.Append(CalamityGlobalItem.MiningSpeedString(MiningSpeedPercentSetBonus));
            setBonusText += sb.ToString();
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.GetDamage<GenericDamageClass>().Flat += FlatDamage;
            player.moveSpeed += MoveSpeed;
            player.pickSpeed -= MiningSpeedPercentSetBonus * 0.01f;
        }
    }
}
