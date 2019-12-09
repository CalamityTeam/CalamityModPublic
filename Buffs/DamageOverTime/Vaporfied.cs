using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class Vaporfied : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Vaporfied");
            Description.SetDefault("Vape");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			npc.Calamity().vaporfied = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
		}
    }
}
