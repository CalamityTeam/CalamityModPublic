using CalamityMod.Balancing;
using CalamityMod.CalPlayer;
using CalamityMod.World;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class StatMeter : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.zenithWorld || (DateTime.Now.Month == 4 && DateTime.Now.Day == 1))
                Item.SetNameOverride(this.GetLocalizedValue("FoolsName"));
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
            
            static string OnePlace(float f) => f.ToString("n1");
            static string TwoPlaces(float f) => f.ToString("n2");

            // Positive boosts need a + prefix, negative will have its own - sign automatically.
            // There's a "clever" way to do this with formatting but it's harder.
            static string Sign(float f) => (f >= 0 ? "+" : string.Empty);

            int numRageBoosters = (modPlayer.rageBoostOne ? 1 : 0) + (modPlayer.rageBoostTwo ? 1 : 0) + (modPlayer.rageBoostThree ? 1 : 0);
            int rageDuration = BalancingConstants.DefaultRageDuration + numRageBoosters * BalancingConstants.RageDurationPerBooster;
            int numAdrenBoosters = (modPlayer.adrenalineBoostOne ? 1 : 0) + (modPlayer.adrenalineBoostTwo ? 1 : 0) + (modPlayer.adrenalineBoostThree ? 1 : 0);
            float adrenalineDR = BalancingConstants.FullAdrenalineDR + numAdrenBoosters * BalancingConstants.AdrenalineDRPerBooster;
            // Only append rippers stats in Rev+
            string stats1 = !CalamityWorld.revenge ? string.Empty : "\n" + this.GetLocalization("RevStats").Format(
                OnePlace(100f * modPlayer.RageDamageBoost),
                rageDuration / 60,
                OnePlace(100f * modPlayer.GetAdrenalineDamage()),
                OnePlace(100f * adrenalineDR));
            list.FindAndReplace("[REV]", stats1);

            // Append item stats only if the held item isn't null, and base it off of the item's damage type.
            string stats2 = string.Empty;
            if (heldItem != null && !heldItem.IsAir)
            {
                // If block/wall, add respective placement stats, and ignore all combat stats
                if (heldItem.createWall >= 0 || heldItem.createTile >= 0)
                {
                    int extraBlockRangeX = player.blockRange + Player.tileRangeX - 5;
                    int extraBlockRangeY = player.blockRange + Player.tileRangeY - 4;
                    stats2 += "\n" + this.GetLocalization("PlacementRange").Format(Sign(extraBlockRangeX) + extraBlockRangeX, Sign(extraBlockRangeY) + extraBlockRangeY);

                    if (heldItem.createTile >= 0 && player.tileSpeed != 0f)
                    {
                        float tileSpeed = (1f / player.tileSpeed) - 1f;
                        stats2 += this.GetLocalization("TileSpeed").Format(Sign(tileSpeed) + OnePlace(100f * tileSpeed));
                    }
                    else if (heldItem.createWall >= 0 && player.wallSpeed != 0f)
                    {
                        float wallSpeed = (1f / player.wallSpeed) - 1f;
                        stats2 += this.GetLocalization("WallSpeed").Format(Sign(wallSpeed) + OnePlace(100f * wallSpeed));
                    }
                }
                else if (heldItem.damage >= 0)
                {
                    DamageClass dc = heldItem.DamageType;
                    string damageClassLine = dc.DisplayName.ToString();
                    bool displayCrit = true;
                    bool displayAttackSpeed = true;
                    if (dc == DamageClass.Summon)
                    {
                        displayCrit = false;
                        displayAttackSpeed = false; // Minions specifically don't display attack speed. Whips do.
                    }
                    else if (dc == DamageClass.SummonMeleeSpeed)
                    {
                        damageClassLine += this.GetLocalizedValue("ClassNameOverride.SummonMeleeSpeed");
                        displayCrit = false;
                    }
                    else if (dc == DamageClass.MeleeNoSpeed || dc == TrueMeleeNoSpeedDamageClass.Instance)
                    {
                        damageClassLine += this.GetLocalizedValue("ClassNameOverride.NoSpeed");
                        displayAttackSpeed = false;
                    }
                    else if (dc == DamageClass.Generic)
                        damageClassLine = this.GetLocalizedValue("ClassNameOverride.Generic");
                    else if (dc == DamageClass.Default)
                    {
                        damageClassLine = this.GetLocalizedValue("ClassNameOverride.Default");
                        displayCrit = false;
                    }

                    // Uses GetTotalDamage instead of GetDamage. GetTotalDamage adds all inherited stats as well.
                    // This also applies to GetTotalCritChance, GetTotalAttackSpeed, and GetTotalArmorPenetration.
                    var currentStats = player.GetTotalDamage(dc);
                    string damageStatLine = string.Empty;

                    float baseFlatDamage = currentStats.Base; // flat increases to the base damage of the weapon. boosted by % damage and multiplicative damage
                    if (baseFlatDamage != 0f)
                        damageStatLine += this.GetLocalization("DamageBase").Format(Sign(baseFlatDamage) + OnePlace(baseFlatDamage));

                    float normalDamage = currentStats.Additive - 1f; // 1f = +0%, 2f = +100%
                    damageStatLine += this.GetLocalization("DamageNormal").Format(TwoPlaces(100f * normalDamage));

                    float multDamage = currentStats.Multiplicative; // direct multiplier. 1f = 1x, 2f = 2x, applies after standard boosts
                    if (multDamage != 1f)
                        damageStatLine += this.GetLocalization("DamageMult").Format(TwoPlaces(multDamage));

                    float flatDamage = currentStats.Flat; // direct flat addition, applies after multiplication
                    if (flatDamage != 0f)
                        damageStatLine += this.GetLocalization("DamageFlat").Format(Sign(flatDamage) + OnePlace(flatDamage));

                    stats2 += "\n" + this.GetLocalization("DamageStats").Format(
                        damageClassLine,
                        damageStatLine,
                        OnePlace(player.GetTotalArmorPenetration(dc)));

                    if (displayCrit)
                        stats2 += this.GetLocalization("Crit").Format(TwoPlaces(player.GetTotalCritChance(dc)));

                    if (displayAttackSpeed)
                    {
                        // Newline between crit and attack speed
                        float attackSpeed = player.GetTotalAttackSpeed(dc) - 1f; // starts at 1f, 2f = +100% = DOUBLE attack speed, HALF animation time
                        stats2 += "\n" + this.GetLocalization("AttackSpeed").Format(Sign(attackSpeed) + TwoPlaces(100f * attackSpeed));

                        // For whips specifically, also display melee attack speed on the same line.
                        // Only display MELEE-SPECIFIC attack speed, not the total inheritance of attack speed for melee,
                        // because classless attack speed will already affect the whip.
                        if (dc == DamageClass.SummonMeleeSpeed)
                        {
                            float meleeSpeed = player.GetAttackSpeed<MeleeDamageClass>() - 1f;
                            stats2 += this.GetLocalization("WhipMeleeInheritance").Format(Sign(meleeSpeed) + TwoPlaces(100f * meleeSpeed));
                        }
                    }

                    // If ranged, or any direct subclass thereof, display ranged ammo consumption
                    if (dc == DamageClass.Ranged || dc.GetModifierInheritance(DamageClass.Ranged).Equals(StatInheritanceData.Full))
                        stats2 += "\n" + this.GetLocalization("RangedStats").Format(TwoPlaces(100f * player.GetRangedAmmoCostReduction()));

                    // If magic, or any direct subclass thereof, display mana stats
                    if (dc == DamageClass.Magic || dc == DamageClass.MagicSummonHybrid || dc.GetModifierInheritance(DamageClass.Magic).Equals(StatInheritanceData.Full))
                        stats2 += "\n" + this.GetLocalization("MagicStats").Format(TwoPlaces(100f * player.manaCost), player.manaRegen);

                    // If summon, or any direct subclass thereof, AND NOT A WHIP, display minion/sentry slots
                    if (dc != DamageClass.SummonMeleeSpeed && (dc == DamageClass.Summon || dc.GetModifierInheritance(DamageClass.Summon).Equals(StatInheritanceData.Full)))
                        stats2 += "\n" + this.GetLocalization("SummonStats").Format(player.maxMinions, player.maxTurrets);
                    
                    // If whip, show whip range
                    float whipRange = player.whipRangeMultiplier - 1f;
                    if (dc == DamageClass.SummonMeleeSpeed || dc.GetModifierInheritance(DamageClass.SummonMeleeSpeed).Equals(StatInheritanceData.Full))
                        stats2 += "\n" + this.GetLocalization("WhipStats").Format(Sign(whipRange) + TwoPlaces(100f * whipRange));

                    // If throwing or rogue, display rogue stats.
                    float rogueVelocity = modPlayer.rogueVelocity - 1f;
                    if (dc == DamageClass.Throwing || dc.GetModifierInheritance(DamageClass.Throwing).Equals(StatInheritanceData.Full))
                    {
                        stats2 += "\n" + this.GetLocalization("RogueStats").Format(
                            (int)(100f * modPlayer.rogueStealthMax),
                            TwoPlaces(60f * player.GetStandingStealthRegen()),
                            TwoPlaces(60f * player.GetMovingStealthRegen()),
                            Sign(rogueVelocity) + TwoPlaces(100f * rogueVelocity));

                        // Rogue consumable chance only if item is consumable
                        if (heldItem.consumable)
                            stats2 += this.GetLocalization("RogueConsumption").Format(100f * modPlayer.rogueAmmoCost);
                    }

                    // If tool, add tool range
                    if (heldItem.pick > 0 || heldItem.axe > 0 || heldItem.hammer > 0)
                    {
                        int extraToolRangeX = Player.tileRangeX - 5;
                        int extraToolRangeY = Player.tileRangeY - 4;
                        stats2 += "\n" + this.GetLocalization("ToolRange").Format(Sign(extraToolRangeX) + extraToolRangeX, Sign(extraToolRangeY) + extraToolRangeY);

                        // Pickaxe speed specifically for pickaxes
                        if (heldItem.pick > 0)
                        {
                            float pickSpeed = 1f - player.pickSpeed;
                            stats2 += this.GetLocalization("MiningSpeed").Format(Sign(pickSpeed) + OnePlace(100f * pickSpeed));
                        }
                    }
                }
            }
            list.FindAndReplace("[ITEMS]", stats2);

            float moveSpeedBoost = player.moveSpeed - 1f;
            float wingFlightTime = player.wingTimeMax;
            // Does not use NormalizedLuck. Presents the player's luck exactly as it is used by the game engine.
            // NormalizedLuck is only used in one place: the Wizard's luck report. Which is entirely obsoleted by this Meter.
            float luck = player.luck;

            // Generic stats always render.
            string stats3 = "\n" + this.GetLocalization("GenericStats").Format(
                player.GetCurrentDefense(false),
                TwoPlaces(100f * player.endurance),
                player.lifeRegen, // Normally we'd divide this by 2 in Expert without Well Fed, but we disable that shit
                Sign(moveSpeedBoost) + TwoPlaces(100f * moveSpeedBoost),
                TwoPlaces(20f * player.GetJumpBoost()));
            // Show wing stats only if over 0 flight time
            if (wingFlightTime > 0f)
                stats3 += this.GetLocalization("FlightTime").Format(TwoPlaces(wingFlightTime / 60f));
            stats3 += "\n" + this.GetLocalization("MiscStats").Format(
                player.aggro,
                Sign(luck) + TwoPlaces(100f * luck));
            list.FindAndReplace("[GENERIC]", stats3);

            // Detailed Abyss stats only render if the player is in the Abyss.
            string stats4 = "\n" + (!modPlayer.ZoneAbyss ? this.GetLocalizedValue("AbyssStatsHidden") : this.GetLocalization("AbyssStats").Format(
                player.GetCurrentAbyssLightLevel(),
                TwoPlaces(modPlayer.abyssBreathLossStat),
                TwoPlaces(modPlayer.abyssBreathLossRateStat),
                modPlayer.abyssLifeLostAtZeroBreathStat,
                modPlayer.abyssDefenseLossStat));
            list.FindAndReplace("[ABYSS]", stats4);

            // To save screen space, favorited tooltips do not exist for the Stat Meter
            list.RemoveAll(l => l.Mod == "Terraria" && (l.Name == "Favorite" || l.Name == "FavoriteDesc"));
        }
    }
}
