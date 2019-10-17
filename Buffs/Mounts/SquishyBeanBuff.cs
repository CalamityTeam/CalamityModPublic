using CalamityMod.Items;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    class SquishyBeanBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Squishy Bean");
            Description.SetDefault("BEAN MAN. BEAN DO T H E  B EA N IS HER E");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<SquishyBean>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
