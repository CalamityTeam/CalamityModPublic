using Terraria;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.DoTDebuffs
{
    public class AstralInfectionDebuff : ModBuff
	{
        public static int DefenseReduction = 8;

        public override void SetDefaults()
		{
			DisplayName.SetDefault("Astral Infection");
			Description.SetDefault("Your flesh is melting off");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().astralInfection = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			npc.Calamity().astralInfection = true;
		}
	}
}
