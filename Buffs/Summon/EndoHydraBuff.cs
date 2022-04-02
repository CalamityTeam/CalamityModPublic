using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Summon
{
    public class EndoHydraBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Endo Hydra");
            Description.SetDefault("The endo hydra will protect you... for some reason");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            //Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (player.ownedProjectileCounts[ModContent.ProjectileType<EndoHydraBody>()] > 0)
            {
                modPlayer.endoHydra = true;
            }
            if (!modPlayer.endoHydra)
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
