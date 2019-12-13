using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Nychthemeron : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nychthemeron");
            Tooltip.SetDefault("Throws a spiky ball that ignores gravity and summons a pair of dark and light orbs that orbit the player\n" +
                "Once the spiky ball disappears the orbs will home in on the nearest target\n" +
                "Stacks up to 10\n" +
                "Stealth strikes cause all spiky balls and orbs to be thrown at once\n" +
				"Right click to recall all existing spiky balls");
        }

        public override void SafeSetDefaults()
        {
            item.width = 18;
            item.damage = 30;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = 1;
            item.useTime = 20;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 18;
            item.maxStack = 10;
            item.value = Item.buyPrice(0, 3, 60, 0);
            item.rare = 6;
            item.shoot = ModContent.ProjectileType<NychthemeronProjectile>();
            item.shootSpeed = 5f;
            item.Calamity().rogue = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            for (int i = 0; i < 1000; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.type == ModContent.ProjectileType<NychthemeronProjectile>() && p.owner == player.whoAmI)
                {
                    p.ai[0] = 1f;
                }
            }
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int orbDamage = (int)(damage * 0.75f);

            if (player.Calamity().StealthStrikeAvailable())
            {
                for (int j = 0; j < item.stack - player.ownedProjectileCounts[item.shoot]; j++)
                {
                    float spread = 2;
                    int pIndex = Projectile.NewProjectile(position.X, position.Y, speedX + Main.rand.NextFloat(-spread, spread), speedY + Main.rand.NextFloat(-spread, spread), type, damage, knockBack, player.whoAmI, 0f, 1f);
                    Projectile p = Main.projectile[pIndex];
                    p.Calamity().stealthStrike = true;
                    int pID = p.identity;

                    CreateOrbs(position, orbDamage, knockBack, pID, player);
                }
            }
            else
            {
                int pIndex = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 1f);
                int pID = Main.projectile[pIndex].identity;
                
                CreateOrbs(position, orbDamage, knockBack, pID, player);
            }
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[item.shoot] < item.stack && player.altFunctionUse != 2;
        }

        //public override void AddRecipes()
        //{
        //    ModRecipe recipe = new ModRecipe(mod);
        //    recipe.AddIngredient(ItemID.SpikyBall, 15);
        //    recipe.AddIngredient(ItemID.LightShard);
        //    recipe.AddIngredient(ItemID.DarkShard);
        //    recipe.AddIngredient(ItemID.HallowedBar, 2);
        //    recipe.AddTile(TileID.Anvils);
        //    recipe.SetResult(this);
        //    recipe.AddRecipe();
        //}

        private static void CreateOrbs(Vector2 position, int damage, float knockBack, int projectileID, Player player)
        {
            float rotationOffset = 0f;

            // Ideally new projectiles will fill in the most recently vacated spots in the pattern
            int[] activeSlots = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.type == ModContent.ProjectileType<NychthemeronOrb>() && proj.owner == player.whoAmI && proj.active && proj.localAI[0] == 0f && activeSlots[(int)proj.localAI[1]] == -1)
                {
                    activeSlots[(int)proj.localAI[1]] = i;
                }
            }

            int pos = 0;
            bool assignedOffset = false;
            for (int i = 0; i < 10; i++)
            {
                if (activeSlots[i] != -1)
                {
                    rotationOffset = Main.projectile[activeSlots[i]].rotation;
                    assignedOffset = true;
                }
                if (activeSlots[i] == -1 && assignedOffset)
                {
                    pos = i;
                    break;
                }
            }

            float orb1Col = 0f;
            float orb2Col = 1f;

            if (pos > 0 && pos < 5)
            {
                rotationOffset += MathHelper.ToRadians(45f);

                orb1Col = pos % 2;
                orb2Col = pos % 2;
            }
            else if (pos >= 5)
            {
                rotationOffset += MathHelper.ToRadians(72f);
            }

            int orb1 = Projectile.NewProjectile(position.X, position.Y, 0f, 0f, ModContent.ProjectileType<NychthemeronOrb>(), damage, knockBack, player.whoAmI, orb1Col, projectileID);
            int orb2 = Projectile.NewProjectile(position.X, position.Y, 0f, 0f, ModContent.ProjectileType<NychthemeronOrb>(), damage, knockBack, player.whoAmI, orb2Col, projectileID);
            Main.projectile[orb1].localAI[1] = pos;
            Main.projectile[orb2].localAI[1] = pos;
            Main.projectile[orb1].rotation = rotationOffset;
            Main.projectile[orb2].rotation = rotationOffset + MathHelper.ToRadians(180f);
        }
    }
}
