using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class BlazingStar : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.damage = 75;
            Item.DamageType = RogueDamageClass.Instance;
            Item.crit = 4;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item1;

            Item.shootSpeed = 13f;
            Item.shoot = ModContent.ProjectileType<BlazingStarProj>();
        }
        public override float StealthVelocityMultiplier => 1.3f;

        public override float StealthDamageMultiplier => 0.5f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, -1f);
            if (player.Calamity().StealthStrikeAvailable())
            {
                p.Calamity().stealthStrike = true;
                p.netUpdate = true;

                var goType = ModContent.ProjectileType<BlazingStarOrbital>();
                if (player.ownedProjectileCounts[goType] <= 0)
                {
                    p = Projectile.NewProjectileDirect(source, position, Vector2.Zero, goType, damage, 10, player.whoAmI, 0f, -1f);
                    p.Calamity().stealthStrike = true;
                }
                else
                {
                    foreach (var proj in Main.ActiveProjectiles)
                    {
                        if (proj.active && proj.type == goType && proj.owner == player.whoAmI)
                        {
                            proj.timeLeft += 300;
                            proj.netUpdate = true;
                            break;
                        }
                    }
                }

            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Glaive>().
                AddIngredient(ItemID.HellstoneBar, 5).
                AddIngredient<EssenceofHavoc>(10).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
