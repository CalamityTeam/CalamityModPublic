using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class DivineBlessCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Divine Bless Cooldown");
            Description.SetDefault("Your divine powers are recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().divineBlessCooldown = true;
        }
    }
}
