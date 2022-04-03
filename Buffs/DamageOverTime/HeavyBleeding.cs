using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.DamageOverTime
{
    public class HeavyBleeding : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heavy Bleeding");
            Description.SetDefault("You're losing a lot of blood");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex) => player.Calamity().waterLeechBleeding = true;
    }
}
