using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class EyeOfNightBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Eye of Night");
            Description.SetDefault("It eerily watches");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<EyeOfNightSummon>()] > 0)
                modPlayer.eyeOfNight = true;

            if (modPlayer.eyeOfNight)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
