using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.DoTDebuffs
{
    public class BrimstoneFlames : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Brimstone Flames");
			Description.SetDefault("Rapid health loss");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).bFlames = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<CalamityGlobalNPC>(mod).bFlames = true;
		}
	}
}
