using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.VanillaArmorChanges
{
    public class CobaltArmorSetChange : VanillaArmorChange
    {
        public override int? HeadPieceID => ItemID.CobaltHelmet;

        public override int? BodyPieceID => ItemID.CobaltBreastplate;

        public override int? LegPieceID => ItemID.CobaltLeggings;

        public override int[] AlternativeHeadPieceIDs => new int[] { ItemID.CobaltHat, ItemID.CobaltMask };

        public override string ArmorSetName => "Cobalt";

        public const int SpeedBoostSetBonusPercentage = 10;
        public const int MaxManaBoost = 20;
        public const int MovementSpeedBoostPercentageMax = 10;
        public const int MovementSpeedBoostMphThreshold = 80;

        public override void ApplyHeadPieceEffect(Player player)
        {
            if (player.armor[0].type == ItemID.CobaltHat)
                player.statManaMax2 += MaxManaBoost;
        }

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            Player player = Main.LocalPlayer;
            if (player.armor[0].type == ItemID.CobaltHelmet)
            {
                setBonusText = CalamityUtils.GetTextValue($"Vanilla.Armor.SetBonus.{ArmorSetName}.Melee");
            }
            
            setBonusText += $"\n{CalamityUtils.GetText($"Vanilla.Armor.SetBonus.{ArmorSetName}").Format(SpeedBoostSetBonusPercentage)}";
        }

        public static float CalculateMovementSpeedInterpolant(Player player)
        {
            float milesPerHour = player.velocity.Length() * 225f / 44f;
            float movementSpeedInterpolant = Utils.GetLerpValue(0f, MovementSpeedBoostMphThreshold, milesPerHour, true);
            return (float)Math.Pow(movementSpeedInterpolant, 5D / 3D);
        }

        public static void ApplyMovementSpeedBonuses(Player player)
        {
            float movementSpeedInterpolant = CalculateMovementSpeedInterpolant(player);
            player.GetDamage<GenericDamageClass>() += MovementSpeedBoostPercentageMax * movementSpeedInterpolant * 0.01f;
            float critBonus = MovementSpeedBoostPercentageMax * movementSpeedInterpolant;
            player.GetCritChance<GenericDamageClass>() += critBonus;
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.GetAttackSpeed<MeleeDamageClass>() -= 0.05f;
            player.Calamity().CobaltSet = true;
        }
    }
}
