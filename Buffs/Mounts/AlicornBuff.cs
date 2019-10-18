using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items;
namespace CalamityMod.Buffs.Mounts
{
    class AlicornBuff : ModBuff
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
            player.mount.SetMount(ModContent.MountType<AlicornMount>(), player);
            player.buffTime[buffIndex] = 10;
            player.Calamity().fab = true;
        }
    }
}
