using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class ProfanedRageBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Profaned Rage");
            Description.SetDefault("Increased crit chance, increased movement and flight speed, and you gain more rage when damaged");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().profanedRage = true;
        }
    }
}
