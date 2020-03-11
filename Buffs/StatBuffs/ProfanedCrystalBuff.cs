using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class ProfanedCrystalBuff : ModBuff 
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Devotion");
            Description.SetDefault("The profaned soul within has been defiled by the powerful magic of a supreme witch");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (CalamityWorld.downedSCal && (player.Calamity().minionSlotStat - player.slotsMinions) >= 10 && !player.Calamity().profanedCrystalForce)
            {
                player.Calamity().profanedCrystalBuffs = true;
            }
        }
    }
}
