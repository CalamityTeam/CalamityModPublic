using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Dragonfire : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragonfire");
            Description.SetDefault("Engulfed by roaring flames");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().dragonFire = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().dragonFire < npc.buffTime[buffIndex])
                npc.Calamity().dragonFire = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
