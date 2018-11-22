using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Items;

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
            player.mount.SetMount(mod.MountType<Items.Mounts.PhuppersChair>(), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}
