using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Glaive : RogueWeapon
    {
        public static int BaseDamage = 45;
        public static float Knockback = 3f;
        public static float Speed = 10f;
        public static float StealthSpeedMult = 1.8f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Glaive");
            Tooltip.SetDefault(@"Stacks up to 3
Stealth strikes are super fast and pierce infinitely");
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.DamageType = RogueDamageClass.Instance;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.width = 1;
            Item.height = 1;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = Knockback;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice / 3; // Stacks up to 3
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item1;
            Item.maxStack = 3;

            Item.shootSpeed = Speed;
            Item.shoot = ModContent.ProjectileType<GlaiveProj>();
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 4;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float ai1 = 0f;
            if (player.Calamity().StealthStrikeAvailable())
            {
                velocity.X *= StealthSpeedMult;
                velocity.Y *= StealthSpeedMult;
                ai1 = 1f;
            }

            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, ai1);
            if (player.Calamity().StealthStrikeAvailable() && p.WithinBounds(Main.maxProjectiles))
                Main.projectile[p].Calamity().stealthStrike = true;
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < Item.stack;
        }

    }
}
