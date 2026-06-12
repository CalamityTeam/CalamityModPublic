using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using CalamityMod.Systems.Collections;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class VoidVortex : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<AuricRebuke>()];
        }

        public override void SetDefaults()
        {
            Item.width = 130;
            Item.height = 130;
            Item.damage = 210;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 70;
            Item.useAnimation = Item.useTime = 49;
            Item.knockBack = 0.25f;
            Item.shoot = ModContent.ProjectileType<VoidVortexProj>();
            Item.shootSpeed = 12f;

            Item.UseSound = SoundID.Item20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.noMelee = true;

            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<BurnishedAuric>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numOrbs = 12;
            Vector2 clickPos = player.ClampedMouseWorld();
            float orbDistance = 90f;
            float orbSpeed = 4f;

            float spinCoinflip = Main.rand.NextBool() ? -1f : 1f;
            Vector2 dir = Main.rand.NextVector2Unit();
            for (int i = 0; i < numOrbs; i++)
            {
                Vector2 orbPos = clickPos + dir * orbDistance;
                Vector2 vel = dir.RotatedBy(spinCoinflip * -MathHelper.PiOver2) * orbSpeed;

                Projectile.NewProjectile(source, orbPos, -vel, type, damage, knockback, player.whoAmI, i * 4, spinCoinflip);
                dir = dir.RotatedBy(MathHelper.TwoPi / numOrbs);
            }
            // Big orb
            Projectile.NewProjectile(source, clickPos, Vector2.Zero, type, damage * 4, 10, player.whoAmI, 0, spinCoinflip, 1);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VoltaicClimax>().
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
