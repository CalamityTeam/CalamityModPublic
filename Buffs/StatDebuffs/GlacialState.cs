using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class GlacialState : ModBuff
	{
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
			player.GetCalamityPlayer().gState = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<CalamityGlobalNPC>(mod).gState = true;
		}
	}
}
