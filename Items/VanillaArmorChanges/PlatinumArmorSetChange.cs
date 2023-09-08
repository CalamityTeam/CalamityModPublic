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
        public const float LegsMoveSpeed = 0.1f;
        public const float SetBonusLifeRegenPerDefense = 0.1f; // 10 defense = +1 life regen
        public const float SetBonusDamagePerDefense = 0.001f; // 10 defense = +1% damage
        public const float SetBonusCritPerDefense = 0.1f; // 10 defense = +1% crit chance
        public const int SetBonusDefenseCap = 40;

        public override void ApplyHeadPieceEffect(Player player) => player.GetDamage<GenericDamageClass>() += HeadDamage;

        public override void ApplyBodyPieceEffect(Player player) => player.GetCritChance<GenericDamageClass>() += ChestCrit;

        public override void ApplyLegPieceEffect(Player player) => player.moveSpeed += LegsMoveSpeed;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append("\nEvery 10 defense gives you +0.5 HP/s life regen, 1% increased damage and 1% increased critical strike chance\nThese effects cap at 40 defense");
            setBonusText += sb.ToString();
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            int defense = player.statDefense;
            if (defense > SetBonusDefenseCap)
                defense = SetBonusDefenseCap;
            player.lifeRegen += (int)(defense * SetBonusLifeRegenPerDefense);
            player.GetDamage<GenericDamageClass>() += defense * SetBonusDamagePerDefense;
            player.GetCritChance<GenericDamageClass>() += defense * SetBonusCritPerDefense;
        }
    }
}
