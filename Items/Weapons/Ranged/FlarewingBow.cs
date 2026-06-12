using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class FlarewingBow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.OnFire3];
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 72;
            Item.damage = 31;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 1.5f;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 18f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            Vector2 arrowVel = velocity;
            arrowVel.Normalize();
            arrowVel *= 50f;
            bool arrowHitsTiles = Collision.CanHit(vector2, 0, 0, vector2 + arrowVel, 0, 0);
            for (int i = 0; i < 3; i++)
            {
                float piOffsetValue = (float)i - 1f;
                Vector2 offsetSpawn = arrowVel.RotatedBy((double)(MathHelper.Pi * 0.1f * piOffsetValue), default);
                if (!arrowHitsTiles)
                    offsetSpawn -= arrowVel;

                if (CalamityUtils.CheckWoodenAmmo(type, player))
                    type = ModContent.ProjectileType<FlareBat>();

                Projectile.NewProjectile(source, vector2 + offsetSpawn, velocity, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellwingBow).
                AddIngredient<EssenceofSunlight>(5).
                AddIngredient(ItemID.LivingFireBlock, 50).
                AddIngredient(ItemID.Obsidian, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
