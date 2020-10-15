using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
	public class KamiDebuff : ModBuff
    {
        public const float MultiplicativeDamageReduction = 0.8f;
        // Hard-cap for npc speed when afflicted with this debuff. Does not affect certain NPCs and does not affect any bosses (Basically only works on boss minions).
        public const float MaxNPCSpeed = 16f;
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Kami Flu");
            Description.SetDefault("Defenseless and dying");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.Calamity().kamiFlu < npc.buffTime[buffIndex])
				npc.Calamity().kamiFlu = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().teslaFreeze = true;
        }
    }
}
