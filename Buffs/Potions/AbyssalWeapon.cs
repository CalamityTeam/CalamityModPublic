using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class AbyssalWeapon : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Weapon Imbue: Brimstone Flames");
            Description.SetDefault("Rogue and Melee attacks inflict enemies with brimstone flames");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.meleeBuff[Type] = true;
            Main.persistentBuff[Type] = true;
            BuffID.Sets.IsAFlaskBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().aWeapon = true;
            // A very large number to indicate it's a modded Flask
            player.meleeEnchant = 99;
        }
    }
}
