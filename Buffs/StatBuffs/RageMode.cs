using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Buffs.StatBuffs
{
    public class RageMode : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rage Mode");
            Description.SetDefault("35% damage boost.");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true; // Because duration is variable, time is not displayed.
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer mp = player.Calamity();

            // If the player still has Rage left to burn, the buff stays active indefinitely.
            if (mp.rage > 0f)
            {
                player.buffTime[buffIndex] = 2; // Every frame, give another frame for the buff to live.
                mp.rageModeActive = true;
            }

            // Otherwise, Rage Mode ends instantly.
            else
            {
                SoundEngine.PlaySound(Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AbilitySounds/RageEnd"), player.position);
                player.DelBuff(buffIndex--); // TML documentation requires you to decrement buffIndex if deleting the buff during Update.
                mp.rageModeActive = false;
                mp.rage = 0f;
            }
        }
    }
}
