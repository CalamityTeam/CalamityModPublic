using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
	public class RageMode : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Rage Mode");
            Description.SetDefault("50% damage boost. Can be boosted by other items up to 110%.");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().rageModeActive = true;
        }
    }
}
