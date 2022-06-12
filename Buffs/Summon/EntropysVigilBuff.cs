using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class EntropysVigilBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Entropy's Vigil");
            Description.SetDefault("The necromantic clones protect you with undying vigilance");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Calamitamini>()] > 0)
            {
                modPlayer.cEyes = true;
            }
            if (!modPlayer.cEyes)
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
