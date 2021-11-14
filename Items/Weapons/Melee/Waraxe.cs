using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
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
            item.knockBack = 5.25f;
            item.useTime = 18;
            item.useAnimation = 22;
            item.axe = 85 / 5;

            item.melee = true;
            item.width = 32;
            item.height = 40;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
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

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (crit)
            {
                target.AddBuff(ModContent.BuffType<WarCleave>(), 900);
            }
        }
    }
}
