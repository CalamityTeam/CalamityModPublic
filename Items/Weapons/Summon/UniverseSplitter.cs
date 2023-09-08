using CalamityMod.Items.Materials;
using CalamityMod.Items.SummonItems;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class UniverseSplitter : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public const float ItemUseDustMaxRadius = 36f;
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 14));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 76;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item122;

            Item.DamageType = DamageClass.Summon;
            Item.mana = 300;
            Item.damage = 18000;
            Item.knockBack = 7f;
            Item.useTime = Item.useAnimation = 10;
            Item.shoot = ModContent.ProjectileType<UniverseSplitterField>();
            Item.shootSpeed = 10f;
            Item.noUseGraphic = true;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                if (!player.HasCooldown(Cooldowns.UniverseSplitter.ID))
                {
                    player.AddCooldown(Cooldowns.UniverseSplitter.ID, CalamityUtils.SecondsToFrames(45));
                    int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = Item.damage;
                    for (int i = 0; i < 36; i++)
                    {
                        float angle = MathHelper.TwoPi / 36f * i + Main.rand.NextFloat(MathHelper.TwoPi / 36f);
                        Dust dust = Dust.NewDustPerfect(position + angle.ToRotationVector2() * ItemUseDustMaxRadius, 247);
                        dust.velocity = Vector2.Normalize(angle.ToRotationVector2()) * 2.5f;
                        dust.noGravity = true;
                        dust.scale = 0.8f;
                    }
                }
                else
                {
                    Projectile.NewProjectile(source, position, Vector2.UnitY * 3f, ProjectileID.SaucerScrap, 5, 0f, player.whoAmI);
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Abombination>().
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
