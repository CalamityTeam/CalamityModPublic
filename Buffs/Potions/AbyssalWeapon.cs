using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class AbyssalWeapon : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyssal Weapon");
            Description.SetDefault("Melee and rogue weapons inflict abyssal flames, 5% increased movement speed");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().aWeapon = true;
        }
    }
}
