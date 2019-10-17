using CalamityMod.CalPlayer;
using CalamityMod.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    public class YharonKindleBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Son of Yharon");
            Description.SetDefault("The Son of Yharon will protect you");
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
