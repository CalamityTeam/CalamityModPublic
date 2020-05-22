using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class BrimflameFrenzyCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Brimflame Frenzy Cooldown");
            Description.SetDefault("Your use of brimstone magic has left you exhausted");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().brimflameFrenzyCooldown = true;
        }
    }
}
