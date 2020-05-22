using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class BloodflareSoulCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Bloodflare Soul Cooldown");
            Description.SetDefault("Your souls are recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().bloodflareSoulCooldown = true;
        }
    }
}
