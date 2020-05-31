using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
	public class TeslaFreeze : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Galvanic Corrosion");
            Description.SetDefault("Your limbs have begun to corrode");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.Calamity().tesla < npc.buffTime[buffIndex])
				npc.Calamity().tesla = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().teslaFreeze = true;
        }
    }
}
