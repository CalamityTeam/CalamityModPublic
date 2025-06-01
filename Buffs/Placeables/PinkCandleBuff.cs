using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Placeables
{
    public class PinkCandleBuff : ModBuff
    {
        public static double PercentHealthPerSecond = 0.004D;
        
        public override void SetStaticDefaults()
        {
            // These settings are standard for a "opt-in eternal" buff, which has the following properties:
            // - Is not removed on death
            // - Saves with the player / is not removed on logout
            // - Does not display its time
            // - Never reduces its duration
            // - Can be manually canceled
            Main.pvpBuff[Type] = true;
            Main.persistentBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }

        // Implementation is performed elsewhere using the pinkCandle bool.
        public override void Update(Player player, ref int buffIndex) => player.Calamity().pinkCandle = true;
    }
}
