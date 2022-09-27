using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.DraedonsArsenal;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class MoonFistBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moon Fist");
            Description.SetDefault("Catch these hands");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<MoonFist>()] > 0)
                modPlayer.MoonFist = true;

            if (!modPlayer.MoonFist)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
            else
                player.buffTime[buffIndex] = 18000;
        }
    }
}
