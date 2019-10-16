using CalamityMod.Items;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    class BrimstoneMount : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Brimrose Mount");
            Description.SetDefault("The seat is toasty.  That is all");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<PhuppersChair>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
