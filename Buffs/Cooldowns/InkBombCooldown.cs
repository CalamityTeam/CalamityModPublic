using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class InkBombCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Ink Bomb Cooldown");
            Description.SetDefault("Your Ink Bomb is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().inkBombCooldown = true;
        }
    }
}
