using Terraria.ModLoader.Default;

namespace CalamityMod.TileEntities
{
    /// Copied from ExampleMod for documentation if needed for later -pixl
    /// <summary>
	/// This is an empty child class that acts exactly like the default implementation of the abstract <seealso cref="TEModdedPylon"/>
	/// class, which itself acts nearly identical to vanilla pylon TEs. This inheritance only exists so that modded pylon entities
	/// will properly have their "Mod" property set, for I/O purposes. Has the sealed modifier since this TE acts identical to its parent.
	/// </summary>
    public sealed class TECalamityPylon : TEModdedPylon { }
}
