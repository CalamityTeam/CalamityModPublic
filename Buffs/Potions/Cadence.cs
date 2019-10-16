using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class Cadence : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Cadance");
            Description.SetDefault("Your heart is pure");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().cadence = true;
        }
    }
}
