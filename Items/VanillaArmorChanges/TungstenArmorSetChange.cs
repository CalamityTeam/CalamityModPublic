using System.Text;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class TungstenArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.TungstenHelmet;

        public override int? BodyPieceID => ItemID.TungstenChainmail;

        public override int? LegPieceID => ItemID.TungstenGreaves;

        public override string ArmorSetName => "Tungsten";

        public const float HeadDamage = 0.07f;
        public const int ChestLifeRegen = 1;
        public const float LegsMoveSpeed = 0.1f;
        public const float KnockbackMultiplier = 1.33f;
        public const float MaxKnockbackCritConversion = 10f;

        public override void ApplyHeadPieceEffect(Player player) => player.GetDamage<GenericDamageClass>() += HeadDamage;

        public override void ApplyBodyPieceEffect(Player player) => player.lifeRegen += ChestLifeRegen;

        public override void ApplyLegPieceEffect(Player player) => player.moveSpeed += LegsMoveSpeed;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            Player player = Main.LocalPlayer;
            float kb = player.GetWeaponKnockback(player.ActiveItem());
            float bonus = MathHelper.Clamp(kb, 0f, MaxKnockbackCritConversion);
            
            StringBuilder sb = new StringBuilder(256);
            sb.Append("\nIncreases your critical strike chance by 100% of the knockback of your held weapon");
            sb.Append("\nThis effect caps at Insane knockback, which gives 10% increased critical strike chance");
            sb.Append("\nIncreases all knockback by 33%, this counts for the above boost");
            sb.Append("\nCurrent bonus: ");
            sb.Append(bonus.ToString("n2"));
            sb.Append("% critical strike chance from ");
            sb.Append(kb.ToString("n2"));
            sb.Append(" knockback");
            setBonusText += sb.ToString();
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            // Apply the 33% knockback boost first, so it applies to the following crit boost
            player.GetKnockback<GenericDamageClass>() *= KnockbackMultiplier;

            // Give the player crit chance equal to the (now boosted) knockback of their held weapon.
            float kbToUse = player.GetWeaponKnockback(player.ActiveItem());
            player.GetCritChance<GenericDamageClass>() += MathHelper.Clamp(kbToUse, 0f, MaxKnockbackCritConversion);
        }
    }
}
