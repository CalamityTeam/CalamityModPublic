using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class TemporalSadness : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Temporal Sadness");
			Description.SetDefault("You are crying");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.Calamity().tSad = true;
		}
	}
}
