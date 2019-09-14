using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Shrines
{
    class OnyxExcavatorBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Onyx Excavator");
            Description.SetDefault("Drill");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(mod.MountType<Items.Mounts.OnyxExcavator>(), player);
            player.buffTime[buffIndex] = 10;
            player.GetCalamityPlayer().onyxExcavator = true;
        }
    }
}
