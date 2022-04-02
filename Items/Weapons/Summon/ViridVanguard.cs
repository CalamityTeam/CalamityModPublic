using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 280;
            item.summon = true;
            item.sentry = true;
            item.mana = 10;
            item.width = 26;
            item.height = 36;
            item.useTime = item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ViridVanguardBlade>();
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
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
                        if ((Main.projectile[i].modProjectile as ViridVanguardBlade).FiringTime > 0f)
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

                Projectile newBlade = Projectile.NewProjectileDirect(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                (newBlade.modProjectile as ViridVanguardBlade).AltTexture = swordCount % 2 == 1;
                newBlade.netUpdate = true;
                swordCount++;

                newBlade = Projectile.NewProjectileDirect(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                (newBlade.modProjectile as ViridVanguardBlade).AltTexture = swordCount % 2 == 1;
                newBlade.netUpdate = true;
                swordCount++;

                float angleVariance = MathHelper.TwoPi / swordCount;
                float angle = 0f;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                    {
                        if ((Main.projectile[i].modProjectile as ViridVanguardBlade).FiringTime > 0f)
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<IgneousExaltation>());
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
