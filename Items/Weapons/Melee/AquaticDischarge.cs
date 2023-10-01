using CalamityMod.Projectiles.Melee.Shortswords;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class AquaticDischarge : ModItem, ILocalizedModType
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
            Item.damage = 23;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = Item.useTime = 18;
            Item.shoot = ModContent.ProjectileType<AquaticDischargeProj>();
            Item.shootSpeed = 2f;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
        }

        public override bool MeleePrefix() => true;
    }
}
