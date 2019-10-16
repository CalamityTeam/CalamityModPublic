using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class ScarfMeleeBoost : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Scarf Boost");
            Description.SetDefault("10% increased damage, 5% increased crit chance, and 10% increased melee speed");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().sMeleeBoost = true;
        }
    }
}
