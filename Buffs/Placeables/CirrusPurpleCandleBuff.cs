using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Placeables
{
    public class CirrusPurpleCandleBuff : ModBuff
    {
        public static float DefenseRatioBonus = 0.15f;
        
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
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
