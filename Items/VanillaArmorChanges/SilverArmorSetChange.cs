using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class SilverArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.SilverHelmet;

        public override int? BodyPieceID => ItemID.SilverChainmail;

        public override int? LegPieceID => ItemID.SilverGreaves;

        public override string ArmorSetName => "Silver";

        public const float HeadCrit = 6f;
        public const int ChestLifeRegen = 2;
        public const float LegsMoveSpeed = 0.1f;
        public const int SetBonusLifeRegen = 1;
        public const int SetBonusMiningSpeedPercent = 25;

        public const double SetBonusMinimumDamageToHeal = 20.0;
        public const int SetBonusHealTime = 120;
        public const int SetBonusHealAmount = 10;

        public override void ApplyHeadPieceEffect(Player player) => player.GetCritChance<GenericDamageClass>() += HeadCrit;

        public override void ApplyBodyPieceEffect(Player player) => player.lifeRegen += ChestLifeRegen;

        public override void ApplyLegPieceEffect(Player player) => player.moveSpeed += LegsMoveSpeed;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("\n+1 life regen\nTwo seconds after getting hit for 20 or more damage, you heal for 10\nGetting hit again resets the timer, delaying the heal");
            sb.Append(CalamityGlobalItem.MiningSpeedString(SetBonusMiningSpeedPercent));
            setBonusText += sb.ToString();
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.lifeRegen += SetBonusLifeRegen;
            player.Calamity().silverMedkit = true;
            player.pickSpeed -= SetBonusMiningSpeedPercent * 0.01f;
        }
    }
}
