using MonoMod.Cil;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        /// <summary>
        /// Moves an <see cref="ILCursor"/> to the final Ret call in the method the cursor is assocated with.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        public static ILCursor GotoFinalRet(this ILCursor cursor)
        {
            while (cursor.TryGotoNext(c => c.MatchRet())) { }
            return cursor;
        }
    }
}
