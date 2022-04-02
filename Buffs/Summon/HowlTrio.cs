using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class HowlTrio : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Howl's Heart");
            Description.SetDefault("Howl protects you, Calcifer lights your way, Turnip-Head stalks you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            int howl = ModContent.ProjectileType<HowlsHeartHowl>();
            int calcifer = ModContent.ProjectileType<HowlsHeartCalcifer>();
            int turnip = ModContent.ProjectileType<HowlsHeartTurnipHead>();
            if (player.ownedProjectileCounts[howl] > 0 || player.ownedProjectileCounts[calcifer] > 0 || player.ownedProjectileCounts[turnip] > 0)
            {
                modPlayer.howlTrio = true;
            }
            if (!modPlayer.howlTrio)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }
        }
    }
}
