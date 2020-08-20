using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class BloodyBoost : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Bloody Boost");
            Description.SetDefault("Increased offensive and defensive stats\n" +
			"Healing potions grant more health");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

		public override void Update(Player player, ref int buffIndex)
		{
			player.Calamity().bloodPactBoost = true;
		}
	}
}
