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

        public override void Update(Player player, ref int buffIndex)
        {
			//Disables crouching with the Crouch, Crawl, and Roll Mod
            Mod crouchMod = ModLoader.GetMod("CrouchMod");
            if (crouchMod != null)
            {
				//Mod Call inputs
				//"CanCrouch"   //string which is required
				//p             //int for the player's index
				//canCrouch     //bool whether the player can input crouch (point being to set this to false)
				//forceUnCrouch //bool which, if true, forces the player to uncrouch (forcing their head into the ceiling)
                crouchMod.Call("CanCrouch", player.whoAmI, false, true);
            }
        }
    }
}
