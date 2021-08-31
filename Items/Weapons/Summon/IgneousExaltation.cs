using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class IgneousExaltation : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Igneous Exaltation");
            Tooltip.SetDefault("Summons an orbiting blade\n" +
                               "Right click to launch all blades towards the cursor");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 34;
            item.mana = 10;
            item.width = 52;
            item.height = 50;
            item.useTime = item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 4.5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.UseSound = SoundID.Item71;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<IgneousBlade>();
            item.shootSpeed = 10f;
            item.summon = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float totalMinionSlots = 0f;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].minion && Main.projectile[i].owner == player.whoAmI)
                {
                    totalMinionSlots += Main.projectile[i].minionSlots;
                }
            }
            if (player.altFunctionUse != 2 && totalMinionSlots < player.maxMinions)
            {
                position = Main.MouseWorld;
                Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
                int swordCount = 0;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI)
                    {
                        if ((Main.projectile[i].modProjectile as IgneousBlade).Firing)
                            continue;
                        swordCount++;
                        for (int j = 0; j < 22; j++)
                        {
                            Dust dust = Dust.NewDustDirect(Main.projectile[i].position, Main.projectile[i].width, Main.projectile[i].height, 6);
                            dust.velocity = Vector2.UnitY * Main.rand.NextFloat(3f, 5.5f) * Main.rand.NextBool(2).ToDirectionInt();
                            dust.noGravity = true;
                        }
                    }
                }
                float angleVariance = MathHelper.TwoPi / swordCount;
                float angle = 0f;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == type && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].localAI[1] == 0f)
                    {
                        if ((Main.projectile[i].modProjectile as IgneousBlade).Firing)
                            continue;
                        Main.projectile[i].ai[0] = angle;
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
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<UnholyCore>(), 10);
            r.AddIngredient(ModContent.ItemType<EssenceofChaos>(), 5);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
