using Terraria.DataStructures;
using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ElementalExcalibur : ModItem
    {
        private static int BaseDamage = 2000;
        private int BeamType = 0;
        private const int alpha = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Excalibur");
            Tooltip.SetDefault("Freezes enemies and heals the player on hit\n" +
                "Fires rainbow beams that change their behavior based on their color\n" +
                "Right click for true melee");
        }

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.useAnimation = 14;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 14;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.width = 112;
            Item.height = 112;
            Item.value = Item.buyPrice(5, 0, 0, 0);
            Item.rare = ItemRarityID.Purple;
            Item.shoot = ModContent.ProjectileType<ElementalExcaliburBeam>();
            Item.shootSpeed = 6f;
            Item.Calamity().customRarity = CalamityRarity.Rainbow;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, (float)BeamType, 0f);

            BeamType++;
            if (BeamType > 11)
                BeamType = 0;

            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ProjectileID.None;
                Item.shootSpeed = 0f;
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<ElementalExcaliburBeam>();
                Item.shootSpeed = 12f;
            }

            return base.CanUseItem(player);
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (player.altFunctionUse == 2)
                damage *= 2;
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            if (player.altFunctionUse == 2)
                damage *= 2;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                Color color = new Color(255, 0, 0, alpha);
                switch (BeamType)
                {
                    case 0: // Red
                        break;
                    case 1: // Orange
                        color = new Color(255, 128, 0, alpha);
                        break;
                    case 2: // Yellow
                        color = new Color(255, 255, 0, alpha);
                        break;
                    case 3: // Lime
                        color = new Color(128, 255, 0, alpha);
                        break;
                    case 4: // Green
                        color = new Color(0, 255, 0, alpha);
                        break;
                    case 5: // Turquoise
                        color = new Color(0, 255, 128, alpha);
                        break;
                    case 6: // Cyan
                        color = new Color(0, 255, 255, alpha);
                        break;
                    case 7: // Light Blue
                        color = new Color(0, 128, 255, alpha);
                        break;
                    case 8: // Blue
                        color = new Color(0, 0, 255, alpha);
                        break;
                    case 9: // Purple
                        color = new Color(128, 0, 255, alpha);
                        break;
                    case 10: // Fuschia
                        color = new Color(255, 0, 255, alpha);
                        break;
                    case 11: // Hot Pink
                        color = new Color(255, 0, 128, alpha);
                        break;
                    default:
                        break;
                }

                Dust dust24 = Main.dust[Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 267, 0f, 0f, alpha, color, 1.2f)];
                dust24.noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 60);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(BuffID.OnFire, 360);

            if (!target.canGhostHeal || player.moonLeech)
                return;

            int healAmount = Main.rand.Next(3) + 10;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<ExoFreeze>(), 60);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
            target.AddBuff(BuffID.Frostburn, 300);
            target.AddBuff(BuffID.OnFire, 360);

            if (player.moonLeech)
                return;

            int healAmount = Main.rand.Next(3) + 10;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GreatswordofBlah>().
                AddIngredient(ItemID.TrueExcalibur).
                AddIngredient(ItemID.LargeDiamond).
                AddIngredient(ItemID.LightShard, 3).
                AddIngredient(ItemID.DarkShard, 3).
                AddIngredient<LivingShard>(10).
                AddIngredient<GalacticaSingularity>(10).
                AddIngredient(ItemID.SoulofLight, 20).
                AddIngredient(ItemID.SoulofNight, 20).
                AddIngredient<ShadowspecBar>(5).
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
