using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.DraedonsArsenal;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class SnakeEyesBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Snake Eyes");
            Description.SetDefault("Always watching...");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SnakeEyesSummon>()] > 0)
            {
                modPlayer.snakeEyes = true;
            }
            if (!modPlayer.snakeEyes)
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
