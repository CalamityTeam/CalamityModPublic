using Terraria;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.StatBuffs
{
    public class AdrenalineMode : ModBuff
	{
		public static string RevTip = "150% damage boost. Can burnout down to 49.5%.";
		public static string DeathTip = "500% damage boost. Can burnout down to 165%.";

		public override void SetDefaults()
		{
			DisplayName.SetDefault("Adrenaline Mode");
			Description.SetDefault(RevTip);
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			longerExpertDebuff = false;
			canBeCleared = false;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetModPlayer<CalamityPlayer>(mod).adrenalineMode = true;
		}

		public override void ModifyBuffTip(ref string tip, ref int rare)
		{
			if (CalamityWorld.death)
				tip = DeathTip;
		}
	}
}
