using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class GodSlayerInferno : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Inferno");
            Description.SetDefault("Your flesh is burning off");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().gsInferno = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().gsInferno < npc.buffTime[buffIndex])
                npc.Calamity().gsInferno = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
