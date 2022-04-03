using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class ArmorShattering : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Armor Shattering");
            Description.SetDefault("Melee and rogue attacks break enemy armor");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().armorShattering = true;
        }
    }
}
