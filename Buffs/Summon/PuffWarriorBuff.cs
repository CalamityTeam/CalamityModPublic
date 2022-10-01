using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class PuffWarriorBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Puff Warrior");
            Description.SetDefault("It confidently and excitedly protects you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<PuffWarrior>()] > 0)
                modPlayer.puffWarrior = true;

            if (modPlayer.puffWarrior)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
