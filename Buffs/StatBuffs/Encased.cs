using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Buffs.StatBuffs
{
    public class Encased : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Encased");
            Description.SetDefault("30 defense and +30% damage reduction, but...");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().encased = true;
            if (player.buffTime[buffIndex] == 2)
            {
                SoundEngine.PlaySound(SoundID.Item27, player.position);
                player.GiveIFrames(90, true);
            }
        }
    }
}
