using CalamityMod.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class GlacialState : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glacial State");
            Description.SetDefault("Cannot move");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().gState = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().gState < npc.buffTime[buffIndex])
                npc.Calamity().gState = npc.buffTime[buffIndex];
            if ((CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss) && npc.Calamity().debuffResistanceTimer <= 0)
                npc.Calamity().debuffResistanceTimer = CalamityGlobalNPC.slowingDebuffResistanceMin + npc.Calamity().gState;
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
