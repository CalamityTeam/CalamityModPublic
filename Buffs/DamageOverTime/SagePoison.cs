using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class SagePoison : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sage Poison");
            Description.SetDefault("Poisoned");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().sagePoisonTime < npc.buffTime[buffIndex])
                npc.Calamity().sagePoisonTime = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
