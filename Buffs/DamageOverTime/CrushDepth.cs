using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class CrushDepth : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crush Depth");
            Description.SetDefault("Aquatic pressure");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
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
