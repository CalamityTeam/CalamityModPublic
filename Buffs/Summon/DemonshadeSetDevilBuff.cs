using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class DemonshadeSetDevilBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Red Devil");
            Description.SetDefault("The red devil will protect you");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<DemonshadeRedDevil>()] > 0)
            {
                modPlayer.rDevil = true;
            }
            if (!modPlayer.rDevil)
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
