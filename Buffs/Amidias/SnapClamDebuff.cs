using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.Amidias
{
    public class SnapClamDebuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Clammed");
			Description.SetDefault("Clam clap");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<CalamityGlobalNPC>(mod).clamDebuff = true;
		}
	}
}
