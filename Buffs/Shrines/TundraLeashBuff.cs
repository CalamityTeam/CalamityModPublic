using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Buffs.Shrines
{
    class TundraLeashBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Angry Dog");
            Description.SetDefault("You are riding an angry dog");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(mod.MountType<Items.Mounts.AngryDog>(), player);
            player.buffTime[buffIndex] = 10;
            player.GetCalamityPlayer().angryDog = true;
        }
    }
}
