using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.NPC;

namespace CalamityMod.Buffs.Placeables
{
    public class YellowCandleBuff : ModBuff
    {
        public static float ExtraChipDamageRatio = 0.07f;
        
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

        // Implementation is performed elsewhere using the yellowCandle bool.
        public override void Update(Player player, ref int buffIndex) => player.Calamity().yellowCandle = true;

        // Yellow Candle is implemented as a dirty modifier.
        internal static void ModifyHitInfo_Spite(ref HitInfo info)
        {
            int damageBoost = (int)(info.SourceDamage * ExtraChipDamageRatio);
            info.Damage += damageBoost;
        }
    }
}
