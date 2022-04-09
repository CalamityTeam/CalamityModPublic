using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DraedonsExoblade : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exoblade");
            Tooltip.SetDefault("Ancient blade of Yharim's weapons and armors expert, Draedon\n" +
                               "Fires an exo beam that homes in on the player and explodes\n" +
                               "Striking an enemy with the blade causes several comets to fire\n" +
                               "All attacks briefly freeze enemies hit\n" +
                               "Enemies hit at very low HP explode into frost energy and freeze nearby enemies");
        }

        public override void SetDefaults()
        {
            Item.width = 80;
            Item.damage = 900;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 14;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 114;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<Exobeam>();
            Item.shootSpeed = 19f;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 107, 0f, 0f, 100, new Color(0, 255, 255));
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            if (crit)
                damage /= 2;

            if (target.life <= (target.lifeMax * 0.05f))
                Projectile.NewProjectile(source, target.Center, Vector2.Zero, ModContent.ProjectileType<Exoboom>(), damage, knockback, Main.myPlayer);

            target.ExoDebuffs();
            SoundEngine.PlaySound(SoundID.Item88, player.Center);
            float xPos = player.position.X + 800 * Main.rand.NextBool(2).ToDirectionInt();
            float yPos = player.position.Y + Main.rand.Next(-800, 801);
            Vector2 startPos = new Vector2(xPos, yPos);
            Vector2 velocity = target.position - startPos;
            float dir = 10 / startPos.X;
            velocity.X *= dir * 150;
            velocity.Y *= dir * 150;
            velocity.X = MathHelper.Clamp(velocity.X, -15f, 15f);
            velocity.Y = MathHelper.Clamp(velocity.Y, -15f, 15f);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Exocomet>()] < 8)
            {
                for (int comet = 0; comet < 2; comet++)
                {
                    float ai1 = Main.rand.NextFloat() + 0.5f;
                    Projectile.NewProjectile(source, startPos, velocity, ModContent.ProjectileType<Exocomet>(), damage, knockback, player.whoAmI, 0f, ai1);
                }
            }

            if (!target.canGhostHeal || player.moonLeech)
                return;

            int healAmount = Main.rand.Next(3) + 5;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            if (crit)
                damage /= 2;

            if (target.statLife <= (target.statLifeMax2 * 0.05f))
                Projectile.NewProjectile(source, target.Center.X, target.Center.Y, 0f, 0f, ModContent.ProjectileType<Exoboom>(), damage, Item.knockBack, Main.myPlayer);

            target.ExoDebuffs();
            SoundEngine.PlaySound(SoundID.Item88, player.Center);
            float xPos = player.position.X + 800 * Main.rand.NextBool(2).ToDirectionInt();
            float yPos = player.position.Y + Main.rand.Next(-800, 801);
            Vector2 startPos = new Vector2(xPos, yPos);
            Vector2 velocity = target.position - startPos;
            float dir = 10 / startPos.X;
            velocity.X *= dir * 150;
            velocity.Y *= dir * 150;
            velocity.X = MathHelper.Clamp(velocity.X, -15f, 15f);
            velocity.Y = MathHelper.Clamp(velocity.Y, -15f, 15f);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<Exocomet>()] < 8)
            {
                for (int comet = 0; comet < 2; comet++)
                {
                    float ai1 = Main.rand.NextFloat() + 0.5f;
                    Projectile.NewProjectile(source, startPos, velocity, ModContent.ProjectileType<Exocomet>(), damage, Item.knockBack, player.whoAmI, 0f, ai1);
                }
            }

            if (player.moonLeech)
                return;

            int healAmount = Main.rand.Next(3) + 5;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Terratomere>().
                AddIngredient<AnarchyBlade>().
                AddIngredient<FlarefrostBlade>().
                AddIngredient<PhoenixBlade>().
                AddIngredient<StellarStriker>().
                AddIngredient<MiracleMatter>().
                AddTile(ModContent.TileType<DraedonsForge>()).
                Register();
        }
    }
}
