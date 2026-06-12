using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class StarSputter : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        private int counter = 0;
        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 26;
            Item.damage = 138;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 8;
            Item.useAnimation = 24;
            Item.reuseDelay = Item.useTime;
            Item.useLimitPerAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 15f;
            Item.value = CalamityGlobalItem.RarityCyanBuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.UseSound = SoundID.Item92;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SputterComet>();
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.FallenStar;
            Item.consumeAmmoOnFirstShotOnly = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            counter++;
            if (counter == 10)
            {
                Projectile.NewProjectile(source, position, velocity * 0.8f, ModContent.ProjectileType<SputterCometBig>(), (int)(damage * 1.5f), knockback, player.whoAmI);
            }
            if (counter >= 12)
                counter = 0;
            return true;
        }
    }
}
