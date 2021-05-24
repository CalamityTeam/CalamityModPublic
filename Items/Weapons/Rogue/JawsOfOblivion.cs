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
                "Stealth strikes cause the teeth to emit a crushing shockwave on impact\n" +
                "You're gonna need a bigger boat");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 40;
            item.damage = 159;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 15;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<JawsProjectile>();
            item.shootSpeed = 25f;
            item.Calamity().rogue = true;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool usingStealthStrike = player.Calamity().StealthStrikeAvailable();
            float spreadAngle = MathHelper.ToRadians(2.5f);
            Vector2 direction = new Vector2(speedX, speedY);
            Vector2 baseDirection = direction.RotatedBy(-spreadAngle * 2.5f);

            if (usingStealthStrike)
                damage = (int)(damage * 1.8);

            for (int i = 0; i < 6; i++)
            {
                Vector2 currentDirection = baseDirection.RotatedBy(spreadAngle * i);
                currentDirection = currentDirection.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-1f, 1f)));

                if (usingStealthStrike)
                {
                    int p = Projectile.NewProjectile(position, currentDirection, type, damage, knockBack + 6f, player.whoAmI);
                    if (p.WithinBounds(Main.maxProjectiles))
                        Main.projectile[p].Calamity().stealthStrike = true;
                }
                else
                    Projectile.NewProjectile(position, currentDirection, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<LeviathanTeeth>());
            recipe.AddIngredient(ModContent.ItemType<ReaperTooth>(), 6);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 15);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
