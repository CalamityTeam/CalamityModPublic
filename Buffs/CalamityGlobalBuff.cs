using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
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
            if (type == BuffID.Shine)
            {
                player.Calamity().shine = true;
            }
            else if (type == BuffID.IceBarrier)
            {
                player.endurance -= 0.1f;
            }
            else if (type == BuffID.ObsidianSkin)
            {
                player.lavaMax += 420;
            }
            else if (type == BuffID.Rage)
            {
                player.Calamity().throwingCrit += 10;
            }
            else if (type == BuffID.WellFed)
            {
                player.Calamity().throwingCrit += 2;
            }
            else if (type >= BuffID.NebulaUpDmg1 && type <= BuffID.NebulaUpDmg3)
            {
                float nebulaDamage = 0.075f * (float)player.nebulaLevelDamage; //7.5% to 22.5%
                player.allDamage -= nebulaDamage;
            }
            else if (type >= BuffID.NebulaUpLife1 && type <= BuffID.NebulaUpLife3)
            {
                player.lifeRegen -= 5 * player.nebulaLevelLife; //10 to 30 changed to 5 to 15
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
				npc.DelBuff(buffIndex);
				buffIndex--;
			}
            else if (type == BuffID.Slow)
            {
                if (npc.Calamity().slowed < npc.buffTime[buffIndex])
                    npc.Calamity().slowed = npc.buffTime[buffIndex];
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
            if (type == BuffID.NebulaUpDmg1)
                tip = "7.5% increased damage";
            else if (type == BuffID.NebulaUpDmg2)
                tip = "15% increased damage";
            else if (type == BuffID.NebulaUpDmg3)
                tip = "22.5% increased damage";
            else if (type == BuffID.WeaponImbueVenom || type == BuffID.WeaponImbueCursedFlames || type == BuffID.WeaponImbueFire || type == BuffID.WeaponImbueGold ||
                type == BuffID.WeaponImbueIchor || type == BuffID.WeaponImbueNanites || type == BuffID.WeaponImbuePoison)
                tip = "Rogue and " + tip;
            else if (type == BuffID.WeaponImbueConfetti)
                tip = "All attacks cause confetti to appear";
            else if (type == BuffID.IceBarrier)
                tip = "Damage taken is reduced by 15%";
            else if (type == BuffID.ChaosState)
                tip = "Rod of Discord teleports are disabled";
            else if (type == BuffID.Ichor)
            {
                tip = "Defense reduced by 20";
                if (CalamityWorld.revenge)
                    tip += ". All damage taken increased by 25%";
            }
            else if (type == BuffID.CursedInferno && CalamityWorld.revenge)
                tip += ". All damage taken increased by 20%";
            else if (type == BuffID.Warmth)
			{
                tip += ". Immunity to the Chilled, Frozen, and Glacial State debuffs";
				if (CalamityWorld.death)
					tip += ". Provides cold protection in Death Mode";
			}
            else if (type == BuffID.Invisibility)
                tip += ". Grants rogue bonuses while holding certain rogue weapons";
			else if (type == BuffID.ObsidianSkin && CalamityWorld.death)
				tip += ". Provides heat protection in Death Mode";
			else if (type == BuffID.Inferno && CalamityWorld.death)
				tip += ". Provides cold protection in Death Mode";
			else if (type == BuffID.Campfire && CalamityWorld.death)
				tip += ". Provides cold protection in Death Mode";
			else if (type == ModContent.BuffType<Molten>() && CalamityWorld.death)
				tip += ". Provides cold protection in Death Mode";
            else if (type == ModContent.BuffType<ProfanedBabs>())
            {
                Player player = Main.player[Main.myPlayer];
                bool offense = player.Calamity().gOffense;
                bool defense = player.Calamity().gDefense;
                if (player.Calamity().profanedCrystal && (!player.Calamity().profanedCrystalBuffs && !CalamityWorld.downedSCal))
                {
                    tip = "The Profaned Babs will accompany you!";
                }
                else if (offense && defense)
                {
                    tip = "The Profaned Babs will fight for and defend you!";
                }
                else if (offense || defense)
                {
                    tip = "The " + (offense ? "Offensive" : "Defensive") + " Duo will " + (offense ? "fight for and heal you!" : "protect and heal you!");
                }
            }
            else if (type == ModContent.BuffType<ProfanedCrystalBuff>())
            {
                if (Main.player[Main.myPlayer].Calamity().profanedCrystalBuffs)
                {
                    if (Main.player[Main.myPlayer].Calamity().endoCooper)
                    {
                        tip = "An ascended ice construct is suppressing your true potential..";
                    }
                    else if (Main.player[Main.myPlayer].Calamity().magicHat)
                    {
                        tip = "A magical hat overwhelms your senses, squandering your true potential..";
                    }
                    else
                    {
                        Player player = Main.player[Main.myPlayer];
                        bool offense = (Main.dayTime && !player.wet) || player.lavaWet;
                        bool enrage = player.statLife <= (int)((double)player.statLifeMax2 * 0.5);
                        tip = "You are an emissary of the profaned goddess now!\n" +
                            (offense ? "The " + (Main.dayTime ? "light of the sun" : "heat of the lava") + " empowers your offensive capabilities" : 
                            "The " + (player.wet ? (player.honeyWet ? "honey cools" : "water douses") : "darkness of night cools") + " your flames, empowering your defensive capabilities") +
                            (enrage ? "\nYour weakened life force fuels your desperate attacks" : ""); 
                    }
                }
                else if (CalamityWorld.downedSCal)
                {
                    tip = "Your profaned soul is constrained by your insufficient summoning powers";
                }
            }
        }
    }
}
