using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class HeartAttack : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Absolute Rage");
            Description.SetDefault("Your anger has made you more durable. Boosts max life by 5%.");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().hAttack = true;
        }
    }
}
