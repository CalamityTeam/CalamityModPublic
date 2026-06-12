using CalamityMod.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class SnapClamDebuff : ModBuff
    {
        public static DebuffData debuffData = new DebuffData()
        {
            EnemyLostRegen = 15, //Damage per shellfish
            NPCLifeRegenMethod = ShellfishStacking
        };
        public static void ShellfishStacking(NPC npc, int buffType, ref int buffIndex, ref int damage)
        {

            int projectileCount = 0;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.ai[0] == 1f && p.ai[1] == npc.whoAmI)
                {
                    if (p.type == ModContent.ProjectileType<SnapClamProj>())
                        projectileCount += 2;
                    if (p.type == ModContent.ProjectileType<SnapClamStealth>())
                        projectileCount++;
                }
            }

            npc.Calamity().ApplyDPSDebuff((int)(projectileCount * debuffData.EnemyLostRegen), projectileCount * 3, ref npc.lifeRegen, ref damage);
        }
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            CalamityBuffSets.DebuffDataset[Type] = debuffData;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().snapClamDebuff = true;
        }
    }
}
