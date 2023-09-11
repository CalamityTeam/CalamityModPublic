using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.NPCs;
using System.Text;
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
            if (type == BuffID.Archery)
            {
                player.arrowDamage *= 0.955f;
            }
            else if (type == BuffID.Ironskin)
            {
                player.statDefense += CalamityUtils.GetScalingDefense(-1) - 8;
            }
            else if (type == BuffID.MagicPower)
            {
                player.GetDamage<MagicDamageClass>() -= 0.1f;
            }
            else if (type == BuffID.Panic)
            {
                player.moveSpeed -= 0.6f;
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
            else if (type == BuffID.Shine)
            {
                player.Calamity().shine = true;
            }
            else if (type == BuffID.IceBarrier)
            {
                player.endurance -= 0.1f;
            }
            else if (type >= BuffID.NebulaUpDmg1 && type <= BuffID.NebulaUpDmg3)
            {
                float nebulaDamage = 0.075f * player.nebulaLevelDamage; // 15% to 45% changed to 7.5% to 22.5%
                player.GetDamage<GenericDamageClass>() -= nebulaDamage;
            }
            else if (type == BuffID.Warmth)
            {
                player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
                player.buffImmune[BuffID.Frozen] = true;
                player.buffImmune[BuffID.Chilled] = true;
            }
            else if (type == BuffID.Rabies)
            {
                player.GetDamage<GenericDamageClass>() -= 0.2f;
            }
            else if (type == BuffID.BeetleMight1)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.05f;
            }
            else if (type == BuffID.BeetleMight2)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.1f;
            }
            else if (type == BuffID.BeetleMight3)
            {
                player.GetAttackSpeed<MeleeDamageClass>() -= 0.15f;
            }
        }

        public override void Update(int type, NPC npc, ref int buffIndex)
        {
            if (type == BuffID.Webbed)
            {
                if (npc.Calamity().webbed < npc.buffTime[buffIndex])
                    npc.Calamity().webbed = npc.buffTime[buffIndex];
                if ((CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss) && npc.Calamity().debuffResistanceTimer <= 0)
                    npc.Calamity().debuffResistanceTimer = CalamityGlobalNPC.slowingDebuffResistanceMin + npc.Calamity().webbed;
                npc.DelBuff(buffIndex);
                buffIndex--;
            }
            else if (type == BuffID.Slow)
            {
                if (npc.Calamity().slowed < npc.buffTime[buffIndex])
                    npc.Calamity().slowed = npc.buffTime[buffIndex];
                if ((CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss) && npc.Calamity().debuffResistanceTimer <= 0)
                    npc.Calamity().debuffResistanceTimer = CalamityGlobalNPC.slowingDebuffResistanceMin + npc.Calamity().slowed;
                npc.DelBuff(buffIndex);
                buffIndex--;
            }
            if (type == BuffID.Electrified)
            {
                if (npc.Calamity().electrified < npc.buffTime[buffIndex])
                    npc.Calamity().electrified = npc.buffTime[buffIndex];
                npc.DelBuff(buffIndex);
                buffIndex--;
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

                case BuffID.IceBarrier:
                    tip = tip.Replace("25", "15");
                    break;

                case BuffID.Ironskin:
                    tip = tip.Replace("8", CalamityUtils.GetScalingDefense(-1).ToString());
                    break;

                case BuffID.LeafCrystal:
                    tip = CalamityUtils.GetTextValue("Vanilla.BuffDescription.LeafCrystal");
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

                case BuffID.SugarRush:
                    tip = tip.Replace("20", "10");
                    break;

                case BuffID.Swiftness:
                    tip = tip.Replace("25", "15");
                    break;

                case BuffID.Warmth:
                    tip += "\n" + CalamityUtils.GetTextValue("Vanilla.BuffDescription.WarmthExtra");
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
