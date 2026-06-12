using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class UrchinStinger : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [ModContent.BuffType<Irradiated>()];
        }
        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 32;
            Item.damage = 13;
            Item.DamageType = RogueDamageClass.Instance;
            Item.useAnimation = Item.useTime = 17;
            Item.knockBack = 1.5f;

            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<UrchinStingerProj>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 newVel = velocity.RotatedByRandom(MathHelper.ToRadians(23f)) * 0.6f;
            Vector2 newVel2 = velocity.RotatedByRandom(MathHelper.ToRadians(23f)) * 0.8f;
            if (!player.Calamity().StealthStrikeAvailable())
            {
                Projectile.NewProjectile(source, position, newVel, type, damage / 2, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, newVel2, type, damage / 2, knockback, player.whoAmI);
            }

            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (player.Calamity().StealthStrikeAvailable())
                    Main.projectile[proj].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
