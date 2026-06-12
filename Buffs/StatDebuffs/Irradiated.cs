using System;
using CalamityMod.DataStructures;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class Irradiated : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 20, //Base irradiated DOT. This is changed with some application sources.
            NPCLifeRegenMethod = IrradiatedNPCLifeRegen
        };
        public static void IrradiatedNPCLifeRegen(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {
            var cnpc = npc.Calamity();
            int projectileCount = 0;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == ModContent.ProjectileType<WaterLeechProj>() &&
                    p.ai[0] == 1f && p.ai[1] == npc.whoAmI)
                {
                    projectileCount++;
                }
            }
            int baseIrradiatedDoTValue = (int)(cnpc.scionsCurioEffected ? (int)(debuffData.EnemyLostRegen * 1.5f) : debuffData.EnemyLostRegen);
            if (cnpc.scionsCurioEffected)
            {
                for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                {
                    Player player = Main.player[playerIndex];
                    if (player.active)
                    {
                        if (player.Calamity().scionsCurioDebuffDamage > baseIrradiatedDoTValue && player.Calamity().scionsCurio)
                        {
                            baseIrradiatedDoTValue = (int)player.Calamity().scionsCurioDebuffDamage;
                        }
                    }
                }
            }
            if (projectileCount > 0)
                cnpc.ApplyDPSDebuff(projectileCount * baseIrradiatedDoTValue, projectileCount * 4, ref npc.lifeRegen, ref damage);
            else
                cnpc.ApplyDPSDebuff(baseIrradiatedDoTValue, Math.Max((int)(baseIrradiatedDoTValue * debuffData.MultiplierDamageTickSize),debuffData.MinimumDamageTickSize), ref npc.lifeRegen, ref damage);
        }
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().irradiated = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().irradiated = true;
        }
    }
}
