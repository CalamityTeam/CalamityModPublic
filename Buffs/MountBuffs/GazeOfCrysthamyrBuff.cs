using CalamityMod.Items;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

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
            player.mount.SetMount(ModContent.MountType<Crysthamyr>(), player);
            player.buffTime[buffIndex] = 10;
            player.Calamity().crysthamyr = true;
        }
    }
}
