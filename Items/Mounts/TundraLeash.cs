using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    class TundraLeash : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tundra Leash");
            Tooltip.SetDefault("Summons an angry dog mount");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 16;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 4;
            item.rare = 3;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.UseSound = SoundID.NPCHit56;
            item.noMelee = true;
            item.mountType = ModContent.MountType<AngryDogMount>();
        }
    }
}
