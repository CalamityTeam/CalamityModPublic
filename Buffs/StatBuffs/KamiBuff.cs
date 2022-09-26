using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Typeless;

namespace CalamityMod.Buffs.StatBuffs
{
    public class KamiBuff : ModBuff
    {
        public const float RunSpeedBoost = 0.15f;
        public const float RunAccelerationBoost = 0.15f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Kami Injection");
            Description.SetDefault("Increased max movement speed, acceleration and damage");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().kamiBoost = true;
            if (player.buffTime[buffIndex] == 1)
                SoundEngine.PlaySound(YanmeisKnife.ExpireSound, player.Center);
        }
    }
}
