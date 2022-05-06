using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
namespace CalamityMod.Items.Weapons.Summon
{
    public class ViridVanguard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Virid Vanguard");
            Tooltip.SetDefault("Summons 2 blades that rotate around you\n" +
                               "Each pair of blades take up three minion slots\n" +
                               "Right clicking prompts all blades to fly upward and redirect towards the mouse");
            Item.staff[Item.type] = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 280;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 36;
            Item.useTime = Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = 10;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ViridVanguardBlade>();
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float totalMinionSlots = 0f;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].minion && Main.projectile[i].owner == player.whoAmI)
                {
                    totalMinionSlots += Main.projectile[i].minionSlots;
                }
            }
            if (player.altFunctionUse != 2 && totalMinionSlots + 1.5f < player.maxMinions)
            {
                position = Main.MouseWorld;
                int swordCount = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                    {
                        if ((Main.projectile[i].ModProjectile as ViridVanguardBlade).FiringTime > 0f)
                            continue;
                        swordCount++;
                        for (int j = 0; j < 22; j++)
                        {
                            Dust dust = Dust.NewDustDirect(Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height, 2);
                            dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool(2).ToDirectionInt();
                            dust.noGravity = true;
                        }
                    }
                }

                Projectile newBlade = Projectile.NewProjectileDirect(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
                (newBlade.ModProjectile as ViridVanguardBlade).AltTexture = swordCount % 2 == 1;
                newBlade.originalDamage = Item.damage;
                newBlade.netUpdate = true;
                swordCount++;

                newBlade = Projectile.NewProjectileDirect(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
                (newBlade.ModProjectile as ViridVanguardBlade).AltTexture = swordCount % 2 == 1;
                newBlade.originalDamage = Item.damage;
                newBlade.netUpdate = true;
                swordCount++;

                float angleVariance = MathHelper.TwoPi / swordCount;
                float angle = 0f;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                    {
                        if ((Main.projectile[i].ModProjectile as ViridVanguardBlade).FiringTime > 0f)
                            continue;
                        Main.projectile[i].ai[0] = angle;
                        Main.projectile[i].localAI[1] = 80f;
                        angle += angleVariance;
                        for (int j = 0; j < 22; j++)
                        {
                            Dust dust = Dust.NewDustDirect(Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height, 6);
                            dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool(2).ToDirectionInt();
                            dust.noGravity = true;
                        }
                    }
                }
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<IgneousExaltation>().
                AddIngredient<UeliaceBar>(8).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
