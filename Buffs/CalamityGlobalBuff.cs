using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.NPCs;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
	public class CalamityGlobalBuff : GlobalBuff
    {
        public override void Update(int type, Player player, ref int buffIndex)
        {
			if (type == BuffID.Endurance)
			{
				player.endurance -= 0.05f;
			}
			else if (type == BuffID.MagicPower)
			{
				player.magicDamage -= 0.1f;
			}
			else if (type == BuffID.Panic)
			{
				player.moveSpeed -= 0.5f;
			}
			else if (type == BuffID.SugarRush)
			{
				player.moveSpeed -= 0.1f;
			}
			else if (type == BuffID.Swiftness)
			{
				player.moveSpeed -= 0.1f;
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
                player.allDamage -= nebulaDamage;
            }
            else if (type >= BuffID.NebulaUpLife1 && type <= BuffID.NebulaUpLife3)
            {
                player.lifeRegen -= 5 * player.nebulaLevelLife; // 10 to 30 changed to 5 to 15
            }
            else if (type == BuffID.Warmth)
            {
                player.buffImmune[ModContent.BuffType<GlacialState>()] = true;
                player.buffImmune[BuffID.Frozen] = true;
                player.buffImmune[BuffID.Chilled] = true;
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

        public override void ModifyBuffTip(int type, ref string tip, ref int rare)
        {
            Player player = Main.player[Main.myPlayer];

			// Vanilla buffs
            switch (type)
            {
				case BuffID.Endurance:
					tip = "5% reduced damage";
					break;

				case BuffID.MagicPower:
					tip = "10% increased magic damage";
					break;

				case BuffID.Archery:
					tip = "20% increased arrow speed and 1.05x arrow damage";
					break;

				case BuffID.Swiftness:
					tip = "15% increased movement speed";
					break;

				case BuffID.SugarRush:
					tip = "10% increased movement speed and 20% increased mining speed";
					break;

                case BuffID.NebulaUpDmg1:
                    tip = "7.5% increased damage";
                    break;

                case BuffID.NebulaUpDmg2:
                    tip = "15% increased damage";
                    break;

                case BuffID.NebulaUpDmg3:
                    tip = "22.5% increased damage";
                    break;

				case BuffID.BeetleMight1:
					tip = "Melee damage increased by 10%";
					break;

				case BuffID.BeetleMight2:
					tip = "Melee damage increased by 20%";
					break;

				case BuffID.BeetleMight3:
					tip = "Melee damage increased by 30%";
					break;

				case BuffID.BeetleEndurance1:
                    tip = "Damage taken reduced by 10%";
                    break;

                case BuffID.BeetleEndurance2:
                    tip = "Damage taken reduced by 20%";
                    break;

                case BuffID.BeetleEndurance3:
                    tip = "Damage taken reduced by 30%";
                    break;

                case BuffID.WeaponImbueVenom:
                case BuffID.WeaponImbueCursedFlames:
                case BuffID.WeaponImbueFire:
                case BuffID.WeaponImbueGold:
                case BuffID.WeaponImbueIchor:
                case BuffID.WeaponImbueNanites:
                case BuffID.WeaponImbuePoison:
                    tip = "Rogue and " + tip;
                    break;

                case BuffID.WeaponImbueConfetti:
                    tip = "All attacks cause confetti to appear";
                    break;

                case BuffID.IceBarrier:
                    tip = "Damage taken is reduced by 15%";
                    break;

                case BuffID.ChaosState:
                    tip = "Rod of Discord teleports are disabled";
                    break;

                case BuffID.CursedInferno:
					tip += ". All damage taken increased by 20%";
                    break;

                case BuffID.Warmth:
                    tip += ". Immunity to the Chilled, Frozen, and Glacial State debuffs";
                    break;

                case BuffID.Invisibility:
                    tip += ". Grants rogue bonuses while holding certain rogue weapons";
                    break;

                case BuffID.Daybreak:
                    tip = "Incinerated by solar rays";
                    break;
            }
        }
    }
}
