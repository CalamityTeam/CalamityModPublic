using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.DoTDebuffs
{
    public class BurningBlood : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Burning Blood");
			Description.SetDefault("Your blood is on fire");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().bBlood = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<CalamityGlobalNPC>(mod).bBlood = true;
		}
	}
}
