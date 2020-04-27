using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class EnergyShellCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Energy Shell Cooldown");
            Description.SetDefault("Your energy shell is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().energyShellCooldown = true;
        }
    }
}
