using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Fireball : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            Tooltip.SetDefault(@"Boosts all fire-based weapon damage by 10%
Cursed flame, shadowflame, god slayer inferno, brimstone flame, and frostburn weapons will not receive this benefit
The weapon must be more fire-related than anything else
Reduces life regen by 1
A great-tasting cinnamon whiskey");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 2;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<Fireball>();
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 6, 60, 0);
        }
    }
}
