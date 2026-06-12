using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class ArterialAssault : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        int shotNum = 0;
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 100;
            Item.damage = 256;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 6;
            Item.useAnimation = 40;
            Item.reuseDelay = 10;
            Item.useLimitPerAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4.25f;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item102;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 30f;
            Item.useAmmo = AmmoID.Arrow;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rotateBy = 0.75f * ((shotNum*3) % 5 - 2);
            position += velocity.SafeNormalize(Vector2.Zero).RotatedBy(rotateBy) * 64;
            velocity = position.DirectionTo(Main.MouseWorld) * velocity.Length()*2;
            type = ModContent.ProjectileType<BloodfireArrowProj>();
            Projectile shotArrow = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            shotArrow.tileCollide = false;
            (shotArrow.ModProjectile as BloodfireArrowProj).DisableEffects = true;
            shotArrow.Calamity().conditionalHomingRange = 175f;
            shotArrow.Calamity().BloodstoneOrbValue = 15;
            shotNum++;
            if (shotNum > 4) shotNum = 0;
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
