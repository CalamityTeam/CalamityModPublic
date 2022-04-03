using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Mounts
{
    public class AndromedaBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Andromeda");
            Description.SetDefault("You're controlling a piece of history");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // Disables crouching with the Crouch, Crawl, and Roll Mod
            Mod crouchMod = CalamityMod.Instance.crouchMod;
            if (crouchMod != null)
            {
                // Mod Call inputs
                // 1) "CanCrouch" -- name of operation
                // 2) int for the player's index
                // 3) bool for whether the player can use the crouch input this frame
                // 4) bool for whether the player should be force-uncrouched this frame
                crouchMod.Call("CanCrouch", player.whoAmI, false, true);
            }
        }
    }
}
