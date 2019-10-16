using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class MoscowMule : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moscow Mule");
            Tooltip.SetDefault(@"Boosts damage and knockback by 9% and critical strike chance by 3%
Reduces life regen by 2
I once heard the copper mug can be toxic and I told 'em 'listen dummy, I'm already poisoning myself'");
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
            item.buffType = ModContent.BuffType<MoscowMule>();
            item.buffTime = 18000; //5 minutes
            item.value = Item.buyPrice(0, 16, 60, 0);
        }
    }
}
