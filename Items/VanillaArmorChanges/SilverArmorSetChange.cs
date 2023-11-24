using Microsoft.Xna.Framework;
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

        public const float HeadCrit = 5f;
        public const int ChestLifeRegen = 2;
        public const float LegsMoveSpeed = 0.08f;
        public const int SetBonusLifeRegen = 1;

        public const double SetBonusMinimumDamageToHeal = 20.0;
        public const int SetBonusHealTime = 120;
        public const int SetBonusHealAmount = 10;

        public override void ApplyHeadPieceEffect(Player player) => player.GetCritChance<GenericDamageClass>() += HeadCrit;

        public override void ApplyBodyPieceEffect(Player player) => player.lifeRegen += ChestLifeRegen;

        public override void ApplyLegPieceEffect(Player player) => player.moveSpeed += LegsMoveSpeed;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}")}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.lifeRegen += SetBonusLifeRegen;
            player.Calamity().silverMedkit = true;
        }

        internal static void OnHealEffects(Entity entity)
        {
            Vector2 dustCenter = entity.Center;

            int numDust = 36;
            for (int i = 0; i < numDust; ++i)
            {
                float theta = MathHelper.TwoPi * (i / 36f);
                Vector2 dustVel = 3.5f * Vector2.One.RotatedBy(theta);
                Dust d = Dust.NewDustPerfect(dustCenter, DustID.SilverCoin, dustVel, Scale: 1.4f);
                d.noGravity = true;
                d.noLight = false;
            }
        }
    }
}
