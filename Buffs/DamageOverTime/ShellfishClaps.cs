using System;
using CalamityMod.Balancing;
using CalamityMod.DataStructures;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class ShellfishClaps : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 170, //Damage per shellfish
            NPCLifeRegenMethod = ShellfishStacking
        };
        public static void ShellfishStacking(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            if (npc.SuperArmor)
                return;

            int projectileCount = 0;
            int owner = 255;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == ModContent.ProjectileType<Shellfish>() &&
                    p.ai[0] == 1f && p.ai[1] == npc.whoAmI)
                {
                    owner = p.owner;
                    projectileCount++;
                    if (projectileCount >= 5)
                    {
                        projectileCount = 5;
                        break;
                    }
                }
            }

            Item heldItem = Main.player[owner].HeldItem;
            int totalDamage = (int)Main.player[owner].GetTotalDamage<SummonDamageClass>().ApplyTo(debuffData.EnemyLostRegen);

            if (CalamityUtils.ShouldTriggerSummonPenalty(Main.player[owner], heldItem))
                totalDamage = (int)(totalDamage * BalancingConstants.SummonerCrossClassNerf);

            int totalDisplayedDamage = (int)Math.Max(totalDamage * debuffData.MultiplierDamageTickSize, debuffData.MinimumDamageTickSize);
            npc.Calamity().ApplyDPSDebuff(projectileCount * totalDamage, projectileCount * totalDisplayedDamage, ref npc.lifeRegen, ref damage);
        }
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().shellfishStaffDebuff = true;
        }
    }
}
