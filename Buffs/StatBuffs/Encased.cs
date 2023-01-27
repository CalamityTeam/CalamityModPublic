using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().encased = true;
            if (player.buffTime[buffIndex] == 2)
            {
                SoundEngine.PlaySound(SoundID.Item27, player.Center);
                player.GiveIFrames(90, true);
            }
        }
    }
}
