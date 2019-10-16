using CalamityMod.Items;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Buffs
{
    class TundraLeashBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Angry Dog");
            Description.SetDefault("You are riding an angry dog");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(ModContent.MountType<AngryDogMount>(), player);
            player.buffTime[buffIndex] = 10;
            player.Calamity().angryDog = true;
        }
    }
}
