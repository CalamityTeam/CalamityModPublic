using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.DraedonsArsenal;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class StarSwallowerBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Star Swallower");
            Description.SetDefault("Mmmm myes");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<StarSwallowerSummon>()] > 0)
            {
                modPlayer.starSwallowerPetFroge = true;
            }
            if (!modPlayer.starSwallowerPetFroge)
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
