using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class HolyFlames : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Holy Flames");
            Description.SetDefault("Dissolving from holy light");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().hFlames = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().hFlames < npc.buffTime[buffIndex])
                npc.Calamity().hFlames = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
