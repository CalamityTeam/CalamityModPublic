using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class EvasionScarfCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Evasion Scarf Cooldown");
            Description.SetDefault("Your dodge is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().eScarfCooldown = true;
        }
    }
}
