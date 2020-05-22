using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class TarragonImmunityCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Tarragon Immunity Cooldown");
            Description.SetDefault("Your immunity is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().tarragonImmunityCooldown = true;
        }
    }
}
