using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class EndoHydraStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Endo Hydra Staff");
            Tooltip.SetDefault("Summons a frigid entity with a head\n" +
                               "If the entity already exists, using this item again will cause it to gain more heads");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.height = 60;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.UseSound = SoundID.Item60;
            item.summon = true;
            item.mana = 25;
            item.damage = 450;
            item.knockBack = 3f;
            item.autoReuse = true;
            item.useTime = item.useAnimation = 10;
            item.shoot = ModContent.ProjectileType<EndoHydraBody>();
            item.shootSpeed = 10f;

            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                bool bodyExists = false;
                int bodyIndex = -1;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                    {
                        bodyIndex = i;
                        bodyExists = true;
                        break;
                    }
                }
                if (bodyExists)
                {
                    Projectile.NewProjectileDirect(player.Center, Main.rand.NextVector2Unit(), ModContent.ProjectileType<EndoHydraHead>(), damage, knockBack, player.whoAmI, bodyIndex);
                }
                else
                {
                    bodyIndex = Projectile.NewProjectile(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                    Projectile.NewProjectile(player.Center, Main.rand.NextVector2Unit(), ModContent.ProjectileType<EndoHydraHead>(), damage, knockBack, player.whoAmI, bodyIndex);
                    for (int i = 0; i < 72; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Main.projectile[bodyIndex].Center, 113);
                        dust.velocity = (MathHelper.TwoPi * Vector2.Dot((i / 72f * MathHelper.TwoPi).ToRotationVector2(), player.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(i / 72f * -MathHelper.TwoPi))).ToRotationVector2();
                        dust.velocity = dust.velocity.RotatedBy(i / 36f * MathHelper.TwoPi) * 8f;
                        dust.noGravity = true;
                        dust.scale = 1.9f;
                    }
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.StaffoftheFrostHydra);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
