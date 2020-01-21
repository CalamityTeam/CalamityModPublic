using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Cooldowns
{
    public class DraconicSurgeCooldown : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Draconic Surge Cooldown");
            Description.SetDefault("A mysterious force prevents the absorption of draconic energy");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().draconicSurgeCooldown = true;
        }
    }
}
