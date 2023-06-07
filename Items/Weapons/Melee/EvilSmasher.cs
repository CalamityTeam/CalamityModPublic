using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EvilSmasher : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 66;
            Item.scale = 2f;
            Item.damage = 120;
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

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0 && player.Calamity().evilSmasherBoost < 10)
                player.Calamity().evilSmasherBoost += 1;
        }
    }
}
