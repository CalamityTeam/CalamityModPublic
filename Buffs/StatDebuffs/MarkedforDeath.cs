using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class MarkedforDeath : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Marked");
            Description.SetDefault("Damage reduction reduced");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().marked = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().marked = true;
        }
    }
}
