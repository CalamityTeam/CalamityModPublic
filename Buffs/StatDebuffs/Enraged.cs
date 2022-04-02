using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class Enraged : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Enraged");
            Description.SetDefault("All damage taken is increased by 25%");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().enraged = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().enraged < npc.buffTime[buffIndex])
                npc.Calamity().enraged = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
