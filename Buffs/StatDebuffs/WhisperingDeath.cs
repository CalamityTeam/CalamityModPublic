using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class WhisperingDeath : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Whispering Death");
            Description.SetDefault("Death approaches; movement speed, attack power and life regen reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().wDeath = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().wDeath < npc.buffTime[buffIndex])
                npc.Calamity().wDeath = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
