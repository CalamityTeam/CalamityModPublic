using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class Eutrophication : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Eutrophication");
            Description.SetDefault("Excessive nutrients restrict your movement");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.Calamity().eutrophication < npc.buffTime[buffIndex])
				npc.Calamity().eutrophication = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().eutrophication = true;
        }
    }
}
