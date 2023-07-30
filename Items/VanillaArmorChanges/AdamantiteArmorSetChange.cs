using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class AdamantiteArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.AdamantiteHelmet;

        public override int? BodyPieceID => ItemID.AdamantiteBreastplate;

        public override int? LegPieceID => ItemID.AdamantiteLeggings;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.AdamantiteHeadgear, ItemID.AdamantiteMask };

        public override string ArmorSetName => "Adamantite";

        public const int MaxManaBoost = 20;
        public const int DefenseBoostMax = 15;
        public const int TimeUntilDecayBeginsAfterAttacking = 60;
        public const int TimeUntilBoostCompletelyDecays = 210;

        public override void ApplyHeadPieceEffect(Player player)
        {
            if (player.armor[0].type == ItemID.AdamantiteHeadgear)
                player.statManaMax2 += MaxManaBoost;
        }

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText = setBonusText.Replace("20% increased melee and movement speed", "15% increased melee speed and 20% increased movement speed");
            setBonusText += "\nHalf of your current DR is added to your critical strike chance\n" +
                $"Continuously doing damage makes you gradually gain more and more defense, up to a maximum of {DefenseBoostMax}\n" +
                "When not doing damage, this bonus gradually decays\n" +
                "This added defense can be broken by defense damage";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            int critBoost = (int)(MathHelper.Clamp(player.endurance, 0f, 1f) * 50f);
            switch (player.armor[0].type)
            {
                case ItemID.AdamantiteHeadgear:
                    player.GetCritChance<MagicDamageClass>() += critBoost;
                    break;
                case ItemID.AdamantiteHelmet:
                    player.GetCritChance<MeleeDamageClass>() += critBoost;
                    player.GetAttackSpeed<MeleeDamageClass>() -= 0.05f;
                    break;
                case ItemID.AdamantiteMask:
                    player.GetCritChance<RangedDamageClass>() += critBoost;
                    break;
            }
            player.Calamity().AdamantiteSet = true;
        }
    }
}
