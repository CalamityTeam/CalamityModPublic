using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.DoTDebuffs
{
    public class AbyssalFlames : ModBuff
	{
        public static int DefenseReduction = 6;

        public override void SetDefaults()
		{
			DisplayName.SetDefault("Abyssal Flames");
			Description.SetDefault("Your soul is being consumed");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().aFlames = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<CalamityGlobalNPC>(mod).aFlames = true;
		}
	}
}
