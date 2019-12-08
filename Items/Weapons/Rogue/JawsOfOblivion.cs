using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class JawsOfOblivion : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jaws of Oblivion");
            Tooltip.SetDefault("Throws a tight spread of six venomous reaper fangs that stick in enemies\n" +
                "Stealth strikes cause the teeth to emit a crushing shockwave on impact");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.damage = 438;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 40;
            item.maxStack = 1;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<JawsProjectile>();
            item.shootSpeed = 25f;
            item.Calamity().postMoonLordRarity = 13;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float spreadAngle = MathHelper.ToRadians(2.5f);
            Vector2 direction = new Vector2(speedX, speedY);
            Vector2 baseDirection = direction.RotatedBy(-spreadAngle * 2.5f);

            for (int i = 0; i < 6; i++)
            {
                Vector2 currentDirection = baseDirection.RotatedBy(spreadAngle * i);
                currentDirection = currentDirection.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-1f, 1f)));

                if (player.Calamity().StealthStrikeAvailable())
                {
                    int p = Projectile.NewProjectile(position.X, position.Y, currentDirection.X, currentDirection.Y, type, damage, knockBack, player.whoAmI, 0f, 0f);
                    Main.projectile[p].Calamity().stealthStrike = true;
                }
                else
                {
                    int p = Projectile.NewProjectile(position.X, position.Y, currentDirection.X, currentDirection.Y, type, (int)(damage * 1.5f), 10, player.whoAmI, 0f, 0f);
                }
            }
            return false;
        }

        /*
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LeviathanTeeth>());
            recipe.AddIngredient(ModContent.ItemType<ReaperTooth>(), 6);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 15);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
        */
    }
}
