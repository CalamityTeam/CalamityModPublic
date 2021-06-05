using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatBuffs
{
    public class Encased : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Encased");
            Description.SetDefault("30 defense and +30% damage reduction, but...");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().encased = true;
            if (player.buffTime[buffIndex] == 2)
            {
                Main.PlaySound(SoundID.Item27, player.position);
                player.immune = true;
                player.immuneNoBlink = false;
                player.immuneTime = 90;
				for (int j = 0; j < player.hurtCooldowns.Length; j++)
					player.hurtCooldowns[j] = player.immuneTime;
			}
        }
    }
}
