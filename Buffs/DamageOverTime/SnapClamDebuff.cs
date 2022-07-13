using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class SnapClamDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clammed");
            Description.SetDefault("Clapped by Snap Clams");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().clamDebuff < npc.buffTime[buffIndex])
                npc.Calamity().clamDebuff = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
