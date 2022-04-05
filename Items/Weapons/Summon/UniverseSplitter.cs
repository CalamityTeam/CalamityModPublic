using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Items.SummonItems;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class UniverseSplitter : ModItem
    {
        public const float ItemUseDustMaxRadius = 36f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Universe Splitter");
            Tooltip.SetDefault("Summons an energy field at the mouse cursor\n" +
                               "After the field has been deployed, it begins to summon multiple small beams\n" +
                               "After several seconds have passed, an enormous laser beam appears at the field's position\n" +
                               "This effect has a cooldown\n" +
                               "Attempting to use this item during the cooldown will cause it to short circuit and do damage to you.\n" +
                               "An ancient artifact from a previous age, it waits for your command...\n" +
                               "This is a terrible idea, but it isn't yours anyways... right?");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 14));
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
            Item.Calamity().customRarity = CalamityRarity.HotPink;
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
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI);
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<DraedonsRemote>()).AddIngredient(ModContent.ItemType<Abomination>()).AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5).AddTile(ModContent.TileType<DraedonsForge>()).Register();
        }
    }
}
