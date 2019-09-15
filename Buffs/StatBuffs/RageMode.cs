using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatBuffs
{
    public class RageMode : ModBuff
	{
		public static string RevTip = "50% damage boost. Can be boosted by other items up to 110%.";
		public static string DeathTip = "170% damage boost. Can be boosted by other items up to 350%.";

		public override void SetDefaults()
		{
			DisplayName.SetDefault("Rage Mode");
			Description.SetDefault(RevTip);
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetCalamityPlayer().rageMode = true;
		}

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			if (CalamityWorld.death)
				tip = DeathTip;
		}
	}
}
