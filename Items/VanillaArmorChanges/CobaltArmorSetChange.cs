using System;
using Terraria;
using Terraria.ID;

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
        public const int MovementSpeedBoostMphThreshold = 60;

        public override void ApplyHeadPieceEffect(Player player)
        {
            if (player.armor[0].type == ItemID.CobaltHat)
                player.statManaMax2 += MaxManaBoost;
        }

        public override void UpdateSetBonusText(ref string setBonusText)
        {
            setBonusText += $"\n{SpeedBoostSetBonusPercentage}% increased max speed and acceleration\n" +
                $"You gain a damage and critical strike chance boost relative to your current movement speed, up to {SpeedBoostSetBonusPercentage}%";
        }

        public static float CalculateMovementSpeedInterpolant(Player player)
        {
            float milesPerHour = player.velocity.Length() * 225f / 44f;
            float movementSpeedInterpolant = Utils.InverseLerp(0f, MovementSpeedBoostMphThreshold, milesPerHour, true);
            return (float)Math.Pow(movementSpeedInterpolant, 5D / 3D);
        }

        public static void ApplyMovementSpeedBonuses(Player player)
        {
            float movementSpeedInterpolant = CalculateMovementSpeedInterpolant(player);
            player.allDamage += MovementSpeedBoostPercentageMax * movementSpeedInterpolant * 0.01f;
            player.Calamity().AllCritBoost((int)(MovementSpeedBoostPercentageMax * movementSpeedInterpolant));
        }

        public override void ApplyArmorSetBonus(Player player)
        {
            player.Calamity().CobaltSet = true;
        }
    }
}
