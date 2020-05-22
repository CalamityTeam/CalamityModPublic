using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class BloodflareBloodFrenzyCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Blood Frenzy Cooldown");
            Description.SetDefault("Your blood frenzy is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().bloodFrenzyCooldown = true;
        }
    }
}
