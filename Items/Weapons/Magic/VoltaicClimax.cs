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
    [LegacyName("Climax")]
    public class VoltaicClimax : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Electrified];
        }

        public override void SetDefaults()
        {
            Item.width = 78;
            Item.height = 78;
            Item.damage = 215;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 45;
            Item.useTime = Item.useAnimation = 34;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ClimaxProj>();
            Item.shootSpeed = 12f;
            Item.rare = ModContent.RarityType<CosmicPurple>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int numOrbs = 9;
            Vector2 clickPos = player.ClampedMouseWorld();
            float orbSpeed = 14f;
            Vector2 vel = Main.rand.NextVector2CircularEdge(orbSpeed, orbSpeed);
            for (int i = 0; i < numOrbs; i++)
            {
                float timingStagger = i * 2;
                Projectile.NewProjectile(source, clickPos, vel, type, damage, knockback, player.whoAmI, timingStagger);

                vel = vel.RotatedBy(MathHelper.TwoPi / numOrbs);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MagneticMeltdown>().
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<DarksunFragment>(8).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
