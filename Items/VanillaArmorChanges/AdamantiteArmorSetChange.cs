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

        public const int DefenseBoostMax = 10;
        public const int TimeUntilDecayBeginsAfterAttacking = 60;
        public const int TimeUntilBoostCompletelyDecays = 210; // Unlisted use: this is also the amount of hits to max out defense.
        public const int CritToDRConversionPercent = 25;

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            Player player = Main.LocalPlayer;
            if (player.armor[0].type == ItemID.AdamantiteHelmet)
            {
                setBonusText = CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}.Melee");
            }

            setBonusText += $"\n{CalamityUtils.GetText($"Vanilla.Armor.SetBonus.{ArmorSetName}").Format(CritToDRConversionPercent, DefenseBoostMax, TimeUntilBoostCompletelyDecays.FramesToSeconds())}";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            int critBoost = (int)(MathHelper.Clamp(player.endurance, 0f, 1f) * CritToDRConversionPercent);
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
