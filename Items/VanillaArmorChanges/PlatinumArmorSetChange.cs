using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class PlatinumArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.PlatinumHelmet;

        public override int? BodyPieceID => ItemID.PlatinumChainmail;

        public override int? LegPieceID => ItemID.PlatinumGreaves;

        public override string ArmorSetName => "Platinum";

        public const float HeadDamage = 0.06f;
        public const float ChestCrit = 5f;
        public const float LegsMoveSpeed = 0.12f;
        public const float SetBonusLifeRegenPerDefense = 0.0666666f; // 15 defense = +1 life regen
        public const float SetBonusCritPerDefense = 0.0666666f; // 15 defense = +1% crit chance
        public const int SetBonusDefenseCap = 50;
        public const int SetBonusMiningSpeedPercent = 25;

        public override void ApplyHeadPieceEffect(Player player) => player.GetDamage<GenericDamageClass>() += HeadDamage;

        public override void ApplyBodyPieceEffect(Player player) => player.GetCritChance<GenericDamageClass>() += ChestCrit;

        public override void ApplyLegPieceEffect(Player player) => player.moveSpeed += LegsMoveSpeed;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("\nEvery 15 defense gives you +1 life regen\nEvery 15 defense gives you 1% increased critical strike chance\nThese effects both cap at 50 defense");
            sb.Append(CalamityGlobalItem.MiningSpeedString(SetBonusMiningSpeedPercent));
            setBonusText += sb.ToString();
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            int defense = player.statDefense;
            if (defense > SetBonusDefenseCap)
                defense = SetBonusDefenseCap;
            player.lifeRegen += (int)(defense * SetBonusLifeRegenPerDefense);
            player.GetCritChance<GenericDamageClass>() += defense * SetBonusCritPerDefense;
            player.pickSpeed -= SetBonusMiningSpeedPercent * 0.01f;
        }
    }
}
