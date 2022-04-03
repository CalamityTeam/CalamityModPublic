using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Potions
{
    public class YharimPower : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharim's Power");
            Description.SetDefault("You feel like you can break the world in two... with your bare hands!");
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().yPower = true;
        }
    }
}
