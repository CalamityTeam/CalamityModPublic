using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class FleshTotemCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Flesh Totem Cooldown");
            Description.SetDefault("The Flesh Totem effect is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().fleshTotemCooldown = true;
        }
    }
}
