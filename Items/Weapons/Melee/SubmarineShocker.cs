using CalamityMod.Projectiles.Melee.Shortswords;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SubmarineShocker : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.damage = 90;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = Item.useTime = 10;
            Item.shoot = ModContent.ProjectileType<SubmarineShockerProj>();
            Item.shootSpeed = 2f;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override bool MeleePrefix() => true;
    }
}
