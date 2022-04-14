using CalamityMod.Buffs.StatDebuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Waraxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Waraxe");
            Tooltip.SetDefault("Critical hits cleave enemy armor, reducing their defense by 15 and protection by 25%");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.knockBack = 5.25f;
            Item.useTime = 18;
            Item.useAnimation = 22;
            Item.axe = 85 / 5;

            Item.DamageType = DamageClass.Melee;
            Item.width = 32;
            Item.height = 40;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
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
