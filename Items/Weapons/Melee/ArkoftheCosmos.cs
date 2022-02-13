using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ArkoftheCosmos : ModItem
    {
        public float Combo = 0f;
        public float Charge = 0f;
        public override bool CloneNewInstances => true;

        public static float snapDamageMultiplier = 1.3f; //Extra damage from making the scissors snap
        public static float chargeDamageMultiplier = 1.6f; //Extra damage from charge
        public static float chainDamageMultiplier = 0.1f;

        const string ComboTooltip = "Performs a combo of swings, alternating between narrow and wide swings and throwing the blade out every 5 swings\n" +
                "The thrown blade is held in place by constellations and will follow your cursor\n" +
                "Releasing the mouse while the blade is out will throw the second half towards it, making the scissors snap and explode into stars";

        const string ParryTooltip = "Using RMB will snip out the scissor blades in front of you. Hitting an enemy with it will parry them, granting you a small window of invulnerability\n" +
                "You can also parry projectiles and temporarily make them deal 200 less damage\n" +
                "Parrying will empower the next 10 swings of the sword, letting you use both blades at once\n" +
                "Using RMB and pressing up while the Ark is charged will release all the charges in a powerful burst of energy";


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ark of the Cosmos");
            Tooltip.SetDefault("This line gets set in ModifyTooltips\n" +
                "This line also gets set in ModifyTooltips\n" +
                "The physical culmination of your journey, capable to rend gods asunder");
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var comboTooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip0" && x.mod == "Terraria");
            comboTooltip.text = ComboTooltip;
            comboTooltip.overrideColor = Color.Lerp(Color.Gold, Color.HotPink, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.5f);

            var parryTooltip = tooltips.FirstOrDefault(x => x.Name == "Tooltip1" && x.mod == "Terraria");
            parryTooltip.text = ParryTooltip;
            parryTooltip.overrideColor = Color.Lerp(Color.HotPink, Color.DeepSkyBlue, 0.5f + (float)Math.Sin(Main.GlobalTime) * 0.5f);
        }

        public override void SetDefaults()
        {
            item.width = item.height = 136;
            item.damage = 140;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 9.5f;
            item.UseSound = SoundID.Item60;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 28f;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 15;

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            player.Calamity().mouseWorldListener = true;

            if (CanUseItem(player) && Combo != 4)
                item.channel = false;

            if (Combo == 4)
                item.channel = true;
        }
        public override bool CanUseItem(Player player)
        {
            return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ProjectileType<ArkoftheCosmosSwungBlade>());
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                if (Charge > 0 && player.controlUp)
                {
                    float angle = new Vector2(speedX, speedY).ToRotation();
                    Projectile.NewProjectile(player.Center + angle.ToRotationVector2() * 90f, new Vector2(speedX, speedY), ProjectileType<ArkoftheElementsSnapBlast>(), (int)(damage * Charge * 1.8f), 0, player.whoAmI, angle, 600);

                    if (Main.LocalPlayer.Calamity().GeneralScreenShakePower < 3)
                        Main.LocalPlayer.Calamity().GeneralScreenShakePower = 3;

                    Charge = 0;
                }

                else if (!Main.projectile.Any(n => n.active && n.owner == player.whoAmI && (n.type == ProjectileType<ArkoftheAncientsParryHoldout>() || n.type == ProjectileType<TrueArkoftheAncientsParryHoldout>() || n.type == ProjectileType<ArkoftheElementsParryHoldout>())))
                    Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<ArkoftheElementsParryHoldout>(), damage, 0, player.whoAmI, 0, 0);

                return false;
            }

            if (Charge > 0)
                damage = (int)(chargeDamageMultiplier * damage);

            float scissorState = Combo == 4 ? 2 : Combo % 2;

            Projectile.NewProjectile(player.Center, new Vector2(speedX, speedY), ProjectileType<ArkoftheCosmosSwungBlade>(), damage, knockBack, player.whoAmI, scissorState, Charge);


            //Shoot projectiles 
            if (true && false)
            {
                Vector2 throwVector = new Vector2(speedX, speedY);
                float empoweredNeedles = Charge > 0 ? 1f : 0f;
                Projectile.NewProjectile(player.Center + Vector2.Normalize(throwVector) * 20, new Vector2(speedX, speedY) * 2.8f, ProjectileType<SolarNeedle>(), (int)(damage * 0.5f), knockBack, player.whoAmI, empoweredNeedles);


                Vector2 Shift = Vector2.Normalize(new Vector2(speedX, speedY).RotatedBy(MathHelper.PiOver2)) * 20;

                Projectile.NewProjectile(player.Center + Shift, throwVector.RotatedBy(MathHelper.PiOver4 * 0.3f), ProjectileType<ElementalGlassStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI);
                Projectile.NewProjectile(player.Center + Shift * 1.2f, throwVector.RotatedBy(MathHelper.PiOver4 * 0.4f) * 0.8f, ProjectileType<ElementalGlassStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI);


                Projectile.NewProjectile(player.Center - Shift, throwVector.RotatedBy(-MathHelper.PiOver4 * 0.3f), ProjectileType<ElementalGlassStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI);
                Projectile.NewProjectile(player.Center - Shift * 1.2f, throwVector.RotatedBy(-MathHelper.PiOver4 * 0.4f) * 0.8f, ProjectileType<ElementalGlassStar>(), (int)(damage * 0.2f), knockBack, player.whoAmI);
            }

            Combo += 1;
            if (Combo > 4)
                Combo = 0;

            Charge--;
            if (Charge < 0)
                Charge = 0;

            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemType<FourSeasonsGalaxia>());
            recipe.AddIngredient(ItemType<ArkoftheElements>());
            recipe.AddIngredient(ItemType<AuricBar>(), 5);
            recipe.AddTile(TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override ModItem Clone(Item item)
        {
            var clone = base.Clone(item);

            (clone as ArkoftheCosmos).Charge = (item.modItem as ArkoftheCosmos).Charge;

            return clone;
        }
        public override ModItem Clone()
        {
            var clone = base.Clone();

            (clone as ArkoftheCosmos).Charge = Charge;

            return clone;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Charge);
        }

        public override void NetRecieve(BinaryReader reader)
        {
            Charge = reader.ReadInt32();
        }
    }
}
