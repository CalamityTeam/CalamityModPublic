using CalamityMod.Items.Materials;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    [LegacyName("GreatbowofTurmoil")]
    public class ContinentalGreatbow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Ichor, BuffID.OnFire];
        }
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 36;
            Item.damage = 45;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 17f;
            Item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo spawnSource, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 source = player.RotatedRelativePoint(player.MountedCenter, true);
            float piOverTen = MathHelper.Pi * 0.1f;
            int arrowAmt = 3;

            velocity.Normalize();
            velocity *= 40f;
            bool canHit = Collision.CanHit(source, 0, 0, source + velocity, 0, 0);
            for (int projIndex = 0; projIndex < arrowAmt; projIndex++)
            {
                float offsetAmt = projIndex - (arrowAmt - 1f) / 2f;
                Vector2 offset = velocity.RotatedBy((double)(piOverTen * offsetAmt), default);
                if (!canHit)
                    offset -= velocity;

                if (CalamityUtils.CheckWoodenAmmo(type, player))
                    type = ProjectileID.FireArrow;

                Projectile.NewProjectile(spawnSource, source + offset, velocity, type, damage, knockback, player.whoAmI);
            }
            for (int i = 0; i < 2; i++)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-10, 11) * 0.05f;
                type = Utils.SelectRandom(Main.rand, new int[]
                {
                    ProjectileID.HellfireArrow,
                    ProjectileID.IchorArrow
                });
                Projectile arrow = Projectile.NewProjectileDirect(spawnSource, position, new Vector2(SpeedX, SpeedY), type, (int)(damage * 0.5f), knockback, player.whoAmI);
                arrow.usesLocalNPCImmunity = true;
                arrow.localNPCHitCooldown = 10;
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ScoriaBar>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
