using CalamityMod.Items.Mounts.Minecarts;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Mounts
{
    public class TheCartofGodsBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<DoGCartMount>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
