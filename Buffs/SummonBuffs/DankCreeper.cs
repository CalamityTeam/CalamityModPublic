using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.SummonBuffs
{
    public class DankCreeper : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Dank Creeper");
            Description.SetDefault("The dank creeper will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[mod.ProjectileType("DankCreeper")] > 0)
            {
                modPlayer.dCreeper = true;
            }
            if (!modPlayer.dCreeper)
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
