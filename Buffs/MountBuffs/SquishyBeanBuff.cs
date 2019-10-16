using CalamityMod.Items;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

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
