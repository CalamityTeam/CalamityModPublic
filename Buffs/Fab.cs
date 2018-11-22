using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Buffs
{
    class Fab : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Alicorn");
            Description.SetDefault("You beat DoG while drunk, you are truly fabulous!");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(mod.MountType<Items.Mounts.Fab>(), player);
            player.buffTime[buffIndex] = 10;
            player.GetModPlayer<CalamityPlayer>(mod).fab = true;
        }
    }
}
