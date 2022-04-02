using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EvilSmasher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evil Smasher");
            Tooltip.SetDefault("EViL! sMaSH eVIl! SmAsh... ER!\n" +
                "For every enemy you kill this hammer gains stat bonuses\n" +
                "These bonuses stack until a cap is reached\n" +
                "The bonus stacks will reset if you select a different item\n" +
                "The bonus stacks will be reduced by 1 every time you get hit");
        }

        public override void SetDefaults()
        {
            item.width = 64;
            item.height = 66;
            item.scale = 2f;
            item.damage = 100;
            item.melee = true;
            item.useAnimation = item.useTime = 38;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = ItemRarityID.LightRed;
        }

        public override float UseTimeMultiplier    (Player player) => 1f + (player.Calamity().evilSmasherBoost * 0.1f);

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat) => mult += player.Calamity().evilSmasherBoost * 0.1f;

        public override void GetWeaponKnockback(Player player, ref float knockback) => knockback *= 1f + (player.Calamity().evilSmasherBoost * 0.1f);

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0 && player.Calamity().evilSmasherBoost < 10)
                player.Calamity().evilSmasherBoost += 1;
        }
    }
}
