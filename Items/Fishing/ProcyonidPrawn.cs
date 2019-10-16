using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class ProcyonidPrawn : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Procyonid Prawn");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 5);
            item.rare = 1;
        }
    }
}
