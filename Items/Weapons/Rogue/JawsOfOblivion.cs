using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class JawsOfOblivion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jaws of Oblivion");
            Tooltip.SetDefault("Throws a tight spread of six venomous reaper fangs that stick in enemies\n" +
                "Stealth strikes cause the teeth to emit a crushing shockwave on impact\n" +
                "You're gonna need a bigger boat");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
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
            Item.DamageType = RogueDamageClass.Instance;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float spreadAngle = MathHelper.ToRadians(2.5f);
            Vector2 direction = velocity;
            Vector2 baseDirection = direction.RotatedBy(-spreadAngle * 2.5f);

            for (int i = 0; i < 6; i++)
            {
                Vector2 currentDirection = baseDirection.RotatedBy(spreadAngle * i);
                currentDirection = currentDirection.RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-1f, 1f)));

                if (player.Calamity().StealthStrikeAvailable())
                {
                    int p = Projectile.NewProjectile(source, position, currentDirection, type, (int)(damage * 1.8), knockback + 6f, player.whoAmI);
                    if (p.WithinBounds(Main.maxProjectiles))
                        Main.projectile[p].Calamity().stealthStrike = true;
                }
                else
                {
                    Projectile.NewProjectile(source, position, currentDirection, type, damage, knockback, player.whoAmI);
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LeviathanTeeth>().
                AddIngredient<ReaperTooth>(6).
                AddIngredient<Lumenyl>(15).
                AddIngredient<RuinousSoul>(2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
