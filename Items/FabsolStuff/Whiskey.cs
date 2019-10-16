using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Whiskey : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Whiskey");
            Tooltip.SetDefault(@"Boosts damage and knockback by 4% and critical strike chance by 2%
Reduces defense by 8
The burning sensation makes it tastier");
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
            item.buffType = ModContent.BuffType<Whiskey>();
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 3, 30, 0);
        }
    }
}
