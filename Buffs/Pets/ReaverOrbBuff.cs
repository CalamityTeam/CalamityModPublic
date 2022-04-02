using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Pets
{
    public class ReaverOrbBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Reaver Orb");
            Description.SetDefault("The reaver orb will light your way");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ReaverOrb>()] > 0)
            {
                modPlayer.rOrb = true;
            }
            if (!modPlayer.rOrb)
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