using Terraria;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Waraxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Waraxe");
            Tooltip.SetDefault("Critical hits cleave enemy armor, reducing their defense by 15 and protection by 25%");
        }

        public override void SetDefaults()
        {
            item.damage = 26;
            item.melee = true;
            item.width = 32;
            item.height = 40;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useTurn = true;
            item.axe = 10;
            item.useStyle = 1;
            item.knockBack = 5.25f;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
            {
                target.AddBuff(ModContent.BuffType<WarCleave>(), 900);
            }
        }
    }
}
