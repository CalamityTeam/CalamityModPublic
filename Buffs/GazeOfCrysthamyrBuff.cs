using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs
{
    class GazeOfCrysthamyrBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Gaze of Crysthamyr");
            Description.SetDefault("You are riding a shadow dragon");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(mod.MountType<Items.Mounts.Crysthamyr>(), player);
            player.buffTime[buffIndex] = 10;
            player.GetModPlayer<CalamityPlayer>(mod).crysthamyr = true;
        }
    }
}
