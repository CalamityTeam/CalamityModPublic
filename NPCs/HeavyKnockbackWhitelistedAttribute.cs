using System;

namespace CalamityMod.NPCs
{
    /// <summary>
    /// This attribute puts an NPC into the heavy knockback whitelist
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class HeavyKnockbackWhitelistedAttribute : Attribute
    {
        
    }
}
