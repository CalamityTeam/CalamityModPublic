using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class CaribbeanRum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Caribbean Rum");
            Tooltip.SetDefault(@"Boosts life regen by 2 and movement speed and wing flight time by 20%
Makes you floaty and reduces defense by 12
Why is the rum gone?");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.rare = 4;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<Buffs.CaribbeanRum>();
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 20, 0, 0);
        }
    }
}
