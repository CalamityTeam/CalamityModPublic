using CalamityMod.Buffs.Cooldowns;
using CalamityMod.Items.Materials;
using CalamityMod.Items.SummonItems;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.UI.CooldownIndicators;
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
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 14));
        }

        public override void SetDefaults()
        {
            item.width = 54;
            item.height = 76;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item122;

            item.summon = true;
            item.mana = 300;
            item.damage = 18000;
            item.knockBack = 7f;
            item.useTime = item.useAnimation = 10;
            item.shoot = ModContent.ProjectileType<UniverseSplitterField>();
            item.shootSpeed = 10f;
            item.noUseGraphic = true;

            item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                if (!player.Calamity().Cooldowns.Exists(cooldown => cooldown.GetType() == typeof(UniverseSplitterCooldown)))
                {
                    player.Calamity().Cooldowns.Add(new UniverseSplitterCooldown(45 * 60, player));
                    Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
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
                    Projectile.NewProjectile(position, Vector2.UnitY * 3f, ProjectileID.SaucerScrap, 5, 0f, player.whoAmI);
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonsRemote>());
            recipe.AddIngredient(ModContent.ItemType<Abomination>());
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
