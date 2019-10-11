using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.Permafrost
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
            player.statDefense += 30;
            player.endurance += 0.3f;
            player.frozen = true;
            player.velocity.X = 0f;
            player.velocity.Y = -0.4f; //should negate gravity

            int d = Dust.NewDust(player.position, player.width, player.height, 88);
            Main.dust[d].noGravity = true;
            Main.dust[d].velocity *= 2f;

            if (player.buffTime[buffIndex] == 2)
            {
                Main.PlaySound(SoundID.Item27, player.position);
                player.immune = true;
                player.immuneNoBlink = false;
                player.immuneTime = 90;
            }
        }
    }
}
