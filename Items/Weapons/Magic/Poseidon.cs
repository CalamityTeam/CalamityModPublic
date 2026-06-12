using CalamityMod.Projectiles.Magic;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Poseidon : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.damage = 52;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 30;
            Item.useAnimation = Item.useTime = 49;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.UseSound = SoundID.Item84;
            Item.rare = ItemRarityID.Pink;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PoseidonTyphoon>();
            Item.shootSpeed = 18f;
        }
    }
}
