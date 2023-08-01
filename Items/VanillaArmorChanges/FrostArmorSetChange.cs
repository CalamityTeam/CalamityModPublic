using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class FrostArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.FrostHelmet;

        public override int? BodyPieceID => ItemID.FrostBreastplate;

        public override int? LegPieceID => ItemID.FrostLeggings;

        public override string ArmorSetName => "Frost";

        public const float ProximityBoost = 0.15f;
        public const float MinDistance = 160f;
        public const float MaxDistance = 800f;

        public override void UpdateSetBonusText(ref string setBonusText)
        {   
            int PercentBoost = (int)Math.Round(ProximityBoost * 100);
            setBonusText = "Melee and ranged attacks inflict Frostbite\n" +
                $"{PercentBoost}% increased damage which scales based on how far the target is from you\n" +
                "Closer range grants melee damage, while farther range grants ranged damage";
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().frostSet = true;

            // Cancel out the vanilla damage boosts
            player.GetDamage<MeleeDamageClass>() -= 0.1f;
            player.GetDamage<RangedDamageClass>() -= 0.1f;
        }
    }
}
