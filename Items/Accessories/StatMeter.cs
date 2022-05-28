using System.Collections.Generic;
using System.Linq;
using System.Text;
using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StatMeter : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Stat Meter");
            // TODO -- On April 1st, rename this item to "Pasta Strainer"
            Tooltip.SetDefault("Displays almost all player stats");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 6, 0, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            Player player = Main.player[Main.myPlayer];
            if (player is null)
                return;
            CalamityPlayer modPlayer = player.Calamity();

            Item heldItem = null;
            if (player.selectedItem >= 0 && player.selectedItem < Main.InventorySlotsTotal)
                heldItem = player.ActiveItem();

            // Replace the vanilla tooltip with a full stat readout
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");

            if (line != null)
                line.Text = CreateStatMeterTooltip(player, modPlayer, heldItem);

            // To save screen space, favorited tooltips do not exist for the Stat Meter
            list.RemoveAll(l => l.Mod == "Terraria" && (l.Name == "Favorite" || l.Name == "FavoriteDesc"));
        }

        private static string CreateStatMeterTooltip(Player player, CalamityPlayer modPlayer, Item heldItem)
        {
            static string OnePlace(float f) => f.ToString("n1");
            static string TwoPlaces(float f) => f.ToString("n2");
            
            //
            // RAW STATS (as obtained from the game engine, not yet modified for display
            //
            
            // Defense
            int defense = player.GetCurrentDefense(false);
            float DR = player.endurance;
            int lifeRegen = player.lifeRegen; // Normally we'd divide this by 2 in Expert without Well Fed, but we disable that shit

            // Mobility
            float moveSpeedBoost = player.moveSpeed - 1f;
            float jumpBoost = player.GetJumpBoost();
            float wingFlightTime = player.wingTimeMax;

            // Melee
            float trueMeleeDamage = (float)modPlayer.trueMeleeDamage;

            // Ranged
            float rangedAmmoConsumption = player.GetRangedAmmoCostReduction();

            // Magic
            float manaCost = player.manaCost;
            int manaRegen = player.manaRegen;

            // Summon
            int minionSlots = player.maxMinions;

            // rogue
            float rogueStealth = modPlayer.rogueStealthMax;
            float standingRegen = player.GetStandingStealthRegen();
            float movingRegen = player.GetMovingStealthRegen();
            float rogueVelocity = modPlayer.rogueVelocity - 1f;
            float rogueAmmoConsumption = modPlayer.rogueAmmoCost;

            // rippers
            float rageDamage = modPlayer.RageDamageBoost;
            float adrenalineDamage = modPlayer.GetAdrenalineDamage();
            int numAdrenBoosters = (modPlayer.adrenalineBoostOne ? 1 : 0) + (modPlayer.adrenalineBoostTwo ? 1 : 0) + (modPlayer.adrenalineBoostThree ? 1 : 0);
            float adrenalineDR = BalancingConstants.FullAdrenalineDR + numAdrenBoosters * BalancingConstants.AdrenalineDRPerBooster;

            // abyss
            int lightLevel = player.GetCurrentAbyssLightLevel();
            float breathLoss = modPlayer.abyssBreathLossStat;
            float breathLossRate = modPlayer.abyssBreathLossRateStat;
            int lifeLostAtZeroBreath = modPlayer.abyssLifeLostAtZeroBreathStat;
            int abyssDefenseLoss = modPlayer.abyssDefenseLossStat;

            // The notice about held item mattering is always displayed first.
            StringBuilder sb = new StringBuilder("Displays almost all player stats\nOffensive stats displayed vary with held item\n\n", 1024);

            // Only append rippers stats in Rev+
            if (CalamityWorld.revenge)
            {
                string rageDamageDisplay = TwoPlaces(100f * rageDamage);
                string adrenalineDamageDisplay = TwoPlaces(100f * adrenalineDamage);
                string adrenalineDRDisplay = TwoPlaces(100f * adrenalineDR);
                sb.Append("Rage Damage Boost: ").Append(rageDamageDisplay).Append("%\n");
                sb.Append("Adrenaline Damage Boost: ").Append(adrenalineDamageDisplay)
                    .Append("% | Adrenaline DR Boost: ").Append(adrenalineDRDisplay)
                    .Append("%\n\n");
            }

            // Append item stats only if the held item isn't null, and base it off of the item's damage type.
            if (heldItem != null && !heldItem.IsAir)
            {
                DamageClass dc = heldItem.DamageType;
                bool displayAttackSpeed = true;

                string damageClassName = "Unsupported";
                if (dc == DamageClass.Default)
                    damageClassName = "True (No Scaling)";
                else if (dc == DamageClass.Generic)
                    damageClassName = "Classless";
                else if (dc == AverageDamageClass.Instance)
                    damageClassName = "Averaged";
                else if (dc == DamageClass.Melee)
                    damageClassName = "Melee";
                else if (dc == DamageClass.Ranged)
                    damageClassName = "Ranged";
                else if (dc == DamageClass.Magic)
                    damageClassName = "Magic";
                else if (dc == DamageClass.MagicSummonHybrid)
                    damageClassName = "Magic+Summon";
                else if (dc == DamageClass.Summon)
                {
                    damageClassName = "Minion";
                    displayAttackSpeed = false; // Minions specifically don't display attack speed. Whips do.
                }
                else if (dc == DamageClass.SummonMeleeSpeed)
                    damageClassName = "Whip";
                else if (dc == RogueDamageClass.Instance)
                    damageClassName = "Rogue";
                else if (dc == DamageClass.Throwing)
                    damageClassName = "Throwing";

                // Uses GetTotalDamage instead of GetDamage. GetTotalDamage adds all inherited stats as well.
                // This also applies to GetTotalCritChance, GetTotalAttackSpeed, and GetTotalArmorPenetration.
                var currentStats = player.GetTotalDamage(dc);

                float baseFlatDamage = currentStats.Flat;
                float normalDamage = currentStats.Additive - 1f; // 1f = +0%, 2f = +100%
                float multDamage = currentStats.Multiplicative; // direct multiplier. 1f = 1x, 2f = 2x, applies after standard boosts
                float flatDamage = currentStats.Flat; // direct flat addition, applies after multiplication
                float totalCrit = player.GetTotalCritChance(dc); // exact crit, stored as a float. meaning with no gear it's 4f = 4% UNLESS summoner
                float attackSpeed = player.GetTotalAttackSpeed(dc); // starts at 1f, 2f = +100% = DOUBLE attack speed, HALF animation time
                float armorPen = player.GetTotalArmorPenetration(dc); // starts at 0f = +0 armor pen

                // Attack damage tooltip. The boosts are displayed in exactly the order they are applied
                sb.Append(damageClassName).Append(" Damage: ");

                // Flat base damage additions are extremely rare and only appear when relevant
                if (baseFlatDamage != 0f)
                {
                    char sign = baseFlatDamage < 0f ? '-' : '+';
                    sb.Append('[').Append(sign).Append(OnePlace(baseFlatDamage)).Append(" base] ");
                    sb.Append('+'); // Place this because otherwise the regular additive damage will look weird
                }

                // Additive damage is the vast majority of damage boosts
                sb.Append(TwoPlaces(100f * normalDamage)).Append('%');

                // Multiplicative damage is rare and only appears when relevant
                if (multDamage != 1f)
                    sb.Append(" x").Append(TwoPlaces(multDamage));

                // Flat damage is rare and only appears when relevant
                if (flatDamage != 0f)
                {
                    char sign = flatDamage < 0f ? '-' : '+';
                    sb.Append(' ').Append(sign).Append(OnePlace(flatDamage)).Append(" flat");
                }

                // If melee, display true melee damage
                if (dc == DamageClass.Melee)
                    sb.Append('\n').Append("True Melee Damage: ").Append(TwoPlaces(100f * trueMeleeDamage)).Append('%');

                // Newline between damage and crit
                sb.Append('\n').Append(damageClassName).Append(" Crit Chance: ");
                sb.Append(TwoPlaces(totalCrit)).Append('%');

                if (displayAttackSpeed)
                {
                    // Newline between crit and attack speed
                    sb.Append('\n').Append(damageClassName).Append(" Attack Speed: ");

                    // Positive boosts need a + prefix, negative will have its own - sign automatically.
                    // There's a "clever" way to do this with formatting but it's harder.
                    if (attackSpeed >= 1f)
                        sb.Append('+');
                    sb.Append(TwoPlaces(100f * (attackSpeed - 1f))).Append('%');

                    // For whips specifically, also display melee attack speed on the same line.
                    // Only display MELEE-SPECIFIC attack speed, not the total inheritance of attack speed for melee,
                    // because classless attack speed will already affect the whip.
                    if (dc == DamageClass.SummonMeleeSpeed)
                    {
                        float meleeSpeed = player.GetAttackSpeed<MeleeDamageClass>();
                        sb.Append('[');

                        // Positive boosts need a + prefix, negative will have its own - sign automatically.
                        // There's a "clever" way to do this with formatting but it's harder.
                        if (attackSpeed >= 1f)
                            sb.Append('+');
                        sb.Append(TwoPlaces(100f * (meleeSpeed - 1f))).Append("% from melee]");
                    }
                }

                // Newline between attack speed and armor penetration
                sb.Append('\n').Append(damageClassName).Append(" Armor Penetration: ");
                sb.Append(OnePlace(armorPen));

                // If ranged, or any direct subclass thereof, display ranged ammo consumption
                if (dc == DamageClass.Ranged || dc.GetModifierInheritance(DamageClass.Ranged).Equals(StatInheritanceData.Full))
                    sb.Append("\nAmmo Consumption Chance: ").Append(TwoPlaces(100f * rangedAmmoConsumption));

                // If magic, or any direct subclass thereof, display mana stats
                if (dc == DamageClass.Magic || dc == DamageClass.MagicSummonHybrid || dc.GetModifierInheritance(DamageClass.Magic).Equals(StatInheritanceData.Full))
                {
                    sb.Append("\nMana Usage: ").Append(TwoPlaces(100f * manaCost));
                    sb.Append("\nMana Regen: ").Append(manaRegen);
                }

                // If summon, or any direct subclass thereof, AND NOT A WHIP, display minion slots
                if (dc != DamageClass.SummonMeleeSpeed && (dc == DamageClass.Summon || dc.GetModifierInheritance(DamageClass.Summon).Equals(StatInheritanceData.Full)))
                    sb.Append("\nMinion Slots: ").Append(minionSlots);

                // If throwing or rogue, display rogue stats.
                if (dc == DamageClass.Throwing || dc.GetModifierInheritance(DamageClass.Throwing).Equals(StatInheritanceData.Full))
                {
                    sb.Append("\nRogue Velocity Boost: +").Append(TwoPlaces(100f * rogueVelocity));
                    sb.Append("% | Rogue Weapon Consumption Chance: ").Append(TwoPlaces(100f * rogueAmmoConsumption));
                    sb.Append("%\nMax Stealth: ").Append((int)(100f * rogueStealth));
                    sb.Append(" | Standing Regen: ").Append(TwoPlaces(60f * standingRegen)).Append(" / sec");
                    sb.Append(" | Moving Regen: ").Append(TwoPlaces(60f * movingRegen)).Append(" / sec");
                }
            }

            // Newline break between offensive and defensive stats
            sb.Append("\n\n");

            // Generic stats always render.
            sb.Append("Defense: ").Append(defense);
            sb.Append(" | DR: ").Append(TwoPlaces(100f * DR)).Append('%');
            sb.Append(" | Life Regen: ").Append(lifeRegen);

            sb.Append("\nMove Speed: ");
            // Positive boosts need a + prefix, negative will have its own - sign automatically.
            // There's a "clever" way to do this with formatting but it's harder.
            if (moveSpeedBoost >= 0f)
                sb.Append('+');
            sb.Append(TwoPlaces(100f * moveSpeedBoost)).Append('%');
            sb.Append(" | Jump Boost: ").Append(TwoPlaces(20f * jumpBoost)).Append('%');
            sb.Append(" | Wing Flight Time: ").Append(TwoPlaces(wingFlightTime / 60f)).Append(" seconds\n\n");

            // Detailed Abyss stats only render if the player is in the Abyss.
            sb.Append("Abyss Light Strength: ").Append(lightLevel).Append('\n');
            if (modPlayer.ZoneAbyss)
            {
                sb.Append("Other Abyss Stats:");
                sb.Append("\nBreath Lost Per Tick: ").Append(TwoPlaces(breathLoss));
                sb.Append(" | Breath Loss Rate: ").Append(TwoPlaces(breathLossRate));
                sb.Append("\nLife Lost Per Tick at Zero Breath: ").Append(lifeLostAtZeroBreath);
                sb.Append("\nDefense Lost From Pressure: ").Append(abyssDefenseLoss);
            }
            else
                sb.Append("Other Abyss stats are only displayed while in the Abyss");

            return sb.ToString();
        }
    }
}
