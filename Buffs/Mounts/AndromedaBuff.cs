using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Mounts
{
	public class AndromedaBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Andromeda");
            Description.SetDefault("You're controlling a piece of history");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }
    }
}
