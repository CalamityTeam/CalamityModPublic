using CalamityMod.Balancing;
using CalamityMod.NPCs;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class CalamityGlobalBuff : GlobalBuff
    {
        public const byte ModdedFlaskEnchant = 99;

        public override void Update(int type, Player player, ref int buffIndex)
        {
            // Globally remove summon tag buff stacking
            // Other mods need to add to this list because we genuinely don't have any way to tell this
            if (CalamityBuffSets.IsSummonTagBuff[type] && buffIndex > 0)
            {
                for (int i = buffIndex; i >= 0; i--)
                {
                    if (player.buffTime[i] > 0)
                    {
                        int buffID = player.buffType[i];
                        if (CalamityBuffSets.IsSummonTagBuff[buffID] && buffID != type)
                        {
                            player.DelBuff(i);
                            break;
                        }
                    }
                }
            }

            if (type == BuffID.Archery)
            {
                player.arrowDamage *= 0.955f;
            }
            else if (type == BuffID.MagicPower)
            {
                player.GetDamage<MagicDamageClass>() -= 0.1f;
            }
            else if (type == BuffID.Clairvoyance)
            {
                player.GetDamage<MagicDamageClass>() -= 0.02f;
                player.GetCritChance<MagicDamageClass>() -= 2;
            }
            else if (type == BuffID.SugarRush)
            {
                player.moveSpeed -= 0.1f;
                player.pickSpeed += 0.1f;
            }
            else if (type == BuffID.Mining)
            {
                player.pickSpeed += 0.1f;
            }
            else if (type == BuffID.Swiftness)
            {
                player.moveSpeed -= 0.1f;
            }
            else if (type == BuffID.Tipsy)
            {
                player.statDefense += 4;
                player.GetCritChance<MeleeDamageClass>() -= 2;
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.2f;
                player.GetDamage<MeleeDamageClass>() -= 0.1f;
            }
            else if (type == BuffID.WellFed)
            {
                player.moveSpeed -= 0.15f;
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.05f;
            }
            else if (type == BuffID.WellFed2)
            {
                player.moveSpeed -= 0.225f;
                player.pickSpeed += 0.025f;
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.075f;
            }
            else if (type == BuffID.WellFed3)
            {
                player.moveSpeed -= 0.3f;
                player.pickSpeed += 0.05f;
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
            }
            else if (type == BuffID.Weak)
            {
                player.GetAttackSpeed<MeleeDamageClass>() += 0.051f;
            }
            else if (type == BuffID.Shine)
            {
                player.Calamity().shine = true;
            }

            // Star in a Bottle provides Mana Regeneration Potion effect and cancels out the usual effect if you don't have it
            else if (type == BuffID.StarInBottle && !player.manaRegenBuff)
            {
                player.manaRegenBuff = true;
                player.manaRegenDelayBonus -= 0.5f;
                player.manaRegenBonus -= 10;
            }

            // Beetle Shell DR is a full compensation, as the vanilla multiplicative DR is removed entirely.
            else if (type >= BuffID.BeetleEndurance1 && type <= BuffID.BeetleEndurance3 && player.beetleDefense)
            {
                int orbsToGrant = player.beetleOrbs < 0 ? 0 : player.beetleOrbs;
                if (orbsToGrant > 3)
                    orbsToGrant = 3;
                player.endurance += BalancingConstants.BeetleShellDRPerBeetle * orbsToGrant;
            }

            // Solar Flare DR is a full compensation, as the vanilla multiplicative DR is removed entirely.
            else if (type >= BuffID.SolarShield1 && type <= BuffID.SolarShield3)
            {
                player.endurance += BalancingConstants.SolarFlareShieldDR;
            }
            else if (type == BuffID.Rabies)
            {
                player.GetDamage<GenericDamageClass>() -= 0.2f;

                // Reimplementation of random debuff infliction; now occurs on a consistent timer and with a different debuff list
                if (player.buffTime[buffIndex] % 600 == 300)
                {
                    int debuffType = Main.rand.Next(4) switch
                    {
                        0 => BuffID.Weak,
                        1 => BuffID.Bleeding,
                        2 => BuffID.Darkness,
                        _ => BuffID.BrokenArmor,
                    };
                    player.AddBuff(debuffType, Main.rand.Next(90, 211));
                }
            }
            else if (type == BuffID.Werewolf)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.051f;
            }
        }

        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            // Globally remove summon tag debuff stacking, unless allowed in the SummonTag
            // Other mods need to add to this list because otherwise we can't tell which IsATag buff is actually for whips
            var tag1 = CalamityBuffSets.SummonTagDebuff[type];
            if (tag1 is not null && !tag1.AllowsWhipStacking && buffIndex > 0)
            {
                for (int i = buffIndex; i >= 0; i--)
                {
                    if (npc.buffTime[i] > 0)
                    {
                        int buffID = npc.buffType[i];
                        var tag2 = CalamityBuffSets.SummonTagDebuff[buffID];
                        if (tag2 is not null && !tag2.AllowsWhipStacking && buffID != type)
                        {
                            npc.DelBuff(i);
                            break;
                        }
                    }
                }
            }

            if (type == BuffID.Frozen)
            {
                npc.Calamity().frozen = true;
            }
            if (type == BuffID.Webbed)
            {
                npc.Calamity().webbed = true;
            }
            if (type == BuffID.Electrified)
            {
                npc.Calamity().electrified = true;
            }
        }

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            // Vanilla buffs (sorted alphabetically)
            switch (type)
            {
                case BuffID.Archery:
                    tip = tip.Replace("10", "5");
                    break;

                case BuffID.BeetleEndurance1:
                    tip = tip.Replace("15", "10");
                    break;

                case BuffID.BeetleEndurance2:
                    tip = tip.Replace("30", "20");
                    break;

                case BuffID.BeetleEndurance3:
                    tip = tip.Replace("45", "30");
                    break;

                case BuffID.BeetleMight1:
                    tip = CalamityUtils.GetText("Vanilla.BuffDescription.BeetleMight").Format(10, 5);
                    break;

                case BuffID.BeetleMight2:
                    tip = CalamityUtils.GetText("Vanilla.BuffDescription.BeetleMight").Format(20, 10);
                    break;

                case BuffID.BeetleMight3:
                    tip = CalamityUtils.GetText("Vanilla.BuffDescription.BeetleMight").Format(30, 15);
                    break;

                case BuffID.ChaosState:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.ChaosState");
                    break;

                case BuffID.MagicPower:
                    tip = tip.Replace("20", "10");
                    break;

                case BuffID.Mining:
                    tip = tip.Replace("25", "15");
                    break;

                case BuffID.MonsterBanner: //Vanilla's wording is unclear and nondescript
                    var tooltipLines = tip.Split("\n");
                    var result = "";
                    foreach (var tooltip in tooltipLines)
                    {
                        if (tooltip == tooltipLines[0])
                            result += CalamityUtils.GetText("Vanilla.BuffDescription.Banner");
                        else
                            result += "\n" + tooltip;
                    }
                    tip = result;
                    break;

                case BuffID.NebulaUpDmg1:
                    tip = tip.Replace("15", "7.5");
                    break;

                case BuffID.NebulaUpDmg2:
                    tip = tip.Replace("30", "15");
                    break;

                case BuffID.NebulaUpDmg3:
                    tip = tip.Replace("45", "22.5");
                    break;

                case BuffID.Rabies:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.Rabies");
                    break;

                case BuffID.SugarRush:
                    tip = tip.Replace("20", "10");
                    break;

                case BuffID.Swiftness:
                    tip = tip.Replace("25", "15");
                    break;

                case BuffID.WeaponImbueConfetti:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.WeaponImbueConfetti");
                    break;

                case BuffID.WeaponImbueCursedFlames:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.WeaponImbueCursedFlames");
                    break;

                case BuffID.WeaponImbueFire:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.WeaponImbueFire");
                    break;

                case BuffID.WeaponImbueGold:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.WeaponImbueGold");
                    break;

                case BuffID.WeaponImbueIchor:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.WeaponImbueIchor");
                    break;

                case BuffID.WeaponImbueNanites:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.WeaponImbueNanites");
                    break;

                case BuffID.WeaponImbuePoison:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.WeaponImbuePoison");
                    break;

                case BuffID.WeaponImbueVenom:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.WeaponImbueVenom");
                    break;

                case BuffID.WellFed:
                    tip = Language.GetTextValue("BuffDescription.WellFed");
                    break;

                case BuffID.WellFed2:
                    tip = Language.GetTextValue("BuffDescription.WellFed2");
                    break;

                case BuffID.WellFed3:
                    tip = Language.GetTextValue("BuffDescription.WellFed3");
                    break;
            }
        }
    }
}
