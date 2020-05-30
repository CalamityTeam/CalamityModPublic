using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class GlacialState : ModBuff
    {
        public static int DefenseReduction = 10;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Glacial State");
            Description.SetDefault("Cannot move and defense is shattered");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().gState = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.Calamity().gState < npc.buffTime[buffIndex])
				npc.Calamity().gState = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
        }
    }
}
