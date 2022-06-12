using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class FieryDraconidBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fiery Draconid");
            Description.SetDefault("The fiery draconid will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SonOfYharon>()] > 0)
            {
                modPlayer.aChicken = true;
            }
            if (!modPlayer.aChicken)
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
