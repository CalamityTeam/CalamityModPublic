using CalamityMod.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class ExoFreeze : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Exo Freeze");
            Description.SetDefault("Cannot move");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().eFreeze < npc.buffTime[buffIndex])
                npc.Calamity().eFreeze = npc.buffTime[buffIndex];
            if ((CalamityLists.enemyImmunityList.Contains(npc.type) || npc.boss) && npc.Calamity().debuffResistanceTimer <= 0)
                npc.Calamity().debuffResistanceTimer = CalamityGlobalNPC.slowingDebuffResistanceMin + npc.Calamity().eFreeze;
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().eFreeze = true;
        }
    }
}
