using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    public class ArmorCrunch : ModBuff
    {
        public static int DefenseReduction = 15;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Armor Crunch");
            Description.SetDefault("Your armor is shredded");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.Calamity().aCrunch = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().aCrunch = true;
        }
    }
}
