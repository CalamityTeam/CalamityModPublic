using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class LethalLavaBurn : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Lethal Lava Burn");
            Description.SetDefault("Losing life, reduced movement speed");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().lethalLavaBurn = true;
        }
    }
}
