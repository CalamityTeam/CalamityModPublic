using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Fabsol
{
    public class RedWine : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Red Wine");
            Description.SetDefault("Life regen reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().redWine = true;
        }
    }
}
