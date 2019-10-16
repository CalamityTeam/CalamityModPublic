using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class AbyssalMirrorCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Abyssal Evade Cooldown");
            Description.SetDefault("Your Abyssal Mirror's dodge is recharging");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().abyssalMirrorCooldown = true;
        }
    }
}
