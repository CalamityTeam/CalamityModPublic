using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class WarCleave : ModBuff
    {
        public static int DefenseReduction = 15;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("War Cleave");
            Description.SetDefault("Defense and protection reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().wCleave < npc.buffTime[buffIndex])
                npc.Calamity().wCleave = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().wCleave = true;
        }
    }
}
