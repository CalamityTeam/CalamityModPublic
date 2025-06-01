using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Placeables
{
    public class PurpleCandleBuff : ModBuff
    {
        public static float DefenseRatioBonus = 0.1f;

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

        public override void Update(Player player, ref int buffIndex)
        {
            // MultipliableFloats cannot be added to or reduced.
            // To work around this, we get its current value, add what we want to that,
            // then multiply it by the ratio between the two.
            // A + B = A * ((A+B/A)
            float currentEffectiveness = player.DefenseEffectiveness.Value;
            float desiredEffectiveness = currentEffectiveness + DefenseRatioBonus;
            player.DefenseEffectiveness *= desiredEffectiveness / currentEffectiveness;
        }
    }
}
