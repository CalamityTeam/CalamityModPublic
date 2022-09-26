using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class DemonFlames : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Demon Flames");
            Description.SetDefault("Another burning debuff");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().dFlames < npc.buffTime[buffIndex])
                npc.Calamity().dFlames = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
