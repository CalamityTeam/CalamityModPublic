using CalamityMod.Items.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Mounts
{
    public class RimehoundBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rimehound");
            Description.SetDefault("You are riding an angry dog");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<RimehoundMount>(), player);
            player.buffTime[buffIndex] = 10;
            player.Calamity().rimehound = true;
        }
    }
}
