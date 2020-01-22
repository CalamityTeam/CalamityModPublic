using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class SandCloakCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Sand Cloak Cooldown");
            Description.SetDefault("Your Sand Cloak is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().sandCloakCooldown = true;
        }
    }
}
