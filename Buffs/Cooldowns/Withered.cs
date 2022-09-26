using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class Withered : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Withered");
            Description.SetDefault("Holding withered weapons causes you to suffer but makes your weapons strong");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex) => player.Calamity().witheredDebuff = true;
    }
}
