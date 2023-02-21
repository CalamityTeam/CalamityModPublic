using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class MiracleBlight : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miracle Blight");
            Description.SetDefault("Exotic resonance shreds your corporeal form");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().miracleBlight = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.Calamity().miracleBlight < npc.buffTime[buffIndex])
                npc.Calamity().miracleBlight = npc.buffTime[buffIndex];
            npc.DelBuff(buffIndex);
            buffIndex--;
        }
    }
}
