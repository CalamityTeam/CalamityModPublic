using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class PearlAura : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pearl Aura");
            Description.SetDefault("Slowed down");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().pearlAura < npc.buffTime[buffIndex])
                npc.Calamity().pearlAura = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
