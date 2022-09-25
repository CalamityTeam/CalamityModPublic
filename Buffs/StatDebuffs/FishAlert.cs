using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class FishAlert : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fish Alert");
            Description.SetDefault("The abyssal creatures have spotted you!");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().fishAlert = true;
        }
    }
}
