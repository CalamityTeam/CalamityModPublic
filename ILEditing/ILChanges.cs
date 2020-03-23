using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using static Mono.Cecil.Cil.OpCodes;

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
                var cursor = new ILCursor(il)
                {
                    Index = 45
                };
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
