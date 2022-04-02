using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class CrushDepth : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Crush Depth");
            Description.SetDefault("Aquatic pressure");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().cDepth = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().cDepth < npc.buffTime[buffIndex])
                npc.Calamity().cDepth = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
