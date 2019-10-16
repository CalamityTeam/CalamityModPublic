using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BloodlettingEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodletting Essence");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 8));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 1);
            item.rare = 1;
        }
    }
}
