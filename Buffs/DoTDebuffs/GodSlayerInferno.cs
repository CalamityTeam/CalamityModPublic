using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class GodSlayerInferno : ModBuff
    {
        public static int DefenseReduction = 10;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("God Slayer Inferno");
            Description.SetDefault("Your flesh is burning off");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().gsInferno = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().gsInferno = true;
        }
    }
}
