using CalamityMod.Items.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Mounts
{
    public class OnyxExcavatorBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Onyx Excavator");
            Description.SetDefault("Drill");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<OnyxExcavator>(), player);
            player.buffTime[buffIndex] = 10;
            player.Calamity().onyxExcavator = true;
        }
    }
}
