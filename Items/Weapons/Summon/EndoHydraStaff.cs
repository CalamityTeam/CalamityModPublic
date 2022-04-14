using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Summon
{
    public class EndoHydraStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Endo Hydra Staff");
            Tooltip.SetDefault("Summons a frigid entity with a head\n" +
                               "If the entity already exists, using this item again will cause it to gain more heads");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 60;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item60;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.damage = 225;
            Item.knockBack = 3f;
            Item.autoReuse = true;
            Item.useTime = Item.useAnimation = 10;
            Item.shoot = ModContent.ProjectileType<EndoHydraBody>();
            Item.shootSpeed = 10f;

            Item.value = Item.buyPrice(2, 50, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
                    int p = Projectile.NewProjectile(source, player.Center, Main.rand.NextVector2Unit(), ModContent.ProjectileType<EndoHydraHead>(), damage, knockback, player.whoAmI, bodyIndex);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = Item.damage;
                }
                else
                {
                    bodyIndex = Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);
                    int head = Projectile.NewProjectile(source, player.Center, Main.rand.NextVector2Unit(), ModContent.ProjectileType<EndoHydraHead>(), damage, knockback, player.whoAmI, bodyIndex);
                    if (Main.projectile.IndexInRange(bodyIndex))
                        Main.projectile[bodyIndex].originalDamage = Item.damage;
                    if (Main.projectile.IndexInRange(head))
                        Main.projectile[head].originalDamage = Item.damage;
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
            CreateRecipe(1).AddIngredient(ItemID.StaffoftheFrostHydra).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 8).AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 20).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
