using CalamityMod.Items.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Mounts
{
    public class DraedonGamerChairBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Chair");
            Description.SetDefault("Riding a physics defying gamer chair");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<DraedonGamerChairMount>(), player);
            player.buffTime[buffIndex] = 10;
            player.Calamity().ExoChair = true;
        }
    }
}
