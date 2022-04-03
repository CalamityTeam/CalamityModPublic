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
            Item.width = 42;
            Item.height = 40;
            Item.damage = 159;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<JawsProjectile>();
            Item.shootSpeed = 25f;
            Item.Calamity().rogue = true;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
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
                    int p = Projectile.NewProjectile(position, currentDirection, type, (int)(damage * 1.8), knockBack + 6f, player.whoAmI);
                    if (p.WithinBounds(Main.maxProjectiles))
                        Main.projectile[p].Calamity().stealthStrike = true;
                }
                else
                {
                    Projectile.NewProjectile(position, currentDirection, type, damage, knockBack, player.whoAmI);
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<LeviathanTeeth>()).AddIngredient(ModContent.ItemType<ReaperTooth>(), 6).AddIngredient(ModContent.ItemType<Lumenite>(), 15).AddIngredient(ModContent.ItemType<RuinousSoul>(), 2).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
