namespace CalamityMod.Cooldowns
{
    // This base version of Cooldown is never directly used. It is necessary due to C# language constraints.
    // All actual cooldowns are the below type, Cooldown<T>.
    public class Cooldown
    {
        // Do not ever change this value. It is set when a cooldown is registered.
        internal readonly ushort netID;

        /// <summary>
        /// Unique string ID of the cooldown. You can set this to whatever you want.<br/>
        /// It is recommended that this ID be defined statically in your CooldownHandler implementation.
        /// </summary>
        public readonly string ID = "";

        internal Cooldown(string id, ushort nid)
        {
            ID = id;
            netID = nid;
        }
    }
    
    // Cooldowns are nothing other than identifiers. They serve as a minimal identification interface that is netcode compatible.
    // Everything else is handled by one of the following two types:
    // - CooldownInstance (if a player has a cooldown, this is what they have)
    // - CooldownHandler (every CooldownInstance has one: it handles the behavior of that cooldown instance)
    //
    // Individual "types" of cooldowns from a content sense are registered with a Handler.
    // Every cooldown MUST define a Handler to be registered and functional.
    // The differences between all the cooldowns are implemented as various subclasses of CooldownHandler.
    public class Cooldown<T> : Cooldown where T : CooldownHandler
    {
        internal Cooldown(string id, ushort nid) : base(id, nid) {}
    }
}
