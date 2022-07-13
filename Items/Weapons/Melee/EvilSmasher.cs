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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 66;
            Item.scale = 2f;
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 38;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
        }

        public override float UseSpeedMultiplier(Player player) => 1f + (player.Calamity().evilSmasherBoost * 0.1f);
 
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage) => damage *= 1f + player.Calamity().evilSmasherBoost * 0.1f;

        public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback) => knockback *= 1f + (player.Calamity().evilSmasherBoost * 0.1f);

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= 0 && player.Calamity().evilSmasherBoost < 10)
                player.Calamity().evilSmasherBoost += 1;
        }
    }
}
