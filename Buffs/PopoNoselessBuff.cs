using CalamityMod.CalPlayer;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class PopoNoselessBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Noseless Popo");
            Description.SetDefault("Your nose has been stolen!");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.snowmanPrevious)
            {
                modPlayer.snowmanPower = true;
                modPlayer.snowmanNoseless = true;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
