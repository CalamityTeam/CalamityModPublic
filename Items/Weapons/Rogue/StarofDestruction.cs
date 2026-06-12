using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StarofDestruction : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 94;
            Item.damage = 400;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 90;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<DestructionBolt>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override float StealthDamageMultiplier => 0.25f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                bool spawner = true;
                for (int t = 0; t < 4; t++)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        float rot = MathHelper.ToRadians(-36f) + MathHelper.ToRadians(18f) * i;
                        Vector2 vel = velocity.RotatedBy(rot) * (i == 0 ? 0.7f : i == 4 ? 0.7f : i == 1 ? 0.85f : i == 3 ? 0.85f : 1);
                        int proj = Projectile.NewProjectile(source, position, vel, type, damage, knockback, player.whoAmI, 0f, i);
                        if (proj.WithinBounds(Main.maxProjectiles))
                            Main.projectile[proj].ai[2] = 1; // This counts as a stealth strike in this case
                        if (spawner && i == 4)
                        { 
                            Main.projectile[proj].ai[2] = 15;
                            spawner = false;
                        }
                    }
                    velocity = velocity.RotatedBy(MathHelper.ToRadians(90f));
                }
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    float rot = -0.8f + 0.4f * i;
                    Vector2 vel = velocity.RotatedBy(rot) * (i == 0 ? 0.7f : i == 4 ? 0.7f : i == 1 ? 0.85f : i == 3 ? 0.85f : 1);
                    int proj = Projectile.NewProjectile(source, position, vel, type, damage, knockback, player.whoAmI, 0f, i);
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MeldBlob>(18).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
