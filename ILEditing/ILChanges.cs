using MonoMod.Cil;
using System;
using Terraria;

namespace CalamityMod.ILEditing
{
    public class ILChanges
    {
        /// <summary>
        /// Loads all IL Editing changes in the mod.
        /// </summary>
        public static void Initialize()
        {
            IL.Terraria.WorldGen.MakeDungeon += (il) =>
            {
                var cursor = new ILCursor(il);
                if (!cursor.TryGotoNext(i => i.MatchStsfld("Terraria.WorldGen", "dMaxY")))
                {
                    CalamityMod.Instance.Logger.Warn("Dungeon movement editing code failed.");
                    return;
                }
                cursor.Index++;
                cursor.EmitDelegate<Action>(() =>
                {
                    WorldGen.dungeonX += (WorldGen.dungeonX < Main.maxTilesX / 2).ToDirectionInt() * 450;

                    if (WorldGen.dungeonX < 200)
                        WorldGen.dungeonX = 200;
                    if (WorldGen.dungeonX > Main.maxTilesX - 200)
                        WorldGen.dungeonX = Main.maxTilesX - 200;
                });
            };
        }
    }
}
