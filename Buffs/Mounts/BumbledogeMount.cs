using CalamityMod.Items.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Mounts
{
    public class BumbledogeMount : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bumbledoge");
            Description.SetDefault("Wait a second. That's not right...");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<BUMBLEDOGE>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
