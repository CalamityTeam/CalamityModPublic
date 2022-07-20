using CalamityMod.Items.Mounts;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Armor.MarniteArchitect;

namespace CalamityMod.Buffs.Mounts
{
    public class MarniteLiftBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marnite Lift");
            Description.SetDefault("Do you even..?");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<MarniteLift>(), player);
            player.buffTime[buffIndex] = 10; // reset buff time
            player.GetModPlayer<MarniteArchitectPlayer>().mounted = true;
        }
    }
}
