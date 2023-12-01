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
            Player player = Main.LocalPlayer;
            if (player.armor[0].type == ItemID.AdamantiteHelmet)
            {
                setBonusText = CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}.Melee");
            }
            
            setBonusText += $"\n{CalamityUtils.GetText($"Vanilla.Armor.SetBonus.{ArmorSetName}").Format(DefenseBoostMax)}";
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
